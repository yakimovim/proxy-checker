using ProxyChecker.Interfaces.Loaders;

namespace ProxyChecker.Loaders.UriTextFile
{
  internal class LoaderCreator : ILoaderCreator
  {
    public Guid Uid => new Guid("51F64252-2868-4CB5-B32B-024A27742FC3");

    public string Name => "URI from Text file";

    public string Description => "This loader takes list of URI of proxies from a simple text file. Each line of the file contains one URI.";

    public ILoader Create()
      => new Loader();
  }
}
