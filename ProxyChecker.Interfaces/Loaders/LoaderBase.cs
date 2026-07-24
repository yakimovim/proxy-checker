using Avalonia.Controls;
using Newtonsoft.Json.Linq;

namespace ProxyChecker.Interfaces.Loaders;

public abstract class LoaderBase<TSettings> : ILoader
  where TSettings : class, new()
{
  protected TSettings _settings = new();

  public string Name { get; set; } = string.Empty;

  public JToken? GetSettings()
  {
    return JToken.FromObject(_settings!);
  }

  public abstract Control? GetSettingsControl();

  public JToken? GetSettingsFromControl(Control? control)
  {
    var settings = GetTypedSettingsFromControl(control);

    return settings is null ? null : JToken.FromObject(settings);
  }

  protected abstract TSettings? GetTypedSettingsFromControl(Control? control);

  public abstract IAsyncEnumerable<Proxy> LoadAsync(CancellationToken cancellationToken);

  public void SetSettings(JToken? settings)
  {
    _settings = settings?.ToObject<TSettings>()!;
  }
}
