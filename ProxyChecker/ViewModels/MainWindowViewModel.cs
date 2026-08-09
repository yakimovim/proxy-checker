using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProxyChecker.Common.Models;
using ProxyChecker.Common.Services;
using ProxyChecker.Common.Storage;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Resources;
using ProxyChecker.Interfaces.ViewModels;

namespace ProxyChecker.ViewModels;

internal partial class MainWindowViewModel : ViewModelBase, IRequireWindow
{
  private readonly IDesktopService _desktopService;
  private readonly IWindowFactory _windowFactory;
  private readonly AppDbContext _db;
  private readonly CurrentEntityProvider _currentEntityProvider;

  private bool _processing = false;

  public MainWindowViewModel(
    IDesktopService desktopService,
    IWindowFactory windowFactory,
    AppDbContext db,
    CurrentEntityProvider currentEntityProvider
    )
  {
    _desktopService = desktopService;
    _windowFactory = windowFactory ?? throw new ArgumentNullException(nameof(windowFactory));
    _db = db ?? throw new ArgumentNullException(nameof(db));
    _currentEntityProvider = currentEntityProvider ?? throw new ArgumentNullException(nameof(currentEntityProvider));

    Task.WaitAll(
      ReloadExistingLoadersAsync(CancellationToken.None),
      ReloadExistingCheckersAsync(CancellationToken.None),
      ReloadExistingExportersAsync(CancellationToken.None)
    );
  }

  [ObservableProperty]
  private ObservableCollection<ProxyViewModel> _loadedProxies = new();

  [ObservableProperty]
  private ObservableCollection<ProxyViewModel> _validProxies = new();

  [ObservableProperty]
  private ObservableCollection<NamedEntityViewModel> _loaders = new();

  [ObservableProperty]
  private ObservableCollection<NamedEntityViewModel> _checkers = new();

  [ObservableProperty]
  private ObservableCollection<NamedEntityViewModel> _exporters = new();

  public Window Window { get; set; } = default!;

  [RelayCommand(CanExecute = nameof(CanLoadProxies))]
  private async Task LoadProxiesAsync(CancellationToken cancellationToken)
  {
    var appSettings = await _db.Settings.SingleAsync();

    if (appSettings.LoaderId is null)
    {
      await ShowLoadersAsync(cancellationToken);
      return;
    }

    var loader = await _currentEntityProvider.GetCurrentLoaderWithSettingsAsync(cancellationToken);

    if (loader is null)
    {
      return;
    }

    try
    {
      _processing = true;

      NotifyCommands();

      await foreach (var proxy in loader.LoadAsync(cancellationToken))
      {
        LoadedProxies.Add(
          new ProxyViewModel(
          proxy
          )
        );
      }
    }
    finally
    {
      _processing = false;

      NotifyCommands();
    }
  }

  private bool CanLoadProxies() => !_processing;

  private void NotifyCommands()
  {
    LoadProxiesCommand.NotifyCanExecuteChanged();
    ClearProxiesCommand.NotifyCanExecuteChanged();
    CheckProxiesCommand.NotifyCanExecuteChanged();
    ExportProxiesCommand.NotifyCanExecuteChanged();
  }

  [RelayCommand(CanExecute = nameof(CanClearProxies))]
  private void ClearProxies()
  {
    LoadedProxies.Clear();

    NotifyCommands();
  }

  private bool CanClearProxies() => !_processing && LoadedProxies.Any();

  private CancellationTokenSource? _proxyCheckingCancellationTokenSource;

  [RelayCommand(CanExecute = nameof(CanCheckProxies))]
  private async Task CheckProxiesAsync(CancellationToken cancellationToken)
  {
    var appSettings = await _db.Settings.SingleAsync(cancellationToken);

    if (appSettings.CheckerId is null)
    {
      await ShowCheckersAsync(cancellationToken);
      return;
    }

    var checker = await _currentEntityProvider.GetCurrentCheckerWithSettingsAsync(cancellationToken);

    if (checker is null)
    {
      return;
    }

    ValidProxies.Clear();

    if (!(await checker.IsReadyAsync(cancellationToken)))
    {
      return;
    }

    try
    {
      _processing = true;

      NotifyCommands();

      _proxyCheckingCancellationTokenSource = new CancellationTokenSource();
      cancellationToken.Register(() =>
      {
        _proxyCheckingCancellationTokenSource?.Cancel();
      });

      CancelProxyCheckingCommand.NotifyCanExecuteChanged();

      var loadedProxies = LoadedProxies.Select(pvm => pvm.ToProxy());

      if (checker.SupportsParallelChecking)
      {
        await Parallel.ForEachAsync(
          loadedProxies,
          _proxyCheckingCancellationTokenSource.Token,
          async (proxy, ct) =>
          {
            if (await checker.CheckAsync(proxy, ct))
            {
              ValidProxies.Add(
                new ProxyViewModel(proxy)
              );
            }
          }
        );
      }
      else
      {
        foreach (var proxy in loadedProxies)
        {
          if (_proxyCheckingCancellationTokenSource.Token.IsCancellationRequested)
          {
            break;
          }

          if (await checker.CheckAsync(proxy, cancellationToken))
          {
            ValidProxies.Add(
              new ProxyViewModel(proxy)
            );
          }
        }
      }
    }
    catch (TaskCanceledException) { }
    finally
    {
      _proxyCheckingCancellationTokenSource?.Dispose();
      _proxyCheckingCancellationTokenSource = null;

      _processing = false;

      NotifyCommands();

      CancelProxyCheckingCommand.NotifyCanExecuteChanged();
    }
  }

