using Microsoft.Extensions.Logging;
using ProxyChecker.Interfaces.Loaders;

namespace ProxyChecker.Loaders.GithubIpLocate
{
  internal class LoaderCreator : ILoaderCreator
  {
    private readonly ILogger<Loader> _logger;

    public Guid Uid => new Guid("469D1C2B-94BA-4010-AF07-415584C05F07");

    public string Name => Resource.CreatorName;

    public string Description => Resource.CreatorDescription;

    public LoaderCreator(ILogger<Loader> logger)
    {
      _logger = logger;
    }

    public ILoader Create()
      => new Loader(_logger);
  }
}
