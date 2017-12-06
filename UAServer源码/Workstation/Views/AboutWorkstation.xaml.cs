// Copyright (c) 2014 Converter Systems LLC

using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace ConverterSystems.Workstation.Views
{
    /// <summary>
    /// Interaction logic for AboutBox.xaml
    /// </summary>
    public partial class AboutWorkstation
    {
        public AboutWorkstation(MetroWindow owningWindow, MetroDialogSettings settings) : base(owningWindow, settings)
        {
            InitializeComponent();
        }

        private async void OnClick(object sender, RoutedEventArgs e)
        {
            await OwningWindow.HideMetroDialogAsync(this);
        }
    }
}