  [RelayCommand(CanExecute = nameof(CanCancelProxyChecking))]
  private async Task CancelProxyCheckingAsync()
  {
    _proxyCheckingCancellationTokenSource?.Cancel();
  }

  public bool CanCancelProxyChecking()
    => _proxyCheckingCancellationTokenSource != null;

  private bool CanCheckProxies() => !_processing && LoadedProxies.Any();

  [RelayCommand(CanExecute = nameof(CanExportProxies))]
  private async Task ExportProxiesAsync(CancellationToken cancellationToken)
  {
    var appSettings = await _db.Settings.SingleAsync(cancellationToken);

    if (appSettings.ExporterId is null)
    {
      await ShowExportersAsync(cancellationToken);
      return;
    }

    var exporter = await _currentEntityProvider.GetCurrentExporterWithSettingsAsync(cancellationToken);

    if (exporter is null)
    {
      return;
    }

    try
    {
      _processing = true;

      NotifyCommands();

      await exporter.ExportAsync(ValidProxies.Select(vm => vm.ToProxy()), cancellationToken);
    }
    finally
    {
      _processing = false;

      NotifyCommands();
    }

    var dialog = new MessageWindow(Resource.ExportFinishedMessage);

    await dialog.ShowDialog(Window);
  }

  private bool CanExportProxies() => !_processing && ValidProxies.Any();

  [RelayCommand]
  private void Exit()
  {
    _desktopService.Desktop.Shutdown();
  }

  [RelayCommand]
  private async Task ShowLoadersAsync(CancellationToken cancellationToken)
  {
    var dialog = _windowFactory.CreateWindow<LoadersWindow>();

    await dialog.ShowDialog(Window);

    await ReloadExistingLoadersAsync(cancellationToken);
  }

  [RelayCommand]
  private async Task ShowCheckersAsync(CancellationToken cancellationToken)
  {
    var dialog = _windowFactory.CreateWindow<CheckersWindow>();

    await dialog.ShowDialog(Window);

    await ReloadExistingCheckersAsync(cancellationToken);
  }

  [RelayCommand]
  private async Task ShowExportersAsync(CancellationToken cancellationToken)
  {
    var dialog = _windowFactory.CreateWindow<ExportersWindow>();

    await dialog.ShowDialog(Window);

    await ReloadExistingExportersAsync(cancellationToken);
  }

  private async Task ReloadExistingLoadersAsync(CancellationToken cancellationToken)
  {
    var loaders = await _db.Loaders.AsNoTracking().ToListAsync(cancellationToken);

    var appSettings = await _db.Settings.AsNoTracking().SingleAsync(cancellationToken);

    Loaders.Clear();

    loaders.ForEach(l =>
    {
      Loaders.Add(new NamedEntityViewModel(l)
      {
        IsActive = l.Id == appSettings.LoaderId
      });
    });
  }

  [RelayCommand]
  private async Task SetActiveLoaderAsync(
    NamedEntityViewModel loaderViewModel,
    CancellationToken cancellationToken)
  {
    var appSettings = await _db.Settings.SingleAsync(cancellationToken);

    appSettings.LoaderId = loaderViewModel.Id;

    await _db.SaveChangesAsync(cancellationToken);

    await ReloadExistingLoadersAsync(cancellationToken);
  }

  private async Task ReloadExistingCheckersAsync(CancellationToken cancellationToken)
  {
    var checkers = await _db.Checkers.AsNoTracking().ToListAsync(cancellationToken);

    var appSettings = await _db.Settings.AsNoTracking().SingleAsync(cancellationToken);

    Checkers.Clear();

    checkers.ForEach(c =>
    {
      Checkers.Add(new NamedEntityViewModel(c)
      {
        IsActive = c.Id == appSettings.CheckerId
      });
    });
  }

  [RelayCommand]
  private async Task SetActiveCheckerAsync(
    NamedEntityViewModel checkerViewModel,
    CancellationToken cancellationToken)
  {
    var appSettings = await _db.Settings.SingleAsync(cancellationToken);

    appSettings.CheckerId = checkerViewModel.Id;

    await _db.SaveChangesAsync(cancellationToken);

    await ReloadExistingCheckersAsync(cancellationToken);
  }

