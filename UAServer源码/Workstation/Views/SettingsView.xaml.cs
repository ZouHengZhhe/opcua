// Copyright (c) 2014 Converter Systems LLC

using System.Windows;
using MahApps.Metro.Controls.Dialogs;

namespace ConverterSystems.Workstation.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void ShowSignIn(object sender, RoutedEventArgs e)
        {
            var w = this.ParentOfType<MainWindow>();
            if (w != null)
            {
                w.SettingsFlyout.IsOpen = false;
                w.SignInFlyout.IsOpen = true;
            }
        }

        private async void ShowAboutBox(object sender, RoutedEventArgs e)
        {
            var w = this.ParentOfType<MainWindow>();
            if (w != null)
            {
                var mySettings = new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Accented };
                var d = new AboutWorkstation(w, mySettings);
                await w.ShowMetroDialogAsync(d);
            }
        }
    }
}