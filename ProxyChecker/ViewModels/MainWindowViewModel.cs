using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProxyChecker.Factories;
using ProxyChecker.Interfaces;
using ProxyChecker.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyChecker.ViewModels
{
  internal partial class MainWindowViewModel : ViewModelBase
  {
    private readonly LoadersWindowFactory _loadersWindowFactory;
    private readonly ProxyCheckerService _proxyCheckerService;

    public MainWindowViewModel(
      LoadersWindowFactory loadersWindowFactory,
      ProxyCheckerService proxyCheckerService)
    {
      _loadersWindowFactory = loadersWindowFactory ?? throw new System.ArgumentNullException(nameof(loadersWindowFactory));
      _proxyCheckerService = proxyCheckerService ?? throw new System.ArgumentNullException(nameof(proxyCheckerService));
    }

    [ObservableProperty]
    private ObservableCollection<ProxyViewModel> _loadedProxies = new();

    [ObservableProperty]
    private ObservableCollection<ProxyViewModel> _validProxies = new();

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
      MainApplication.Desktop.Shutdown();
    }

    [RelayCommand]
    private void ShowLoaders()
    {
      var mainWindow = MainApplication.MainWindow;

      if (mainWindow is not null)
      {
        var dialog = _loadersWindowFactory.CreateLoadersWindow();

        dialog.ShowDialog(mainWindow!);
      }
    }

    [RelayCommand]
    private void ShowCheckers()
    { }

    [RelayCommand]
    private void ShowExporters()
    { }
  }
}
