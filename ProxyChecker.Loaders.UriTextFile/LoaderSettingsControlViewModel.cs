using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ProxyChecker.Loaders.UriTextFile
{
	internal partial class LoaderSettingsControlViewModel
	  : ObservableObject
	{
		[ObservableProperty]
		private string? _filePath;

		public Control Control { get; set; } = default!;

		[RelayCommand]
		private async Task SelectFilePathAsync(CancellationToken cancellationToken)
		{
			var topLevel = TopLevel.GetTopLevel(Control);

			if (topLevel == null) 
			{
				return;
			}

			var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
			{
				Title = "Open URI File",
				AllowMultiple = false,
			});

			if (files.Count > 0)
			{
				var path = files[0].TryGetLocalPath();

				if (!string.IsNullOrWhiteSpace(path)) 
				{
					FilePath = path;
				}
			}
		}
	}
}
