namespace ProxyChecker.Interfaces
{
  public record Proxy(
    string Scheme,
    string Host,
    int Port,
    string? User = null,
    string? Password = null
  )
  { }
}
