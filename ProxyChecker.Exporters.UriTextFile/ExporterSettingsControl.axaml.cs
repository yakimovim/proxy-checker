using Avalonia.Controls;

namespace ProxyChecker.Exporters.UriTextFile;

internal partial class ExporterSettingsControl : UserControl
{
  public ExporterSettingsControl(ExporterSettingsControlViewModel viewModel)
  {
    DataContext = viewModel;

    viewModel.Control = this;

    InitializeComponent();
  }
}