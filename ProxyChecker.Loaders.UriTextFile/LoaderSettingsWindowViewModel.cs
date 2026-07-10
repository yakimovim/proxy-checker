using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ProxyChecker.Loaders.UriTextFile
{
  internal partial class LoaderSettingsWindowViewModel
    : ObservableObject
  {
    public Window Window { get; set; } = default!;

    [ObservableProperty]
    private Control? _settingsControl;

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
