using Avalonia.Controls.ApplicationLifetimes;

namespace ProxyChecker.Interfaces
{
  public interface IDesktopService
  {
    IClassicDesktopStyleApplicationLifetime Desktop { get; }
  }
}
