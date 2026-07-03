using Newtonsoft.Json.Linq;

namespace ProxyChecker.Interfaces.Loaders
{
  public interface ILoader
  {
    string Name { get; set; }

    JToken GetSettings();

    void SetSettings(JToken settings);

    IAsyncEnumerable<Proxy> LoadAsync(CancellationToken cancellationToken);
  }
}
