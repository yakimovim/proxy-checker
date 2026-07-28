using Avalonia.Controls.ApplicationLifetimes;
using ProxyChecker.Interfaces;

namespace ProxyChecker.Cli.Services;

internal class DesktopServiceStub : IDesktopService
{
  public IClassicDesktopStyleApplicationLifetime Desktop 
    => throw new NotSupportedException(Resource.NoDesktopService);
}
