namespace ProxyChecker.Interfaces.Loaders;

public abstract class LoaderBase<TSettings> : EntityWithSettings<TSettings>, ILoader
  where TSettings : class, new()
{
  public abstract IAsyncEnumerable<Proxy> LoadAsync(CancellationToken cancellationToken);
}
