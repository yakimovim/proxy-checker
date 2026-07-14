using Avalonia.Controls;
using Newtonsoft.Json.Linq;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Checkers;
using System.Net;

namespace ProxyChecker.Checkers.OkResponse
{
  internal class Checker : IChecker
  {
    private static readonly Random _rnd = new Random((int)DateTime.Now.Ticks);

    private CheckerSettings _settings = new CheckerSettings();

    public bool SupportsParallelChecking => true;

    public string Name { get; set; } = default!;

    public JToken? GetSettings()
    {
      return JToken.FromObject(_settings);
    }

    public Control? GetSettingsControl()
    {
      throw new NotImplementedException();
    }

    public JToken? GetSettingsFromControl(Control? control)
    {
      throw new NotImplementedException();
    }

    public void SetSettings(JToken? settings)
    {
      _settings = settings?.ToObject<CheckerSettings>()!;
    }

    public async Task<bool> CheckAsync(Proxy proxy, CancellationToken cancellationToken)
    {
      var webProxy = new WebProxy
      {
        Address = GetProxyUri(proxy)
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
        using var response = await client.GetAsync(GetRandomTargetUri(_settings.TargetUris), cancellationToken);

        return response.StatusCode == HttpStatusCode.OK;
      }
      catch (Exception)
      {
        return false;
      }
    }

    private Uri GetRandomTargetUri(Uri[] targetUris)
    {
      return targetUris[_rnd.Next(targetUris.Length)];
    }

    private Uri GetProxyUri(Proxy proxy)
    {
      var builder = new UriBuilder
      {
        Scheme = proxy.Scheme,
        Host = proxy.Host,
        Port = proxy.Port,
      };

      return builder.Uri;
    }
  }
}
