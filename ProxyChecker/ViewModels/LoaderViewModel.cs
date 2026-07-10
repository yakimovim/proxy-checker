using CommunityToolkit.Mvvm.ComponentModel;
using ProxyChecker.Storage;

namespace ProxyChecker.ViewModels
{
  internal partial class LoaderViewModel : ViewModelBase
  {
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private bool _isActive = false;

    public LoaderViewModel(Loader loader)
    {
      Id = loader.Id;
      Name = loader.Name;
    }
  }
}
