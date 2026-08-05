using Avalonia.Controls;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Loaders;
using System.Net;
using System.Runtime.CompilerServices;
using System.Web;

namespace ProxyChecker.Loaders.FlashProxyApi;

internal class Loader : LoaderBase<LoaderSettings>
{
  private readonly ILogger<Loader> _logger;

  public Loader(ILogger<Loader> logger)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  public override async IAsyncEnumerable<Proxy> LoadAsync(
    [EnumeratorCancellation] CancellationToken cancellationToken)
  {
    var responseText = await GetFlashProxyApiResponseText(cancellationToken);

    if (string.IsNullOrWhiteSpace(responseText))
    {
      yield break;
    }

    foreach (var line in responseText.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries))
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
      _logger.LogDebug(ex, "Error while loading list of proxies");

      return null;
    }
  }

  private Uri GetFlashProxyApiUri()
  {
    var uriBuilder = new UriBuilder("https://www.flashproxy.com/api/proxies/txt");

    var query = HttpUtility.ParseQueryString(string.Empty);
    query["limit"] = _settings.Limit.ToString();

    if (!string.IsNullOrEmpty(_settings.Protocol))
    {
      query["protocol"] = _settings.Protocol.ToLower();
    }

    if (!string.IsNullOrEmpty(_settings.Country))
    {
      query["country"] = _settings.Country.ToUpper();
    }

    if (_settings.SpeedMs.HasValue)
    {
      query["speed"] = _settings.SpeedMs.Value.ToString();
    }

    if (!string.IsNullOrEmpty(_settings.Anonymity))
    {
      query["anonymity"] = _settings.Anonymity.ToLower();
    }

    uriBuilder.Query = query.ToString();

    return uriBuilder.Uri;
  }

  public override Control GetSettingsControl()
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

  public override ValidationResult ValidateSettingsForCli()
  {
    return new LoaderSettingsValidator().Validate(_settings);
  }
}
