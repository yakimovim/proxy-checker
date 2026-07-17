namespace ProxyChecker.Interfaces
{
  public record Proxy(
    string Scheme,
    string Host,
    int Port,
    string? User = null,
    string? Password = null
  )
  {
    public Uri GetUri()
    {
      var builder = new UriBuilder
      {
        Scheme = Scheme,
        Host = Host,
        Port = Port,
        UserName = User,
        Password = Password,
      };

      return builder.Uri;
    }
  }
}
