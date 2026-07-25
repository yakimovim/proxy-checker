using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ProxyChecker.Loaders.FlashProxyApi;

internal partial class LoaderSettingsControlViewModel
  : ObservableValidator
{
	private static readonly CodeWithName[] _countries;

	static LoaderSettingsControlViewModel()
	{
		var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

		var countryList = cultures
			.Select(culture =>
			{
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

	public CodeWithName[] AnonymityLevels { get; } = [
		new("", Resource.AnyOption),
		new("elite", "Elite"),
		new("anonymous", "Anonymous"),
		new("transparent", "Transparent"),
	];

	public CodeWithName[] Countries { get; } = _countries;

	[ObservableProperty]
	private string? _protocol = string.Empty;

	[ObservableProperty]
	private string? _country = string.Empty;

	[ObservableProperty]
	[Range(1, 1000, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = nameof(Resource.RangeErrorMessage))]
	private int? _speedMs;

	[ObservableProperty]
	private string? _anonymity = string.Empty;

	[ObservableProperty]
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
			ProxyUri = string.IsNullOrWhiteSpace(value) ? null : new Uri(value);
			OnPropertyChanged(nameof(ProxyUriString));
		}
	}
}
