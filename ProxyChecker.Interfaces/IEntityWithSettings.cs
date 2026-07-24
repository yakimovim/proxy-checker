using Avalonia.Controls;
using Newtonsoft.Json.Linq;

namespace ProxyChecker.Interfaces
{
  public interface IEntityWithSettings
  {
    JToken? GetSettings();

    Control? GetSettingsControl();

    void SetSettings(JToken? settings);

    JToken? GetSettingsFromControl(Control? control);
  }
}
