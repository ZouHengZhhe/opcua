// Copyright (c) 2014 Converter Systems LLC

using System.Windows;
using System.Windows.Media;

namespace ConverterSystems.Workstation
{
    internal static class FrameworkElementExtensions
    {
        // Methods
        public static FrameworkElement GetChildByName(this FrameworkElement parentVisual, string partName)
        {
            var childByName = parentVisual.FindName(partName) as FrameworkElement;
            if (childByName != null)
            {
                return childByName;
            }
            int childrenCount = VisualTreeHelper.GetChildrenCount(parentVisual);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parentVisual, i) as FrameworkElement;
                if (child != null)
                {
                    childByName = child.GetChildByName(partName);
                    if (childByName != null)
                    {
                        return childByName;
                    }
                }
            }
            return null;
        }

        internal static T GetFirstDescendantOfType<T>(this DependencyObject target) where T : DependencyObject
        {
            var local = target as T;
            if (local != null)
            {
                return local;
            }
            int childrenCount = VisualTreeHelper.GetChildrenCount(target);
            for (int i = 0; i < childrenCount; i++)
            {
                var firstDescendantOfType = VisualTreeHelper.GetChild(target, i).GetFirstDescendantOfType<T>();
                if (firstDescendantOfType != null)
                {
                    return firstDescendantOfType;
                }
            }
            return default(T);
        }
    }
}