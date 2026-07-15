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
  internal abstract partial class CreateWindowViewModel : ViewModelBase, IRequireWindow
	{
		public IEnumerable<ICreator> Creators { get; protected set; } = default!;

    public Window Window { get; set; } = default!;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private string _name = string.Empty;

    [ObservableProperty]
    private ICreator? _selectedCreator;

		[RelayCommand(CanExecute = nameof(CanCreate))]
		protected abstract void Ok();

    private bool CanCreate()
      => !string.IsNullOrWhiteSpace(Name) && SelectedCreator is not null;

    [RelayCommand]
    private void Cancel()
    {
      Window.Close(null);
    }
  }

  internal partial class CreateWindowViewModel<TCreator> : CreateWindowViewModel
    where TCreator : ICreator
	{
		public CreateWindowViewModel(IEnumerable<TCreator> creators)
		{
			Creators = creators.Cast<ICreator>();

			SelectedCreator = Creators.FirstOrDefault();
		}

    protected override void Ok()
    {
      Window.Close(
        new CreatorModel<TCreator>
        {
          Name = Name,
          Creator =(TCreator)SelectedCreator!
        }
      );
    }
	}
}
