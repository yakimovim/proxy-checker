using Avalonia.Controls.ApplicationLifetimes;
using ProxyChecker.Interfaces;

namespace ProxyChecker.Services
{
  internal class DesktopService : IDesktopService
  {
    public IClassicDesktopStyleApplicationLifetime Desktop { get; set; } = default!;
  }
}
