using Microsoft.Extensions.DependencyInjection;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Loaders;
using System.ComponentModel.Composition;

namespace ProxyChecker.Loaders.UriTextFile
{
  [Export(typeof(IServicesRegistrator))]
  public class ServicesRegistrator : IServicesRegistrator
  {
    public void RegisterServices(IServiceCollection services)
    {
      services.AddTransient<ILoaderCreator, LoaderCreator>();
    }
  }
}
