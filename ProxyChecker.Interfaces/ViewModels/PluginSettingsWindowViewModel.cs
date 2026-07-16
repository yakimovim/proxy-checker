using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ProxyChecker.Interfaces.ViewModels
{
  public partial class PluginSettingsWindowViewModel : ViewModelBase, IRequireWindow
  {
    [ObservableProperty]
    private Control? _settingsControl;

    public Window Window { get; set; } = default!;

    [RelayCommand]
    private void Ok()
    {
      Window.Close(true);
    }

    [RelayCommand]
    private void Cancel()
    {
      Window.Close(false);
    }
  }
}
