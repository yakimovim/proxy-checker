namespace ProxyChecker.Loaders.FlashProxyApi;

internal class LoaderSettings
{
  public string? Protocol { get; set; }
  public string? Country { get; set; }
  public int? SpeedMs { get; set; }
  public string? Anonymity { get; set; }
  public int Limit { get; set; } = 10;
  public Uri? ProxyUri { get; set; }
  public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
}
