using CommunityToolkit.Mvvm.ComponentModel;

namespace ProxyChecker.Loaders.FlashProxyApi;

internal partial class LoaderSettingsControlViewModel
  : ObservableValidator
{
  [ObservableProperty]
  private string? _protocol;

  [ObservableProperty]
  private string? _country;

  [ObservableProperty]
  private int? _speedMs;

  [ObservableProperty]
  private string? _anonymity;

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
