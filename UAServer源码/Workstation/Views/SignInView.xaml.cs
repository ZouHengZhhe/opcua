// Copyright (c) 2014 Converter Systems LLC

using System.Windows;
using System.Windows.Input;

namespace ConverterSystems.Workstation.Views
{
    /// <summary>
    /// Interaction logic for SignInView.xaml
    /// </summary>
    public partial class SignInView
    {
        public SignInView()
        {
            InitializeComponent();
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            NavigationCommands.BrowseBack.Execute(null, this);
        }
    }
}