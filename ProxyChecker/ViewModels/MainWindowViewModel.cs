using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.ViewModels;
using ProxyChecker.Services;
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
    private readonly ProxyCheckerService _proxyCheckerService;

    public MainWindowViewModel(
      IDesktopService desktopService,
      IWindowFactory windowFactory,
      ProxyCheckerService proxyCheckerService
      )
    {
      _desktopService = desktopService;
      _windowFactory = windowFactory ?? throw new System.ArgumentNullException(nameof(windowFactory));
      _proxyCheckerService = proxyCheckerService ?? throw new System.ArgumentNullException(nameof(proxyCheckerService));
    }

    [ObservableProperty]
    private ObservableCollection<ProxyViewModel> _loadedProxies = new();

    [ObservableProperty]
    private ObservableCollection<ProxyViewModel> _validProxies = new();

    public Window Window { get; set; } = default!;

    [RelayCommand]
    private void LoadProxies()
    {
      LoadedProxies.Add(
        new ProxyViewModel(
          new Proxy("http", "72.56.238.99", 1080)
        )
      );

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

    [RelayCommand(CanExecute = nameof(CanCheckProxies))]
    private async Task CheckProxiesAsync(CancellationToken cancellationToken)
    {
      ValidProxies.Clear();

      await foreach (var proxy in _proxyCheckerService.Check(LoadedProxies.Select(pvm => pvm.ToProxy())))
      {
        ValidProxies.Add(
          new ProxyViewModel(proxy)
        );
      }

      ExportProxiesCommand.NotifyCanExecuteChanged();
    }

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
