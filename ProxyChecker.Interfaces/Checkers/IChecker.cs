namespace ProxyChecker.Interfaces.Checkers
{
  public interface IChecker : IEntityWithSettings
  {
    bool SupportsParallelChecking { get; }

    Task<bool> IsReadyAsync(CancellationToken cancellationToken);

    Task<bool> CheckAsync(Proxy proxy, CancellationToken cancellationToken);
  }
}
