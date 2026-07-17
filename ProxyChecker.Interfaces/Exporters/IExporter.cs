namespace ProxyChecker.Interfaces.Exporters;

public interface IExporter : INamedEntityWithSettings
{
  Task ExportAsync(IEnumerable<Proxy> proxies, CancellationToken cancellationToken);
}
