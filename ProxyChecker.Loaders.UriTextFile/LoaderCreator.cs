using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Loaders;

namespace ProxyChecker.Loaders.UriTextFile
{
  internal class LoaderCreator : ILoaderCreator
  {
    private readonly IDesktopService _desktopService;

    public LoaderCreator(IDesktopService desktopService)
    {
      _desktopService = desktopService;
    }

    public Guid Uid => new Guid("51F64252-2868-4CB5-B32B-024A27742FC3");

    public string Name => Resource.CreatorName;

    public string Description => Resource.CreatorDescription;

    public ILoader Create()
      => new Loader(_desktopService);
  }
}
