using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Checkers;
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
  internal partial class CheckersWindowViewModel : ViewModelBase, IRequireWindow
  {
    private readonly IWindowFactory _windowFactory;
    private readonly IEnumerable<ICheckerCreator> _checkerCreators;
    private readonly AppDbContext _db;

    [ObservableProperty]
    private ObservableCollection<NamedEntityViewModel> _checkers = new();

    public Window Window { get; set; } = default!;

    public CheckersWindowViewModel(
      IWindowFactory windowFactory,
      IEnumerable<ICheckerCreator> checkerCreators,
      AppDbContext db)
    {
      _windowFactory = windowFactory ?? throw new System.ArgumentNullException(nameof(windowFactory));
      _checkerCreators = checkerCreators;
      _db = db;

      var storedCheckers = _db.Checkers.ToArray();

      var appSettings = _db.Settings.Single();

      foreach (var checker in storedCheckers)
      {
        Checkers.Add(
          new NamedEntityViewModel(checker)
          {
            IsActive = (checker.Id == appSettings.CheckerId),
          }
        );
      }
    }

    [RelayCommand]
    private async Task Add(CancellationToken cancellationToken)
    {
      var dialog = _windowFactory.CreateCreateWindow<ICheckerCreator>();

      dialog.Title = Resource.CreateCheckerWindowTitle;

      var result = await dialog.ShowDialog<CreatorModel<ICheckerCreator>?>(Window);

      if (result is not null)
      {
        var checker = result.Creator.Create();

        Checker dbChecker = new()
        {
          Name = result.Name,
          CreatorUid = result.Creator.Uid,
          JsonSettings = checker.GetSettings()?.ToString(Formatting.None),
        };

        await _db.Checkers.AddAsync(dbChecker);

        await _db.SaveChangesAsync(cancellationToken);

        NamedEntityViewModel checkerViewModel = new(dbChecker);

        Checkers.Add(
          checkerViewModel
        );

        var appSettings = await _db.Settings.SingleAsync(cancellationToken);

        if (appSettings.CheckerId is null)
        {
          appSettings.CheckerId = dbChecker.Id;

          checkerViewModel.IsActive = true;

          await _db.SaveChangesAsync(cancellationToken);
        }
      }
    }

    [RelayCommand]
    private async Task DeleteAsync(NamedEntityViewModel checkerViewModel, CancellationToken cancellationToken)
    {
      Checker? checker = await _db.Checkers.FindAsync(checkerViewModel.Id);

      if (checker is not null)
      {
        _db.Checkers.Remove(checker);

        await _db.SaveChangesAsync(cancellationToken);

        var appSettings = await _db.Settings.SingleAsync(cancellationToken);

        if (appSettings.CheckerId == checkerViewModel.Id)
        {
          appSettings.CheckerId = (await _db.Checkers.FirstOrDefaultAsync())?.Id;

          await _db.SaveChangesAsync(cancellationToken);

          if (appSettings.CheckerId is not null)
          {
            Checkers.Single(c => c.Id == appSettings.CheckerId.Value).IsActive = true;
          }
        }

        Checkers.Remove(checkerViewModel);
      }
    }

    [RelayCommand]
    private async Task ShowSettings(NamedEntityViewModel checkerViewModel, CancellationToken cancellationToken)
    {
      var dbChecker = await _db.Checkers.FindAsync(checkerViewModel.Id, cancellationToken);

      if (dbChecker is null)
      {
        return;
      }

      var checkerCreator = _checkerCreators.SingleOrDefault(c => c.Uid == dbChecker.CreatorUid);

      if (checkerCreator is null)
      {
        return;
      }

      var checker = checkerCreator.Create();

      var settingsToken = string.IsNullOrEmpty(dbChecker.JsonSettings) ? null : JToken.Parse(dbChecker.JsonSettings);

      checker.SetSettings(settingsToken);

      var settingsControl = checker.GetSettingsControl();

      var viewModel = new NamedEntityWithSettingsViewModel
      {
        WindowTitle = Resource.CheckerSettingsWindowTitle,
        SettingsLabel = Resource.CheckerSettingsLabel,
        Name = checkerViewModel.Name,
        SettingsControl = settingsControl,
      };

      var dialog = new SettingsWindow(viewModel);

      if(await dialog.ShowDialog<bool>(Window))
      {
        settingsToken = checker.GetSettingsFromControl(settingsControl);

        dbChecker.Name = viewModel.Name;

        dbChecker.JsonSettings = settingsToken?.ToString(Formatting.None);

        _db.SaveChanges();

        checkerViewModel.Name = viewModel.Name;
      }
    }

    [RelayCommand]
    private async Task MakeActiveAsync(NamedEntityViewModel checkerViewModel, CancellationToken cancellationToken)
    {
      var appSettings = await _db.Settings.SingleAsync(cancellationToken);

      appSettings.CheckerId = checkerViewModel.Id;

      await _db.SaveChangesAsync(cancellationToken);

      foreach(var vm in Checkers)
      {
        vm.IsActive = vm.Id == checkerViewModel.Id;
      }
    }
  }
}
