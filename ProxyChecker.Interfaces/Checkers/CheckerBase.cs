namespace ProxyChecker.Interfaces.Checkers;

public abstract class CheckerBase<TSettings> : EntityWithSettings<TSettings>, IChecker
  where TSettings : class, new()
{
  public virtual bool SupportsParallelChecking { get; protected set; }

  public abstract Task<bool> IsReadyAsync(CancellationToken cancellationToken);

  public abstract Task<bool> CheckAsync(Proxy proxy, CancellationToken cancellationToken);
}
