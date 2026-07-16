using Avalonia.Controls;
using Newtonsoft.Json.Linq;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Checkers;
using ProxyChecker.Interfaces.ViewModels;
using System.Net;

namespace ProxyChecker.Checkers.OkResponse
{
  internal class Checker : IChecker
  {
    private static readonly Random _rnd = new Random((int)DateTime.Now.Ticks);
    private readonly IDesktopService _desktopService;
    private CheckerSettings _settings = new CheckerSettings();
    private CheckerSettings? _currentSettings = null;

    public Checker(IDesktopService desktopService)
    {
      _desktopService = desktopService ?? throw new ArgumentNullException(nameof(desktopService));
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

      foreach (var uri in _settings.TargetUris)
      {
        viewModel.TargetUris.Add(uri);
      }

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
        TargetUris = [.. viewModel.TargetUris],
      };

      return settings;
    }

    public void SetSettings(JToken? settings)
    {
      _settings =  settings is null 
        ? new CheckerSettings() 
        : settings.ToObject<CheckerSettings>()!;
    }

    public async Task<bool> CheckAsync(Proxy proxy, CancellationToken cancellationToken)
    {
      if (_currentSettings is null)
      {
        return false;
      }

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
        Timeout = _currentSettings.Timeout,
      };

      try
      {
        using var response = await client.GetAsync(GetRandomTargetUri(_currentSettings.TargetUris), cancellationToken);

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

    public async Task<bool> IsReadyAsync(CancellationToken cancellationToken)
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
}
