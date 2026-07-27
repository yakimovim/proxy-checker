using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using ProxyChecker.Common.Storage;
using ProxyChecker.Interfaces.Checkers;
using ProxyChecker.Interfaces.Exporters;
using ProxyChecker.Interfaces.Loaders;

namespace ProxyChecker.Common.Services;

public class CurrentEntityProvider
{
	private readonly AppDbContext _db;
	private readonly IEnumerable<ILoaderCreator> _loaderCreators;
	private readonly IEnumerable<ICheckerCreator> _checkerCreators;
	private readonly IEnumerable<IExporterCreator> _exporterCreators;

	public CurrentEntityProvider(
		AppDbContext db,
		IEnumerable<ILoaderCreator> loaderCreators,
		IEnumerable<ICheckerCreator> checkerCreators,
		IEnumerable<IExporterCreator> exporterCreators
	)
	{
		_db = db ?? throw new ArgumentNullException(nameof(db));
		_loaderCreators = loaderCreators ?? throw new ArgumentNullException(nameof(loaderCreators));
		_checkerCreators = checkerCreators ?? throw new ArgumentNullException(nameof(checkerCreators));
		_exporterCreators = exporterCreators ?? throw new ArgumentNullException(nameof(exporterCreators));
	}

  public async Task<Loader?> GetCurrentLoaderInfoAsync(
    CancellationToken cancellationToken
  )
  {
    var appSettings = await _db.Settings.AsNoTracking().SingleAsync(cancellationToken);

    if (appSettings.LoaderId is null)
    {
      return null;
    }

    var dbLoader = await _db.Loaders.SingleOrDefaultAsync(l => l.Id == appSettings.LoaderId.Value, cancellationToken);

		return dbLoader;
  }

  public async Task<ILoader?> GetCurrentLoaderWithSettingsAsync(
		CancellationToken cancellationToken
	)
	{
		var dbLoader = await GetCurrentLoaderInfoAsync(cancellationToken);

		if (dbLoader is null)
		{
			return null;
		}

		var loaderCreator = _loaderCreators.SingleOrDefault(c => c.Uid == dbLoader.CreatorUid);

		if (loaderCreator is null)
		{
			return null;
		}

		var loader = loaderCreator.Create();

		loader.SetSettings(dbLoader.JsonSettings is null ? null : JToken.Parse(dbLoader.JsonSettings));

		return loader;
	}

  public async Task<Exporter?> GetCurrentExporterInfoAsync(
      CancellationToken cancellationToken
    )
  {
    var appSettings = await _db.Settings.AsNoTracking().SingleAsync(cancellationToken);

    if (appSettings.ExporterId is null)
    {
      return null;
    }

    var dbExporter = await _db.Exporters.SingleOrDefaultAsync(e => e.Id == appSettings.ExporterId.Value, cancellationToken);

		return dbExporter;
  }

  public async Task<IExporter?> GetCurrentExporterWithSettingsAsync(
		CancellationToken cancellationToken
	)
	{
		var dbExporter = await GetCurrentExporterInfoAsync(cancellationToken);

		if (dbExporter is null)
		{
			return null;
		}

		var exporterCreator = _exporterCreators.SingleOrDefault(c => c.Uid == dbExporter.CreatorUid);

		if (exporterCreator is null)
		{
			return null;
		}

		var exporter = exporterCreator.Create();

		exporter.SetSettings(dbExporter.JsonSettings is null ? null : JToken.Parse(dbExporter.JsonSettings));

		return exporter;
	}
  public async Task<Checker?> GetCurrentCheckerInfoAsync(
    CancellationToken cancellationToken
  )
  {
    var appSettings = await _db.Settings.AsNoTracking().SingleAsync(cancellationToken);

    if (appSettings.CheckerId is null)
    {
      return null;
    }

    var dbChecker = await _db.Checkers.SingleOrDefaultAsync(c => c.Id == appSettings.CheckerId.Value, cancellationToken);

		return dbChecker;
  }

  public async Task<IChecker?> GetCurrentCheckerWithSettingsAsync(
		CancellationToken cancellationToken
	)
	{
		var dbChecker = await GetCurrentCheckerInfoAsync(cancellationToken);

		if (dbChecker is null)
		{
			return null;
		}

		var checkerCreator = _checkerCreators.SingleOrDefault(c => c.Uid == dbChecker.CreatorUid);

		if (checkerCreator is null)
		{
			return null;
		}

		var checker = checkerCreator.Create();

		checker.SetSettings(dbChecker.JsonSettings is null ? null : JToken.Parse(dbChecker.JsonSettings));

		return checker;
	}
}
