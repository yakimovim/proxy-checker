namespace ProxyChecker.Interfaces.Checkers
{
  public interface IChecker : INamedEntityWithSettings
  {
    bool SupportsParallelChecking { get; }

    Task<bool> CheckAsync(Proxy proxy, CancellationToken cancellationToken);
  }
}
