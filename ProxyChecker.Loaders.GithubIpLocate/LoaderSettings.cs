namespace ProxyChecker.Loaders.GithubIpLocate;

internal class LoaderSettings
{
  public string? Protocol { get; set; } = string.Empty;
  public string? Country { get; set; } = string.Empty;
  public Uri? ProxyUri { get; set; }
  public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
}
