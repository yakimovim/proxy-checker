using Microsoft.Extensions.Logging;
using ProxyChecker.Interfaces.Checkers;

namespace ProxyChecker.Checkers.Anonymity
{
  internal class CheckerCreator : ICheckerCreator
  {
    private readonly ILogger<Checker> _logger;

    public CheckerCreator(
      ILogger<Checker> logger
    )
    {
      _logger = logger;
    }

    public Guid Uid => new Guid("781837FD-C596-4662-92D6-10214EAE7031");

    public string Name => Resource.CreatorName;

    public string Description => Resource.CreatorDescription;

    public IChecker Create()
      => new Checker(_logger);
  }
}
