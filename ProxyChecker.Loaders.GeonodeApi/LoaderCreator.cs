using Microsoft.Extensions.Logging;
using ProxyChecker.Interfaces.Loaders;

namespace ProxyChecker.Loaders.GeonodeApi
{
  internal class LoaderCreator : ILoaderCreator
  {
    private readonly ILogger<Loader> _logger;

    public Guid Uid => new Guid("AAA097B7-2AAF-4C4B-B4C6-F0411F6C1AAF");

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
