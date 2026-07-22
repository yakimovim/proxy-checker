using Avalonia.Controls;
using Newtonsoft.Json.Linq;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Loaders;
using System.Net;
using System.Runtime.CompilerServices;
using System.Web;

namespace ProxyChecker.Loaders.FlashProxyApi
{
  internal class Loader : ILoader
  {
    private LoaderSettings _settings = new LoaderSettings();

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
      var responseText = await GetFlashProxyApiResponseText(cancellationToken);

      if (string.IsNullOrWhiteSpace(responseText))
      {
        yield break;
      }

      foreach (var line in responseText.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
      {
        if (line is null)
        {
          break;
        }

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

    private async Task<string?> GetFlashProxyApiResponseText(CancellationToken cancellationToken)
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
        using var response = await client.GetAsync(GetFlashProxyApiUri(), cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
        {
          return null;
        }

        return await response.Content.ReadAsStringAsync(cancellationToken);
      }
      catch (Exception ex)
      {
        return null;
      }
    }

    private Uri GetFlashProxyApiUri()
    {
      var uriBuilder = new UriBuilder("https://www.flashproxy.com/api/proxies/txt");

      var query = HttpUtility.ParseQueryString(string.Empty);
      query["limit"] = _settings.Limit.ToString();

      uriBuilder.Query = query.ToString();

      return uriBuilder.Uri;
    }

    public Control GetSettingsControl()
    {
      var viewModel = new LoaderSettingsControlViewModel
      {
        Protocol = _settings.Protocol,
        Country = _settings.Country,
        SpeedMs = _settings.SpeedMs,
        Anonymity = _settings.Anonymity,
        Limit = _settings.Limit,
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
        Protocol = viewModel.Protocol,
        Country = viewModel.Country,
        SpeedMs = viewModel.SpeedMs,
        Anonymity = viewModel.Anonymity,
        Limit = viewModel.Limit,
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
