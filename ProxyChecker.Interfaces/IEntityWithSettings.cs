using Avalonia.Controls;
using FluentValidation.Results;
using Newtonsoft.Json.Linq;

namespace ProxyChecker.Interfaces;

public interface IEntityWithSettings
{
  JToken? GetSettings();

  Control? GetSettingsControl();

  ValidationResult ValidateSettingsForCli();

  void SetSettings(JToken? settings);

  JToken? GetSettingsFromControl(Control? control);

}
