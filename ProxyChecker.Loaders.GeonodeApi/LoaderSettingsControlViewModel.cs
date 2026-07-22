using CommunityToolkit.Mvvm.ComponentModel;

namespace ProxyChecker.Loaders.GeonodeApi;

internal partial class LoaderSettingsControlViewModel
  : ObservableValidator
{
  [ObservableProperty]
  private int? _uptime;

  [ObservableProperty]
  private int? _lastChecked;

  [ObservableProperty]
  private int? _port;

  [ObservableProperty]
  private string? _protocol;

  [ObservableProperty]
  private string? _anonymity;

  [ObservableProperty]
  private string? _speed;

  [ObservableProperty]
  private int _limit;

  [ObservableProperty]
  private TimeSpan _timeout;

  public int TimeoutInSeconds
  {
    get => (int)Math.Floor(Timeout.TotalSeconds);
    set
    {
      Timeout = TimeSpan.FromSeconds(value);
      OnPropertyChanged(nameof(TimeoutInSeconds));
    }
  }

  [ObservableProperty]
  private Uri? _proxyUri;

  public string? ProxyUriString
  {
    get => ProxyUri?.ToString();
    set
    {
      ProxyUri = string.IsNullOrWhiteSpace(value) ? null : new Uri(value);
      OnPropertyChanged(nameof(ProxyUriString));
    }
  }
}
