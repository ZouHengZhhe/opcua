// Copyright (c) 2014 Converter Systems LLC

using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ConverterSystems.Workstation.Behaviors
{
    public class DisableMouseWheelAndArrowKeysBehavior : Behavior<UIElement>
    {
        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Left || e.Key == Key.Right)
            {
                e.Handled = true;
            }
        }

        protected override void OnAttached()
        {
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
            AssociatedObject.PreviewMouseWheel += OnPreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
            AssociatedObject.PreviewMouseWheel -= OnPreviewMouseWheel;
        }
    }
}