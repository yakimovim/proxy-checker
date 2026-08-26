using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProxyChecker.Common.Services;

namespace ProxyChecker.Common.Storage;

public static class StorageConfigurator
{
	public static IServiceCollection ConfigureStorage(IServiceCollection services)
	{
    var storagePath = Path.Combine(PathsProvider.GetStorageFolder(), "app.db");

		services.AddDbContext<AppDbContext>(options =>
		{
			options.UseSqlite(@$"Data Source=""{storagePath}""");
		});

		return services;
	}
}
