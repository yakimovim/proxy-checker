using Microsoft.Extensions.DependencyInjection;

namespace ProxyChecker.Common.Storage;

public static class StoragePreparer
{
	public static void PrepareStorage(IServiceProvider serviceProvider)
	{
		var db = serviceProvider.GetRequiredService<AppDbContext>();
		db.Database.EnsureCreated();

		if (!db.Settings.Any())
		{
			db.Settings.Add(new Settings());

			db.SaveChanges();
		}
	}
}
