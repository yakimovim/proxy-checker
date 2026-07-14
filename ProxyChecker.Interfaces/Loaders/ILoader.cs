using Avalonia.Controls;
using Newtonsoft.Json.Linq;

namespace ProxyChecker.Interfaces.Loaders
{
  public interface ILoader
  {
    string Name { get; set; }

    JToken? GetSettings();

    Control? GetSettingsControl();

    void SetSettings(JToken? settings);

    JToken? GetSettingsFromControl(Control? control);

    IAsyncEnumerable<Proxy> LoadAsync(CancellationToken cancellationToken);
  }
}
