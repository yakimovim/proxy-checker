using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Checkers;
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
    private ObservableCollection<CheckerViewModel> _checkers = new();

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
          new CheckerViewModel(checker)
          {
            IsActive = (checker.Id == appSettings.LoaderId),
          }
        );
      }
    }

    [RelayCommand]
    private async Task Add(CancellationToken cancellationToken)
    {
      var dialog = _windowFactory.CreateWindow<CreateCheckerWindow>();

      var result = await dialog.ShowDialog<CheckerCreationModel?>(Window);

      if (result is not null)
      {
        var checker = result.CheckerCreator.Create();

        Checker dbChecker = new()
        {
          Name = result.Name,
          CreatorUid = result.CheckerCreator.Uid,
          JsonSettings = checker.GetSettings()?.ToString(Formatting.None),
        };

        await _db.Checkers.AddAsync(dbChecker);

        await _db.SaveChangesAsync(cancellationToken);

        CheckerViewModel checkerViewModel = new(dbChecker);

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
    private async Task DeleteAsync(CheckerViewModel checkerViewModel, CancellationToken cancellationToken)
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
        }

        Checkers.Remove(checkerViewModel);
      }
    }

    [RelayCommand]
    private async Task ShowSettings(CheckerViewModel checkerViewModel, CancellationToken cancellationToken)
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

      var viewModel = new CheckerSettingsWindowViewModel
      {
        Name = checkerViewModel.Name,
        SettingsControl = settingsControl,
      };

      var dialog = new CheckerSettingsWindow(viewModel);

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
    private async Task MakeActiveAsync(CheckerViewModel checkerViewModel, CancellationToken cancellationToken)
    {
      var appSettings = await _db.Settings.SingleAsync(cancellationToken);

      appSettings.LoaderId = checkerViewModel.Id;

      await _db.SaveChangesAsync(cancellationToken);

      foreach(var vm in Checkers)
      {
        vm.IsActive = vm.Id == checkerViewModel.Id;
      }
    }
  }
}
