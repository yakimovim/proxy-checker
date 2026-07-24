using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Checkers;
using System.Net;

namespace ProxyChecker.Checkers.Anonymity;

internal class Checker : CheckerBase<CheckerSettings>
{
  private readonly ILogger<Checker> _logger;

  public Checker(ILogger<Checker> logger)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    SupportsParallelChecking = true;
  }

  public override Control? GetSettingsControl()
  {
    var viewModel = new CheckerSettingsControlViewModel
    {
      Timeout = _settings.Timeout,
    };

    return new CheckerSettingsControl(viewModel);
  }

  protected override CheckerSettings? GetTypedSettingsFromControl(Control? control)
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

  public override async Task<bool> CheckAsync(Proxy proxy, CancellationToken cancellationToken)
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

      var proxyAddresses = await GetProxyIps(proxy.Host);

      return proxyAddresses.Any(ip => responseText == ip.ToString());
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

  private async Task<IPAddress[]> GetProxyIps(string host)
  {
    if (IPAddress.TryParse(host, out var ip))
    {
      return [ip];
    }

    return await Dns.GetHostAddressesAsync(host);
  }

  public override Task<bool> IsReadyAsync(CancellationToken cancellationToken) => Task.FromResult(true);
}
