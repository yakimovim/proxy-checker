using ProxyChecker.Interfaces.Loaders;

namespace ProxyChecker.Models
{
  internal class LoaderCreationModel
  {
    public string Name { get; init; } = default!;

    public ILoaderCreator LoaderCreator { get; init; } = default!;
  }
}
