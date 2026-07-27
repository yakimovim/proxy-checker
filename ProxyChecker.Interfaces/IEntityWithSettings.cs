using Avalonia.Controls;
using Newtonsoft.Json.Linq;

namespace ProxyChecker.Interfaces;

public interface IEntityWithSettings
{
  JToken? GetSettings();

  Control? GetSettingsControl();

  bool CheckSettingsAreReadyForCli();

  void SetSettings(JToken? settings);

  JToken? GetSettingsFromControl(Control? control);

}
