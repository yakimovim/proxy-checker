using Microsoft.Extensions.DependencyInjection;

namespace ProxyChecker.Interfaces
{
  public interface IServicesRegistrator
  {
    void RegisterServices(IServiceCollection services);
  }
}
