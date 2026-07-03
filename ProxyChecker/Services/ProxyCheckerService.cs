using ProxyChecker.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProxyChecker.Services
{
  internal class ProxyCheckerService
  {
    public async IAsyncEnumerable<Proxy> Check(IEnumerable<Proxy> proxies)
    {
      foreach (var proxy in proxies)
      {
        var isValid = await IsProxyValid(proxy);

        yield return proxy;
      }
    }

    private async Task<bool> IsProxyValid(Proxy proxy)
    {
      var webProxy = new WebProxy
      {
        Address = GetProxyUri(proxy)
      };

      // 2. Assign the proxy to the handler
      using var handler = new HttpClientHandler
      {
        Proxy = webProxy
      };

      
      // 3. Initialize HttpClient with the handler
      using var client = new HttpClient(handler);

      try
      {
        // 4. Send your request
        // string response = await client.GetStringAsync("https://ipify.org");
        var response = await client.GetAsync("https://google.com");

        return response.StatusCode == HttpStatusCode.OK;

      }
      catch (Exception)
      {
        return false;
      }
    }

    private Uri GetProxyUri(Proxy proxy)
    {
      var builder = new UriBuilder
      {
        Scheme = proxy.Scheme,
        Host = proxy.Host,
        Port = proxy.Port,
      };

      return builder.Uri;
    }
  }
}
