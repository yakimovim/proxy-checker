using Newtonsoft.Json;

namespace ProxyChecker.Loaders.GeonodeApi
{
  internal class ResponseModel
  {
    public ResponseProxyModel[] Data { get; set; } = default!;
  }

  internal class ResponseProxyModel
  {
    public string Ip { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string[] Protocols { get; set; } = default!;
  }
}
