using Newtonsoft.Json.Linq;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Loaders;
using System.Runtime.CompilerServices;

namespace ProxyChecker.Loaders.UriTextFile
{
  internal class Loader : ILoader
  {
    private LoaderSettings _settings = new();

    public string Name { get; set; } = string.Empty;

    public JToken GetSettings()
    {
      return JToken.FromObject(_settings);
    }

    public void SetSettings(JToken settings)
    {
      _settings = settings.ToObject<LoaderSettings>()!;
    }

    public async IAsyncEnumerable<Proxy> LoadAsync(
      [EnumeratorCancellation]CancellationToken cancellationToken)
    {
      using var reader = File.OpenText(_settings.FilePath);

      while(true)
      {
        var line = await reader.ReadLineAsync(cancellationToken);

        if (line is null)
        {
          break;
        }

        if (Uri.TryCreate(line, UriKind.Absolute, out var uri))
        {
          yield return new Proxy(
            uri.Scheme,
            uri.Host,
            uri.Port
          );
        }
      }
    }
  }
}
