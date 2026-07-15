using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProxyChecker.Interfaces.ViewModels;

namespace ProxyChecker.ViewModels
{
  internal partial class NamedEntityWithSettingsViewModel : ViewModelBase, IRequireWindow
  {
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private Control? _settingsControl;

    public string? WindowTitle { get; set; }

    public string? SettingsLabel { get; set; }

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
