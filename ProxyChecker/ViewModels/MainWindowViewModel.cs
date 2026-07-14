using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Loaders;
using ProxyChecker.Interfaces.ViewModels;
using ProxyChecker.Services;
using ProxyChecker.Storage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyChecker.ViewModels
{
  internal partial class MainWindowViewModel : ViewModelBase, IRequireWindow
  {
    private readonly IDesktopService _desktopService;
    private readonly IWindowFactory _windowFactory;
    private readonly AppDbContext _db;
    private readonly IEnumerable<ILoaderCreator> _loaderCreators;
    private readonly ProxyCheckerService _proxyCheckerService;

    public MainWindowViewModel(
      IDesktopService desktopService,
      IWindowFactory windowFactory,
      AppDbContext db,
      IEnumerable<ILoaderCreator> loaderCreators,
      ProxyCheckerService proxyCheckerService
      )
    {
      _desktopService = desktopService;
      _windowFactory = windowFactory ?? throw new System.ArgumentNullException(nameof(windowFactory));
      _db = db;
      _loaderCreators = loaderCreators;
      _proxyCheckerService = proxyCheckerService ?? throw new System.ArgumentNullException(nameof(proxyCheckerService));
    }

    [ObservableProperty]
    private ObservableCollection<ProxyViewModel> _loadedProxies = new();

    [ObservableProperty]
    private ObservableCollection<ProxyViewModel> _validProxies = new();

    public Window Window { get; set; } = default!;

    [RelayCommand]
    private async Task LoadProxiesAsync(CancellationToken cancellationToken)
    {
      var appSettings = _db.Settings.Single();

      if (appSettings.LoaderId is null)
      {
        ShowLoaders();
        
        return;
      }

      var dbLoader = await _db.Loaders.FindAsync(appSettings.LoaderId.Value, cancellationToken);

      if (dbLoader is null)
      {
        return;
      }

      var loaderCreator = _loaderCreators.SingleOrDefault(c => c.Uid == dbLoader.CreatorUid);

      if (loaderCreator is null)
      {
        return;
      }

      var loader = loaderCreator.Create();

      loader.SetSettings(dbLoader.JsonSettings is null ? null : JToken.Parse(dbLoader.JsonSettings));

      await foreach(var proxy in loader.LoadAsync(cancellationToken))
      {
        LoadedProxies.Add(
          new ProxyViewModel(
            proxy
          )
        );
      }

      ClearProxiesCommand.NotifyCanExecuteChanged();
      CheckProxiesCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanClearProxies))]
    private void ClearProxies()
    {
      LoadedProxies.Clear();

      ClearProxiesCommand.NotifyCanExecuteChanged();
      CheckProxiesCommand.NotifyCanExecuteChanged();
    }

    private bool CanClearProxies() => LoadedProxies.Any();

    private CancellationTokenSource? _proxyCheckingCancellationTokenSource;

    [RelayCommand(CanExecute = nameof(CanCheckProxies))]
    private async Task CheckProxiesAsync(CancellationToken cancellationToken)
    {
      ValidProxies.Clear();

      try
      {
        _proxyCheckingCancellationTokenSource = new CancellationTokenSource();
        cancellationToken.Register(() => {
          _proxyCheckingCancellationTokenSource?.Cancel();
        });

        CancelProxyCheckingCommand.NotifyCanExecuteChanged();

        await foreach (var proxy in _proxyCheckerService.CheckAsync(LoadedProxies.Select(pvm => pvm.ToProxy()), _proxyCheckingCancellationTokenSource.Token))
        {
          ValidProxies.Add(
            new ProxyViewModel(proxy)
          );
        }
      }
      finally
      {
        _proxyCheckingCancellationTokenSource?.Dispose();
        _proxyCheckingCancellationTokenSource = null;

        CancelProxyCheckingCommand.NotifyCanExecuteChanged();
        ExportProxiesCommand.NotifyCanExecuteChanged();
      }
    }

    [RelayCommand(CanExecute = nameof(CanCancelProxyChecking))]
    private async Task CancelProxyCheckingAsync()
    {
      _proxyCheckingCancellationTokenSource?.Cancel();
    }

    public bool CanCancelProxyChecking()
      => _proxyCheckingCancellationTokenSource != null;

    private bool CanCheckProxies() => LoadedProxies.Any();

    [RelayCommand(CanExecute = nameof(CanExportProxies))]
    private void ExportProxies()
    {
    }

    private bool CanExportProxies() => ValidProxies.Any();

    [RelayCommand]
    private void Exit()
    {
      _desktopService.Desktop.Shutdown();
    }

    [RelayCommand]
    private void ShowLoaders()
    {
      var dialog = _windowFactory.CreateWindow<LoadersWindow>();

      dialog.ShowDialog(Window);
    }

    [RelayCommand]
    private void ShowCheckers()
    { }

    [RelayCommand]
    private void ShowExporters()
    { }
  }
}
