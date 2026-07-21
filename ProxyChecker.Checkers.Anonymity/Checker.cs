using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Checkers;
using System.Net;

namespace ProxyChecker.Checkers.Anonymity
{
  internal class Checker : IChecker
  {
    private readonly ILogger<Checker> _logger;
    private CheckerSettings _settings = new CheckerSettings();

    public Checker(ILogger<Checker> logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool SupportsParallelChecking => true;

    public string Name { get; set; } = default!;

    public JToken? GetSettings()
    {
      return JToken.FromObject(_settings);
    }

    public Control? GetSettingsControl()
    {
      var viewModel = new CheckerSettingsControlViewModel
      {
        Timeout = _settings.Timeout,
      };

      return new CheckerSettingsControl(viewModel);
    }

    public JToken? GetSettingsFromControl(Control? control)
    {
      var settings = GetTypedSettingsFromControl(control);

      return settings is null ? null : JToken.FromObject(settings);
    }

    private CheckerSettings? GetTypedSettingsFromControl(Control? control)
    {
      if (control is not CheckerSettingsControl checkerSettingsControl)
      {
        return null;
      }

      if (checkerSettingsControl.DataContext is not CheckerSettingsControlViewModel viewModel)
      {
        return null;
      }

      var settings = new CheckerSettings
      {
        Timeout = viewModel.Timeout,
      };

      return settings;
    }

    public void SetSettings(JToken? settings)
    {
      _settings = settings is null
        ? new CheckerSettings()
        : settings.ToObject<CheckerSettings>()!;
    }

    public async Task<bool> CheckAsync(Proxy proxy, CancellationToken cancellationToken)
    {
      var proxyUri = proxy.GetUri();

      var webProxy = new WebProxy
      {
        Address = proxyUri
      };

      using var handler = new HttpClientHandler
      {
        Proxy = webProxy,
      };

      using var client = new HttpClient(handler)
      {
        Timeout = _settings.Timeout,
      };

      try
      {
        using var response = await client.GetAsync(@"https://ipinfo.io/ip", cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
        {
          return false;
        }

        var responseText = await response.Content.ReadAsStringAsync();

        return responseText == proxy.Host;
      }
      catch (Exception ex)
      {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
          _logger.LogError(ex, $"Error while checking proxy {proxyUri}");
        }
        else
        {
          _logger.LogError($"Error while checking proxy {proxyUri}: {ex.Message}");
        }
        return false;
      }
    }

    public Task<bool> IsReadyAsync(CancellationToken cancellationToken) => Task.FromResult(true);
  }
}
