using Microsoft.Extensions.Logging;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Checkers;

namespace ProxyChecker.Checkers.OkResponse
{
  internal class CheckerCreator : ICheckerCreator
  {
    private readonly IDesktopService _desktopService;
    private readonly ILogger<Checker> _logger;

    public CheckerCreator(
      IDesktopService desktopService,
      ILogger<Checker> logger
    )
    {
      _desktopService = desktopService;
      _logger = logger;
    }

    public Guid Uid => new Guid("93BB003E-10A2-4124-A9A6-8340118CB2D4");

    public string Name => Resource.CreatorName;

    public string Description => Resource.CreatorDescription;

    public IChecker Create()
      => new Checker(_desktopService, _logger);
  }
}
