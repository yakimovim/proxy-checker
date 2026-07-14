using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
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

      Task.WaitAll(
        ReloadExistingLoadersAsync(CancellationToken.None),
        ReloadExistingCheckersAsync(CancellationToken.None)
      );
    }

    [ObservableProperty]
    private ObservableCollection<ProxyViewModel> _loadedProxies = new();

    [ObservableProperty]
    private ObservableCollection<ProxyViewModel> _validProxies = new();

    [ObservableProperty]
    private ObservableCollection<LoaderViewModel> _loaders = new();

    [ObservableProperty]
    private ObservableCollection<CheckerViewModel> _checkers = new();

    public Window Window { get; set; } = default!;

    [RelayCommand]
    private async Task LoadProxiesAsync(CancellationToken cancellationToken)
    {
      var appSettings = await _db.Settings.SingleAsync();

      if (appSettings.LoaderId is null)
      {
        await ShowLoadersAsync(cancellationToken);
        
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
    private async Task ShowLoadersAsync(CancellationToken cancellationToken)
    {
      var dialog = _windowFactory.CreateWindow<LoadersWindow>();

      await dialog.ShowDialog(Window);

      await ReloadExistingLoadersAsync(cancellationToken);
    }

    [RelayCommand]
    private async Task ShowCheckersAsync(CancellationToken cancellationToken)
    {
      var dialog = _windowFactory.CreateWindow<CheckersWindow>();

      await dialog.ShowDialog(Window);

      await ReloadExistingCheckersAsync(cancellationToken);
    }

    [RelayCommand]
    private void ShowExporters()
    { }

    private async Task ReloadExistingLoadersAsync(CancellationToken cancellationToken) 
    {
      var loaders = await _db.Loaders.AsNoTracking().ToListAsync(cancellationToken);

      var settings = await _db.Settings.AsNoTracking().SingleAsync(cancellationToken);

      Loaders.Clear();

      loaders.ForEach(l => {
        Loaders.Add(new LoaderViewModel(l)
        {
          IsActive = l.Id == settings.LoaderId
        });
      });
    }

    [RelayCommand]
    private async Task SetActiveLoaderAsync(
      LoaderViewModel loaderViewModel,
      CancellationToken cancellationToken)
    {
      var appSettings = await _db.Settings.SingleAsync(cancellationToken);

      appSettings.LoaderId = loaderViewModel.Id;

      await _db.SaveChangesAsync(cancellationToken);

      await ReloadExistingLoadersAsync(cancellationToken);
    }

    private async Task ReloadExistingCheckersAsync(CancellationToken cancellationToken)
    {
      var checkers = await _db.Checkers.AsNoTracking().ToListAsync(cancellationToken);

      var settings = await _db.Settings.AsNoTracking().SingleAsync(cancellationToken);

      Checkers.Clear();

      checkers.ForEach(c => {
        Checkers.Add(new CheckerViewModel(c)
        {
          IsActive = c.Id == settings.CheckerId
        });
      });
    }

    [RelayCommand]
    private async Task SetActiveCheckerAsync(
      CheckerViewModel checkerViewModel,
      CancellationToken cancellationToken)
    {
      var appSettings = await _db.Settings.SingleAsync(cancellationToken);

      appSettings.CheckerId = checkerViewModel.Id;

      await _db.SaveChangesAsync(cancellationToken);

      await ReloadExistingCheckersAsync(cancellationToken);
    }
  }
}
