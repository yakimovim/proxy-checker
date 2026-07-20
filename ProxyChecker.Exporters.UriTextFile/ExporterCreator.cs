using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Exporters;

namespace ProxyChecker.Exporters.UriTextFile
{
  internal class ExporterCreator : IExporterCreator
  {
    private readonly IDesktopService _desktopService;

    public ExporterCreator(IDesktopService desktopService)
    {
      _desktopService = desktopService;
    }

    public Guid Uid => new Guid("BCF1B10B-6C55-4D78-9E29-8DE1C725FA76");

    public string Name => Resource.CreatorName;

    public string Description => Resource.CreatorDescription;

    public IExporter Create()
      => new Exporter(_desktopService);
  }
}
