using CommunityToolkit.Mvvm.ComponentModel;
using ProxyChecker.Storage;

namespace ProxyChecker.ViewModels
{
  internal partial class CheckerViewModel : ViewModelBase
  {
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private bool _isActive = false;

    public CheckerViewModel(Checker loader)
    {
      Id = loader.Id;
      Name = loader.Name;
    }
  }
}
