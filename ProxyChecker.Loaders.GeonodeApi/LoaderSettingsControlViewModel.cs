using CommunityToolkit.Mvvm.ComponentModel;

namespace ProxyChecker.Loaders.GeonodeApi;

internal partial class LoaderSettingsControlViewModel
  : ObservableValidator
{
  public CodeWithName[] Protocols { get; } = [
      new("", Resource.AnyOption),
      new("http", "HTTP"),
      new("https", "HTTPS"),
      new("socks4", "SOCKS4"),
      new("socks5", "SOCKS5"),
  ];

  public CodeWithName[] AnonymityLevels { get; } = [
      new("", Resource.AnyOption),
      new("elite", "Elite"),
      new("anonymous", "Anonymous"),
      new("transparent", "Transparent"),
  ];

  public CodeWithName[] Speeds { get; } = [
      new("", Resource.AnyOption),
      new("fast", Resource.FastSpeedOption),
      new("medium", Resource.MediumSpeedOption),
      new("slow", Resource.SlowSpeedOption),
  ];

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
      if (string.IsNullOrEmpty(value) || Uri.TryCreate(value, UriKind.Absolute, out _))
      {
        ProxyUri = string.IsNullOrWhiteSpace(value) ? null : new Uri(value);
        OnPropertyChanged(nameof(ProxyUriString));
      }
    }
  }
}
