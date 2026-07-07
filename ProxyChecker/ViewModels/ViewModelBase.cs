using CommunityToolkit.Mvvm.ComponentModel;
using ProxyChecker.Interfaces.Services;

namespace ProxyChecker.ViewModels
{
  internal abstract class ViewModelBase : ObservableObject
  {
    protected readonly IDesktopProvider _desktopProvider;

    protected ViewModelBase(IDesktopProvider desktopProvider)
    {
      _desktopProvider = desktopProvider ?? throw new System.ArgumentNullException(nameof(desktopProvider));
    }
  }
}
