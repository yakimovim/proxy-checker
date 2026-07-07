using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProxyChecker.Interfaces.Services;

namespace ProxyChecker.ViewModels
{
  internal partial class LoaderViewModel : ViewModelBase
  {
    public LoaderViewModel(IDesktopProvider desktopProvider) 
      : base(desktopProvider)
    {
    }

    [ObservableProperty]
    private string _name = string.Empty;

    [RelayCommand]
    private void Delete()
    {

    }

    [RelayCommand]
    private void ShowSettings()
    {

    }

    [RelayCommand]
    private void MakeActive()
    {

    }
  }
}
