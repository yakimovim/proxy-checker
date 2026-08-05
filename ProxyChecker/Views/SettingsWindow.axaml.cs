using System;
using Avalonia.Controls;
using ProxyChecker.ViewModels;

namespace ProxyChecker;

internal partial class SettingsWindow : Window
{
  public SettingsWindow(NamedEntityWithSettingsViewModel model)
  {
    DataContext = model;

    model.Window = this;
    InitializeComponent();
  }

  protected override void OnOpened(EventArgs e)
  {
    base.OnOpened(e);

    tbName.Focus();
  }
}