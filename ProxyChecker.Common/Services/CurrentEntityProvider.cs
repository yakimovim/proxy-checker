using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using ProxyChecker.Common.Storage;
using ProxyChecker.Interfaces;
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

  private async Task<TEntity?> GetCurrentEntityInfoAsync<TEntity>(
    Func<Settings, int?> idProvider,
    CancellationToken cancellationToken
    )
    where TEntity : class, IPipelineEntity
  {
    var appSettings = await _db.Settings.AsNoTracking().SingleAsync(cancellationToken);

    var entityId = idProvider(appSettings);
    if (entityId is null)
    {
      return default;
    }

    return await _db.Set<TEntity>().AsNoTracking().SingleOrDefaultAsync(l => l.Id == entityId.Value, cancellationToken);
  }

  public async Task<Loader?> GetCurrentLoaderInfoAsync(
    CancellationToken cancellationToken
  )
    => await GetCurrentEntityInfoAsync<Loader>(s => s.LoaderId, cancellationToken);

  public async Task<Exporter?> GetCurrentExporterInfoAsync(
      CancellationToken cancellationToken
    )
    => await GetCurrentEntityInfoAsync<Exporter>(s => s.ExporterId, cancellationToken);

  public async Task<Checker?> GetCurrentCheckerInfoAsync(
    CancellationToken cancellationToken
  )
    => await GetCurrentEntityInfoAsync<Checker>(s => s.CheckerId, cancellationToken);

  private async Task<TEntityWithSettings?> GetCurrentEntityWithSettingsAsync<TEntityWithSettings, TStorageEntity>(
    Func<Settings, int?> idProvider,
    IEnumerable<ICreator<TEntityWithSettings>> creators,
    CancellationToken cancellationToken
    )
    where TStorageEntity : class, IPipelineEntity
    where TEntityWithSettings: IEntityWithSettings
  {
    var storageEntity = await GetCurrentEntityInfoAsync<TStorageEntity>(idProvider, cancellationToken);

    if (storageEntity is null)
    {
      return default;
    }

    var creator = creators.SingleOrDefault(c => c.Uid == storageEntity.CreatorUid);

    if (creator is null)
    {
      return default;
    }

    var entity = creator.Create();

    entity.SetSettings(storageEntity.JsonSettings is null ? null : JToken.Parse(storageEntity.JsonSettings));

    return entity;
  }

  public async Task<ILoader?> GetCurrentLoaderWithSettingsAsync(
    CancellationToken cancellationToken
  )
    => await GetCurrentEntityWithSettingsAsync<ILoader, Loader>(s => s.LoaderId, _loaderCreators, cancellationToken);

  public async Task<IExporter?> GetCurrentExporterWithSettingsAsync(
    CancellationToken cancellationToken
  )
    => await GetCurrentEntityWithSettingsAsync<IExporter, Exporter>(s => s.ExporterId, _exporterCreators, cancellationToken);

  public async Task<IChecker?> GetCurrentCheckerWithSettingsAsync(
    CancellationToken cancellationToken
  )
    => await GetCurrentEntityWithSettingsAsync<IChecker, Checker>(s => s.CheckerId, _checkerCreators, cancellationToken);
}
