using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ProxyChecker.Loaders.GithubIpLocate;

internal partial class LoaderSettingsControlViewModel
  : ObservableValidator
{
  private static readonly CodeWithName[] _countries;

  static LoaderSettingsControlViewModel()
  {
    var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

    var countryList = cultures
        .Select(culture => {
          try
          {
            return new RegionInfo(culture.Name);
          }
          catch
          {
            return null;
          }
        })
        .Where(region => region is not null)
        .GroupBy(region => region!.TwoLetterISORegionName) // Группируем по двубуквенному коду
        .Select(group => group.First()) // Оставляем по одному уникальному региону на код
        .OrderBy(region => region!.NativeName);

    _countries = new CodeWithName[] { new("", Resource.AnyOption) }
      .Concat(countryList.Select(r => new CodeWithName(r!.TwoLetterISORegionName.ToUpper(), $"{r.NativeName} ({r!.TwoLetterISORegionName.ToUpper()})")))
      .ToArray();
  }

  public CodeWithName[] Protocols { get; } = [
      new("", Resource.AnyOption),
      new("http", "HTTP"),
      new("https", "HTTPS"),
      new("socks4", "SOCKS4"),
      new("socks5", "SOCKS5"),
  ];

  public CodeWithName[] Countries { get; } = _countries;

  [ObservableProperty]
  private string? _protocol;

  [ObservableProperty]
  private string? _country;

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
