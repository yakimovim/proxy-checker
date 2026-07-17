using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel.DataAnnotations;

namespace ProxyChecker.Exporters.UriTextFile
{
	internal partial class ExporterSettingsControlViewModel
	  : ObservableValidator
	{
		[ObservableProperty]
		[NotifyDataErrorInfo]
    [Required]
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
				Title = "Choose URI File",
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
