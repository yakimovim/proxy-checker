namespace ProxyChecker.Checkers.Anonymity
{
  internal class CheckerSettings
  {
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
  }
}
