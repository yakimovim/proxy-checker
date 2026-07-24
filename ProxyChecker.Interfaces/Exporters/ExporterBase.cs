namespace ProxyChecker.Interfaces.Exporters;

public abstract class ExporterBase<TSettings> : EntityWithSettings<TSettings>, IExporter
  where TSettings : class, new()
{
  public abstract Task ExportAsync(IEnumerable<Proxy> proxies, CancellationToken cancellationToken);
}
