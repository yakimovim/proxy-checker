using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.ViewModels;
using ProxyChecker.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProxyChecker.ViewModels
{
  internal partial class CreateWindowViewModel<TCreator> : ViewModelBase, IRequireWindow
		where TCreator : ICreator
	{
		public CreateWindowViewModel(IEnumerable<TCreator> checkerCreators)
		{
			Creators = checkerCreators;

			SelectedCreator = Creators.FirstOrDefault();
		}

		public IEnumerable<TCreator> Creators { get; }

		public Window Window { get; set; } = default!;

    [ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(OkCommand))]
		private string _name = string.Empty;

		[ObservableProperty]
		private TCreator? _selectedCreator;

		[RelayCommand(CanExecute = nameof(CanCreate))]
		private void Ok()
		{
			Window.Close(
			  new CreatorModel<TCreator>
			  {
				  Name = Name,
				  Creator = SelectedCreator!
			  }
			);
		}

		private bool CanCreate()
		  => !string.IsNullOrWhiteSpace(Name) && SelectedCreator is not null;

		[RelayCommand]
		private void Cancel()
		{
			Window.Close(null);
		}
	}
}
