namespace ProxyChecker.Interfaces.Loaders
{
  public interface ILoader : IEntityWithSettings
  {
    IAsyncEnumerable<Proxy> LoadAsync(CancellationToken cancellationToken);
  }
}
