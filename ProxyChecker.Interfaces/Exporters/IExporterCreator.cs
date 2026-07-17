namespace ProxyChecker.Interfaces.Exporters;

public interface IExporterCreator : ICreator
{
  IExporter Create();
}
