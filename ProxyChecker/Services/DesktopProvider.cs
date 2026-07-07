using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ProxyChecker.Interfaces.Services;

namespace ProxyChecker.Services
{
  public class DesktopProvider : IDesktopProvider
  {
    public IClassicDesktopStyleApplicationLifetime? GetDesktop()
    {
      if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
      {
        return desktop;
      }

      return null;
    }
  }
}
