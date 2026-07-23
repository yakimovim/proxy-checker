using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
  [NotifyDataErrorInfo]
  [Range(1, 100, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.RangeErrorMessage))]
  private int? _uptime;

  [ObservableProperty]
  [NotifyDataErrorInfo]
  [Range(1, 60, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.RangeErrorMessage))]
  private int? _lastChecked;

  [ObservableProperty]
  [NotifyDataErrorInfo]
  [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.GreaterErrorMessage))]
  private int? _port;

  [ObservableProperty]
  private string? _protocol;

  [ObservableProperty]
  private string? _anonymity;

  [ObservableProperty]
  private string? _speed;

  [ObservableProperty]
  [NotifyDataErrorInfo]
  [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.RequiredErrorMessage))]
  [Range(1, 500, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.RangeErrorMessage))]
  private int _limit;

  [ObservableProperty]
  private TimeSpan _timeout;

  [Range(1, 600, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.RangeErrorMessage))]
  public int TimeoutInSeconds
  {
    get => (int)Math.Floor(Timeout.TotalSeconds);
    set
    {
      ValidateProperty(value, nameof(TimeoutInSeconds));

      Timeout = TimeSpan.FromSeconds(value);
      OnPropertyChanged(nameof(TimeoutInSeconds));
    }
  }

  [ObservableProperty]
  private Uri? _proxyUri;

  [IsUri]
  public string? ProxyUriString
  {
    get => ProxyUri?.ToString();
    set
    {
      ValidateProperty(value, nameof(ProxyUriString));

      if (string.IsNullOrEmpty(value) || Uri.TryCreate(value, UriKind.Absolute, out _))
      {
        ProxyUri = string.IsNullOrWhiteSpace(value) ? null : new Uri(value);
        OnPropertyChanged(nameof(ProxyUriString));
      }
    }
  }
}
