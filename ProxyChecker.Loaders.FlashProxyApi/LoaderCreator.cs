using ProxyChecker.Interfaces.Loaders;

namespace ProxyChecker.Loaders.FlashProxyApi
{
  internal class LoaderCreator : ILoaderCreator
  {
    public Guid Uid => new Guid("0EFC10AE-1918-43EC-AED2-884D4E6B9A1E");

    public string Name => Resource.CreatorName;

    public string Description => Resource.CreatorDescription;

    public ILoader Create()
      => new Loader();
  }
}
