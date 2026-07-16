using CommunityToolkit.Mvvm.ComponentModel;
using ProxyChecker.Interfaces.ViewModels;
using ProxyChecker.Storage;

namespace ProxyChecker.ViewModels
{
  internal partial class NamedEntityViewModel : ViewModelBase
  {
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private bool _isActive = false;

    public NamedEntityViewModel(INamedEntity namedEntity)
    {
      Id = namedEntity.Id;
      Name = namedEntity.Name;
    }
  }
}
