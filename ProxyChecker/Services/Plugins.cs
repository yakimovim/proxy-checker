using ProxyChecker.Interfaces.Loaders;
using System.Collections.Generic;

namespace ProxyChecker.Services
{
	internal static class Plugins
	{
		public static IEnumerable<ILoaderCreator> LoaderCreators { get; set; } = default!;
	}
}
