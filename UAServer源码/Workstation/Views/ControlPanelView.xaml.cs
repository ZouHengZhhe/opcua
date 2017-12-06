// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Windows;

namespace ConverterSystems.Workstation.Views
{
    /// <summary>
    /// Interaction logic for ControlPanelView.xaml
    /// </summary>
    public sealed partial class ControlPanelView
    {
        public ControlPanelView()
        {
            InitializeComponent();
        }

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            // New! Get/Set subscription property values in code.
            // First, cast subscription to variable of type dynamic.
            // Note: dynamic does not support Intellisense

            var subscription = DataContext as dynamic;
            subscription.Robot1Mode = (Int16) 0; // Mind the Type!
        }

        private async void MultiplyButton_OnClick(object sender, RoutedEventArgs e)
        {
            // New! call ua methods in code.
            // First, cast subscription to variable of type dynamic.
            // Note: dynamic does not support Intellisense
            // Call the method using the methodname and pass the input arguments as an object[]
            // Returns the output arguments as a Task<object[]>
            // To decode the output arguments you can:
            // a) call task.Result(), however this blocks the ui thread
            // b) add keyword 'async' to the calling method, and add keyword 'await' to the method call.

            var dyn = (dynamic) DataContext;
            try
            {
                var a = double.Parse(InputA.Text);
                var b = double.Parse(InputB.Text);
                object[] outputArgs = await dyn.Robot1MultiplyMethod(new object[] { a, b });
                var result = (double) outputArgs[0];
                Result.Text = result.ToString("G");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            InputA.Text = "0";
            InputB.Text = "0";
            Result.Text = "0";
        }
    }
}