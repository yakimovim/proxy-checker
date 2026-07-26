using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ProxyChecker.Common.Logging;

public static class LogConfigurator
{
	public static IServiceCollection ConfigureLogging(IServiceCollection services, IConfigurationRoot configuration)
	{
		services.AddLogging(loggingBuilder =>
		{
			loggingBuilder.AddSerilog(
			  new LoggerConfiguration()
				.ReadFrom.Configuration(configuration)
				.WriteTo.File("app.log")
				.CreateLogger()
			);
		});

		return services;
	}
}
