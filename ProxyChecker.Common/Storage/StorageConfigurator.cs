using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ProxyChecker.Common.Storage;

public static class StorageConfigurator
{
	public static IServiceCollection ConfigureStorage(IServiceCollection services)
	{
		services.AddDbContext<AppDbContext>(options =>
		{
			options.UseSqlite("Data Source=app.db");
		});

		return services;
	}
}
