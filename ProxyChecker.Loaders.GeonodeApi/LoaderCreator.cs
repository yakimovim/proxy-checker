using ProxyChecker.Interfaces.Loaders;

namespace ProxyChecker.Loaders.GeonodeApi
{
  internal class LoaderCreator : ILoaderCreator
  {
    public Guid Uid => new Guid("AAA097B7-2AAF-4C4B-B4C6-F0411F6C1AAF");

    public string Name => Resource.CreatorName;

    public string Description => Resource.CreatorDescription;

    public ILoader Create()
      => new Loader();
  }
}
