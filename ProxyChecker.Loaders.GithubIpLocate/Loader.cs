using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Loaders;
using System.Net;
using System.Runtime.CompilerServices;

namespace ProxyChecker.Loaders.GithubIpLocate
{
  internal class Loader : ILoader
  {
    private LoaderSettings _settings = new LoaderSettings();
    private readonly ILogger<Loader> _logger;

    public Loader(ILogger<Loader> logger)
    {
      _logger = logger;
    }

    public string Name { get; set; } = string.Empty;

    public JToken GetSettings()
    {
      return JToken.FromObject(_settings);
    }

    public void SetSettings(JToken? settings)
    {
      _settings = settings?.ToObject<LoaderSettings>()!;
    }

    public async IAsyncEnumerable<Proxy> LoadAsync(
      [EnumeratorCancellation] CancellationToken cancellationToken)
    {
      var response = await GetApiResponse(cancellationToken);

      if (response is null)
      {
        yield break;
      }

      foreach(var line in response.Split([ '\n', '\r' ], StringSplitOptions.RemoveEmptyEntries))
      {
        if (line is null)
        {
          continue;
        }

        if (Uri.TryCreate(line, UriKind.Absolute, out var uri))
        {
          var proxy = new Proxy(
            uri.Scheme,
            uri.Host,
            uri.Port
          );

          if (!string.IsNullOrEmpty(_settings.Protocol))
          {
            if (string.Equals(_settings.Protocol, proxy.Scheme, StringComparison.InvariantCultureIgnoreCase))
            {
              yield return proxy;
            }
          }
          else
          {
            yield return proxy;
          }
        }
      }
    }

    private async Task<string?> GetApiResponse(CancellationToken cancellationToken)
    {
      using var handler = new HttpClientHandler();

      if (_settings.ProxyUri is not null)
      {
        handler.Proxy = new WebProxy
        {
          Address = _settings.ProxyUri,
        };
      }

      using var client = new HttpClient(handler)
      {
        Timeout = _settings.Timeout,
      };

      try
      {
        using var response = await client.GetAsync(GetGithubUri(), cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
        {
          return null;
        }

        return await response.Content.ReadAsStringAsync(cancellationToken);
      }
      catch (Exception ex)
      {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
          _logger.LogError(ex, "Error while loading list of proxies");
        }
        else
        {
          _logger.LogError($"Error while loading list of proxies: {ex.Message}");
        }

        return null;
      }
    }

    private Uri GetGithubUri()
    {
      return new Uri(@"https://raw.githubusercontent.com/iplocate/free-proxy-list/refs/heads/main/countries/US/proxies.txt");
      /*
      var uriBuilder = new UriBuilder("https://proxylist.geonode.com/api/proxy-list");

      var query = HttpUtility.ParseQueryString(string.Empty);
      if (!string.IsNullOrEmpty(_settings.Anonymity))
      {
        query["anonymityLevel"] = _settings.Anonymity;
      }
      if (_settings.Port.HasValue)
      {
        query["filterPort"] = _settings.Port.ToString();
      }
      if (!string.IsNullOrEmpty(_settings.Protocol))
      {
        query["protocols"] = _settings.Protocol;
      }
      if (_settings.Uptime.HasValue)
      {
        query["filterUpTime"] = _settings.Uptime.ToString();
      }
      if (_settings.LastChecked.HasValue)
      {
        query["filterLastChecked"] = _settings.LastChecked.ToString();
      }
      if (!string.IsNullOrEmpty(_settings.Speed))
      {
        query["speed"] = _settings.Speed;
      }
      query["limit"] = _settings.Limit.ToString();
      query["page"] = "1";

      uriBuilder.Query = query.ToString();

      return uriBuilder.Uri;
      */
    }

    public Control GetSettingsControl()
    {
      var viewModel = new LoaderSettingsControlViewModel
      {
        Country = _settings.Country,
        Protocol = _settings.Protocol ?? string.Empty,
        ProxyUri = _settings.ProxyUri,
        Timeout = _settings.Timeout,
      };

      return new LoaderSettingsControl(viewModel);
    }

    private LoaderSettings? GetTypedSettingsFromControl(Control? control)
    {
      if (control is not LoaderSettingsControl loaderSettingsControl)
      {
        return null;
      }

      if (loaderSettingsControl.DataContext is not LoaderSettingsControlViewModel viewModel)
      {
        return null;
      }

      var settings = new LoaderSettings
      {
        Country = viewModel.Country,
        Protocol = viewModel.Protocol,
        ProxyUri = viewModel.ProxyUri,
        Timeout = viewModel.Timeout,
      };

      return settings;
    }

    public JToken? GetSettingsFromControl(Control? control)
    {
      var settings = GetTypedSettingsFromControl(control);

      return settings is null ? null : JToken.FromObject(settings);
    }
  }
}
