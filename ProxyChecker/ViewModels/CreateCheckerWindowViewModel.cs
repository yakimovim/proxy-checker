using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProxyChecker.Interfaces.Checkers;
using ProxyChecker.Interfaces.ViewModels;
using ProxyChecker.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProxyChecker.ViewModels
{
  internal partial class CreateCheckerWindowViewModel : ViewModelBase, IRequireWindow
	{
		public CreateCheckerWindowViewModel(IEnumerable<ICheckerCreator> checkerCreators)
		{
			CheckerCreators = checkerCreators;

			SelectedCheckerCreator = CheckerCreators.FirstOrDefault();
		}

		public IEnumerable<ICheckerCreator> CheckerCreators { get; }

		public Window Window { get; set; } = default!;

    [ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(OkCommand))]
		private string _name = string.Empty;

		[ObservableProperty]
		private ICheckerCreator? _selectedCheckerCreator;

		[RelayCommand(CanExecute = nameof(CanCreate))]
		private void Ok()
		{
			Window.Close(
			  new CreatorModel<ICheckerCreator>
			  {
				  Name = Name,
				  Creator = SelectedCheckerCreator!
			  }
			);
		}

		private bool CanCreate()
		  => !string.IsNullOrWhiteSpace(Name) && SelectedCheckerCreator is not null;

		[RelayCommand]
		private void Cancel()
		{
			Window.Close(null);
		}
	}
}
