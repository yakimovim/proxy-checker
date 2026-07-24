namespace ProxyChecker.Interfaces.Exporters;

public interface IExporter : IEntityWithSettings
{
  Task ExportAsync(IEnumerable<Proxy> proxies, CancellationToken cancellationToken);
}
