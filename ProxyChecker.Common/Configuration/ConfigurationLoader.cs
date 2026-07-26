using Microsoft.Extensions.Configuration;

namespace ProxyChecker.Common.Configuration;

public static class ConfigurationLoader
{
	public static IConfigurationRoot LoadConfiguration()
	{
		return new ConfigurationBuilder()
		  .SetBasePath(Directory.GetCurrentDirectory())
		  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
		  .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
		  .Build();
	}
}
