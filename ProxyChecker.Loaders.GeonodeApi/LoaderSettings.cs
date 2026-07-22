namespace ProxyChecker.Loaders.GeonodeApi;

internal class LoaderSettings
{
  public int? Uptime { get; set; }
  public int? LastChecked { get; set; }
  public int? Port { get; set; }
  public string? Protocol { get; set; }
  public string? Anonymity { get; set; }
  public string? Speed { get; set; }
  public int Limit { get; set; } = 10;
  public Uri? ProxyUri { get; set; }
  public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
}
