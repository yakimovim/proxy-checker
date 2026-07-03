using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProxyChecker.ViewModels
{
  internal abstract class ViewModelBase : ObservableObject
  {
    protected IClassicDesktopStyleApplicationLifetime? Desktop
    {
      get
      {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
          return desktop;
        }

        return null;
      }
    }
  }
}
