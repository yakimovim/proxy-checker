namespace ProxyChecker.Interfaces.Loaders
{
  public interface ILoader : INamedEntityWithSettings
  {
    IAsyncEnumerable<Proxy> LoadAsync(CancellationToken cancellationToken);
  }
}