  private async Task ReloadExistingExportersAsync(CancellationToken cancellationToken)
  {
    var exporters = await _db.Exporters.AsNoTracking().ToListAsync(cancellationToken);

    var appSettings = await _db.Settings.AsNoTracking().SingleAsync(cancellationToken);

    Exporters.Clear();

    exporters.ForEach(e =>
    {
      Exporters.Add(new NamedEntityViewModel(e)
      {
        IsActive = e.Id == appSettings.ExporterId
      });
    });
  }

  [RelayCommand]
  private async Task SetActiveExporterAsync(
    NamedEntityViewModel exporterViewModel,
    CancellationToken cancellationToken)
  {
    var appSettings = await _db.Settings.SingleAsync(cancellationToken);

    appSettings.ExporterId = exporterViewModel.Id;

    await _db.SaveChangesAsync(cancellationToken);

    await ReloadExistingExportersAsync(cancellationToken);
  }

  [RelayCommand]
  private async Task ExportSettingsAsync(CancellationToken cancellationToken)
  {
    var pipelineModel = new PipelineModel();

    var loader = await _currentEntityProvider.GetCurrentLoaderWithSettingsAsync(cancellationToken);

    if (loader is null)
    {
      var messageDialog = new MessageWindow(Resource.NoLoadersMessage);
      await messageDialog.ShowDialog(Window);
      return;
    }

    var loaderValidationResult = loader.ValidateSettingsForCli();

    if (!loaderValidationResult.IsValid)
    {
      var messageDialog = new MessageWindow(
        GetSettingsNotReadyForCliMessage(
          Resource.NotReadyLoaderSettingsMessage,
          loaderValidationResult
        )
      );
      await messageDialog.ShowDialog(Window);
      return;
    }

    pipelineModel.LoaderCreatorUid = (await _currentEntityProvider.GetCurrentLoaderInfoAsync(cancellationToken))!.CreatorUid;
    pipelineModel.LoaderSettings = loader.GetSettings();

    var checker = await _currentEntityProvider.GetCurrentCheckerWithSettingsAsync(cancellationToken);

    if (checker is null)
    {
      var messageDialog = new MessageWindow(Resource.NoCheckersMessage);
      await messageDialog.ShowDialog(Window);
      return;
    }

    var checkerValidationResult = checker.ValidateSettingsForCli();

    if (!checkerValidationResult.IsValid)
    {
      var messageDialog = new MessageWindow(
        GetSettingsNotReadyForCliMessage(
          Resource.NotReadyCheckerSettingsMessage,
          checkerValidationResult
        )
      );
      await messageDialog.ShowDialog(Window);
      return;
    }

    pipelineModel.CheckerCreatorUid = (await _currentEntityProvider.GetCurrentCheckerInfoAsync(cancellationToken))!.CreatorUid;
    pipelineModel.CheckerSettings = checker.GetSettings();

    var exporter = await _currentEntityProvider.GetCurrentExporterWithSettingsAsync(cancellationToken);

    if (exporter is null)
    {
      var messageDialog = new MessageWindow(Resource.NoExportersMessage);
      await messageDialog.ShowDialog(Window);
      return;
    }

    var exporterValidationResult = exporter.ValidateSettingsForCli();

    if (!exporterValidationResult.IsValid)
    {
      var messageDialog = new MessageWindow(
        GetSettingsNotReadyForCliMessage(
          Resource.NotReadyExporterSettingsMessage,
          exporterValidationResult
        )
      );
      await messageDialog.ShowDialog(Window);
      return;
    }

    pipelineModel.ExporterCreatorUid = (await _currentEntityProvider.GetCurrentExporterInfoAsync(cancellationToken))!.CreatorUid;
    pipelineModel.ExporterSettings = exporter.GetSettings();

    var topLevel = TopLevel.GetTopLevel(_desktopService.Desktop.MainWindow);

    if (topLevel == null)
    {
      return;
    }

    var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
    {
      Title = Resource.SavePipelineSettingsTitle,
      ShowOverwritePrompt = true,
    });

    if (file is not null)
    {
      var path = file.TryGetLocalPath();

      if (!string.IsNullOrWhiteSpace(path))
      {
        await File.WriteAllTextAsync(
          path,
          JsonConvert.SerializeObject(pipelineModel, Formatting.Indented),
          cancellationToken
        );
      }
    }
  }

  private string GetSettingsNotReadyForCliMessage(string prefix, ValidationResult validationResult)
  {
    if (validationResult.IsValid)
    {
      throw new InvalidOperationException();
    }

    return prefix
      + Environment.NewLine
      + Environment.NewLine
      + string.Join(Environment.NewLine, validationResult.Errors.Select(e => $"- {e.ErrorMessage}"));
  }
}
