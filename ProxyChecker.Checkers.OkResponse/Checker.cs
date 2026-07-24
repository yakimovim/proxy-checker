using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Checkers;
using ProxyChecker.Interfaces.ViewModels;
using System.Net;

namespace ProxyChecker.Checkers.OkResponse;

internal class Checker : CheckerBase<CheckerSettings>
{
  private static readonly Random _rnd = new Random((int)DateTime.Now.Ticks);
  private readonly IDesktopService _desktopService;
  private readonly ILogger<Checker> _logger;
  private CheckerSettings? _currentSettings = null;

  public Checker(IDesktopService desktopService, ILogger<Checker> logger)
  {
    _desktopService = desktopService ?? throw new ArgumentNullException(nameof(desktopService));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    SupportsParallelChecking = true;
  }

  public override Control? GetSettingsControl()
  {
    var viewModel = new CheckerSettingsControlViewModel
    {
      Timeout = _settings.Timeout,
    };

    foreach (var uri in _settings.TargetUris)
    {
      viewModel.TargetUris.Add(uri);
    }

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
      TargetUris = [.. viewModel.TargetUris],
    };

    return settings;
  }

  public override async Task<bool> CheckAsync(Proxy proxy, CancellationToken cancellationToken)
  {
    if (_currentSettings is null)
    {
      return false;
    }

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
      Timeout = _currentSettings.Timeout,
    };

    try
    {
      using var response = await client.GetAsync(GetRandomTargetUri(_currentSettings.TargetUris), cancellationToken);

      return response.StatusCode == HttpStatusCode.OK;
    }
    catch (Exception ex)
    {
      if(_logger.IsEnabled(LogLevel.Debug))
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

  private Uri GetRandomTargetUri(Uri[] targetUris)
  {
    return targetUris[_rnd.Next(targetUris.Length)];
  }

  public override async Task<bool> IsReadyAsync(CancellationToken cancellationToken)
  {
    _currentSettings = _settings;

    if (_currentSettings.TargetUris.Length == 0)
    {
      var control = GetSettingsControl();

      var viewModel = new PluginSettingsWindowViewModel
      {
        SettingsControl = control,
      };

      var dialog = new PluginSettingsWindow(viewModel)
      {
        Title = Resource.PluginSettingsWindowTitle,
      };

      if (await dialog.ShowDialog<bool>(_desktopService.Desktop.MainWindow!))
      {
        _currentSettings = GetTypedSettingsFromControl(viewModel.SettingsControl);
      }
      else
      {
        return false;
      }
    }

    if (_currentSettings is null || _currentSettings?.TargetUris.Length == 0)
    {
      var dialog = new MessageWindow(Resource.NoTargetUriMessage);

      await dialog.ShowDialog(_desktopService.Desktop.MainWindow!);

      return false;
    }

    return true;
  }
}
