using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ProxyChecker.ViewModels
{
  internal partial class LoaderViewModel : ViewModelBase
  {
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
