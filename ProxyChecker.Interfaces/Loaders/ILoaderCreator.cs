namespace ProxyChecker.Interfaces.Loaders
{
  public interface ILoaderCreator
  {
    Guid Uid { get; }

    string Name { get; }

    string Description { get; }

    ILoader Create();
  }
}
