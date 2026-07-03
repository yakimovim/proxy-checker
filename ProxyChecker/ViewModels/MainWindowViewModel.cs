using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    private readonly ProxyCheckerService _proxyCheckerService;

    public MainWindowViewModel(ProxyCheckerService proxyCheckerService)
    {
      _proxyCheckerService = proxyCheckerService;
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
      Desktop?.Shutdown();
    }

    [RelayCommand]
    private void ShowLoaders()
    {
      if (Desktop?.MainWindow is not null)
      {
        var dialog = new LoadersWindow();

        dialog.ShowDialog(Desktop?.MainWindow!);
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
