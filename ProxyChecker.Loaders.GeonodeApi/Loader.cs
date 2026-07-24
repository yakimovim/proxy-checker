using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Loaders;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Web;

namespace ProxyChecker.Loaders.GeonodeApi;

internal class Loader : LoaderBase<LoaderSettings>
{
  private readonly ILogger<Loader> _logger;

  public Loader(ILogger<Loader> logger)
  {
    _logger = logger;
  }

  public override async IAsyncEnumerable<Proxy> LoadAsync(
    [EnumeratorCancellation] CancellationToken cancellationToken)
  {
    var response = await GetApiResponse(cancellationToken);

    if (response is null || response.Data is null)
    {
      yield break;
    }

    foreach (var proxyModel in response.Data)
    {
      if (proxyModel is null)
      {
        continue;
      }

      foreach (var protocol in proxyModel.Protocols)
      {
        if (protocol is null)
        {
          continue;
        }

        var line = $"{protocol}://{proxyModel.Ip}:{proxyModel.Port}";

        if (Uri.TryCreate(line, UriKind.Absolute, out var uri))
        {
          yield return new Proxy(
            uri.Scheme,
            uri.Host,
            uri.Port
          );
        }
      }
    }
  }

  private async Task<ResponseModel?> GetApiResponse(CancellationToken cancellationToken)
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
      using var response = await client.GetAsync(GetGeonodeApiUri(), cancellationToken);

      if (response.StatusCode != HttpStatusCode.OK)
      {
        return null;
      }

      return await response.Content.ReadFromJsonAsync<ResponseModel>(cancellationToken);
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

  private Uri GetGeonodeApiUri()
  {
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
  }

  public override Control GetSettingsControl()
  {
    var viewModel = new LoaderSettingsControlViewModel
    {
      Uptime = _settings.Uptime,
      LastChecked = _settings.LastChecked,
      Port = _settings.Port,
      Protocol = _settings.Protocol ?? string.Empty,
      Speed = _settings.Speed ?? string.Empty,
      Anonymity = _settings.Anonymity ?? string.Empty,
      Limit = _settings.Limit,
      ProxyUri = _settings.ProxyUri,
      Timeout = _settings.Timeout,
    };

    return new LoaderSettingsControl(viewModel);
  }

  protected override LoaderSettings? GetTypedSettingsFromControl(Control? control)
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
      Uptime = viewModel.Uptime,
      LastChecked = viewModel.LastChecked,
      Port = viewModel.Port,
      Protocol = viewModel.Protocol,
      Speed = viewModel.Speed,
      Anonymity = viewModel.Anonymity,
      Limit = viewModel.Limit,
      ProxyUri = viewModel.ProxyUri,
      Timeout = viewModel.Timeout,
    };

    return settings;
  }
}
