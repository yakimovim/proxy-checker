namespace ProxyChecker.Loaders.FlashProxyApi;

internal class LoaderSettings
{
	public string? Protocol { get; set; } = string.Empty;
	public string? Country { get; set; } = string.Empty;
	public int? SpeedMs { get; set; }
	public string? Anonymity { get; set; } = string.Empty;
	public int Limit { get; set; } = 10;
	public Uri? ProxyUri { get; set; }
	public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
}
