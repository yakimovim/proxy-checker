using Microsoft.Extensions.DependencyInjection;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Exporters;
using System.ComponentModel.Composition;

namespace ProxyChecker.Exporters.UriTextFile
{
  [Export(typeof(IServicesRegistrator))]
  public class ServicesRegistrator : IServicesRegistrator
  {
    public void RegisterServices(IServiceCollection services)
    {
      services.AddTransient<IExporterCreator, ExporterCreator>();
    }
  }
}
