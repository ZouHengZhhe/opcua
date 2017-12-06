// Copyright (c) 2014 Converter Systems LLC

using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace ConverterSystems.Workstation.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            SettingsFlyout.IsOpen = true;
        }

        private void OnBrowseBackExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var ue = e.OriginalSource as UIElement;
            if (ue != null)
            {
                var f = ue.ParentOfType<Flyout>();
                if (f != null)
                {
                    f.IsOpen = false;
                    e.Handled = true;
                }
            }
        }
    }
}