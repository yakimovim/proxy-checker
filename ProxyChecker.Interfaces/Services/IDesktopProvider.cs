using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace ProxyChecker.Interfaces.Services
{
  public interface IDesktopProvider
  {
    IClassicDesktopStyleApplicationLifetime? GetDesktop();
  }

  public static class DesktopProviderExtensions
  {
    extension(IDesktopProvider desktopProvider)
    {
      public Window? MainWindow => desktopProvider.GetDesktop()?.MainWindow;
    }
  }
}