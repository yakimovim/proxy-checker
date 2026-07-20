using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Exporters;
using ProxyChecker.Interfaces.Resources;
using ProxyChecker.Interfaces.ViewModels;
using ProxyChecker.Models;
using ProxyChecker.Storage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyChecker.ViewModels
{
  internal partial class ExportersWindowViewModel : ViewModelBase, IRequireWindow
  {
    private readonly IWindowFactory _windowFactory;
    private readonly IEnumerable<IExporterCreator> _exporterCreators;
    private readonly AppDbContext _db;

    [ObservableProperty]
    private ObservableCollection<NamedEntityViewModel> _exporters = new();

    public Window Window { get; set; } = default!;

    public ExportersWindowViewModel(
      IWindowFactory windowFactory,
      IEnumerable<IExporterCreator> exporterCreators,
      AppDbContext db)
    {
      _windowFactory = windowFactory ?? throw new System.ArgumentNullException(nameof(windowFactory));
      _exporterCreators = exporterCreators;
      _db = db;

      var storedExporters = _db.Exporters.ToArray();

      var settings = _db.Settings.Single();

      foreach (var exporter in storedExporters)
      {
        Exporters.Add(
          new NamedEntityViewModel(exporter)
          {
            IsActive = (exporter.Id == settings.ExporterId),
          }
        );
      }
    }

    [RelayCommand]
    private async Task Add(CancellationToken cancellationToken)
    {
      var dialog = _windowFactory.CreateCreateWindow<IExporterCreator>();

      dialog.Title = Resource.CreateExporterWindowTitle;

      var result = await dialog.ShowDialog<CreatorModel<IExporterCreator>?>(Window);

      if (result is not null)
      {
        var exporter = result.Creator.Create();

        Exporter dbExporter = new()
        {
          Name = result.Name,
          CreatorUid = result.Creator.Uid,
          JsonSettings = exporter.GetSettings()?.ToString(Formatting.None),
        };

        await _db.Exporters.AddAsync(dbExporter);

        await _db.SaveChangesAsync(cancellationToken);

        NamedEntityViewModel exporterViewModel = new(dbExporter);

        Exporters.Add(
          exporterViewModel
        );

        var settings = _db.Settings.Single();

        if (settings.ExporterId is null)
        {
          settings.ExporterId = dbExporter.Id;

          exporterViewModel.IsActive = true;

          await _db.SaveChangesAsync(cancellationToken);
        }
      }
    }

    [RelayCommand]
    private async Task DeleteAsync(NamedEntityViewModel exporterViewModel, CancellationToken cancellationToken)
    {
      Exporter? exporter = await _db.Exporters.FindAsync(exporterViewModel.Id);

      if (exporter is not null)
      {
        _db.Exporters.Remove(exporter);

        await _db.SaveChangesAsync(cancellationToken);

        var appSettings = _db.Settings.Single();

        if (appSettings.ExporterId == exporterViewModel.Id)
        {
          appSettings.ExporterId = (await _db.Exporters.FirstOrDefaultAsync())?.Id;

          await _db.SaveChangesAsync(cancellationToken);

          if (appSettings.ExporterId is not null)
          {
            Exporters.Single(l => l.Id == appSettings.ExporterId.Value).IsActive = true;
          }
        }

        Exporters.Remove(exporterViewModel);
      }
    }

    [RelayCommand]
    private async Task ShowSettingsAsync(NamedEntityViewModel exporterViewModel, CancellationToken cancellationToken)
    {
      var dbExporter = await _db.Exporters.FindAsync(exporterViewModel.Id, cancellationToken);

      if (dbExporter is null)
      {
        return;
      }

      var exporterCreator = _exporterCreators.SingleOrDefault(c => c.Uid == dbExporter.CreatorUid);

      if (exporterCreator is null)
      {
        return;
      }

      var exporter = exporterCreator.Create();

      var settingsToken = string.IsNullOrEmpty(dbExporter.JsonSettings) ? null : JToken.Parse(dbExporter.JsonSettings);

      exporter.SetSettings(settingsToken);

      var settingsControl = exporter.GetSettingsControl();

      var viewModel = new NamedEntityWithSettingsViewModel
      {
        WindowTitle = Resource.ExporterSettingsWindowTitle,
        SettingsLabel = Resource.ExporterSettingsLabel,
        Name = exporterViewModel.Name,
        SettingsControl = settingsControl,
      };

      var dialog = new SettingsWindow(viewModel);

      if(await dialog.ShowDialog<bool>(Window))
      {
        settingsToken = exporter.GetSettingsFromControl(settingsControl);

        dbExporter.Name = viewModel.Name;

        dbExporter.JsonSettings = settingsToken?.ToString(Formatting.None);

        _db.SaveChanges();

        exporterViewModel.Name = viewModel.Name;
      }
    }

    [RelayCommand]
    private async Task MakeActiveAsync(NamedEntityViewModel exporterViewModel, CancellationToken cancellationToken)
    {
      var settings = _db.Settings.Single();

      settings.ExporterId = exporterViewModel.Id;

      await _db.SaveChangesAsync(cancellationToken);

      foreach(var vm in Exporters)
      {
        vm.IsActive = vm.Id == exporterViewModel.Id;
      }
    }
  }
}
