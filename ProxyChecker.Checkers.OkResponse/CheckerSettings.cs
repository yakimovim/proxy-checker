namespace ProxyChecker.Checkers.OkResponse
{
  internal class CheckerSettings
  {
    public Uri[] TargetUris { get; set; } = [];

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
  }
}
