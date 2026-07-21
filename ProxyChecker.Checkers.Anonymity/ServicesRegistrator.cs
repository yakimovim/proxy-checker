using Microsoft.Extensions.DependencyInjection;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Checkers;
using System.ComponentModel.Composition;

namespace ProxyChecker.Checkers.Anonymity
{
  [Export(typeof(IServicesRegistrator))]
  public class ServicesRegistrator : IServicesRegistrator
  {
    public void RegisterServices(IServiceCollection services)
    {
      services.AddTransient<ICheckerCreator, CheckerCreator>();
    }
  }
}
