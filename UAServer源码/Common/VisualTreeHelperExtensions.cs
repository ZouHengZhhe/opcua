// Copyright (c) 2014 Converter Systems LLC

using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ConverterSystems.Workstation
{
    internal static class VisualTreeHelperExtensions
    {
        public static T FindAncestor<T>(DependencyObject dependencyObject) where T : class
        {
            var target = dependencyObject;
            do
            {
                target = VisualTreeHelper.GetParent(target);
            } while (target != null && !(target is T));
            return target as T;
        }

        // Methods

        internal static List<T> GetChildren<T>(this DependencyObject parent) where T : FrameworkElement
        {
            var list = new List<T>();
            if (parent == null)
            {
                return null;
            }
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child.GetType() == typeof (T))
                {
                    list.Add((T) child);
                }
                if (VisualTreeHelper.GetChildrenCount(child) > 0)
                {
                    list.AddRange(child.GetChildren<T>());
                }
            }
            return list;
        }

        internal static T GetParent<T>(this DependencyObject child) where T : FrameworkElement
        {
            return (child as UIElement).ParentOfType<T>();
        }


        /// <summary>
        /// Tries to locate a given item within the visual tree,
        /// starting with the dependency object at a given position. 
        /// </summary>
        /// <typeparam name="T">The type of the element to be found
        /// on the visual tree of the element at the given location.</typeparam>
        /// <param name="reference">The main element which is used to perform
        /// hit testing.</param>
        /// <param name="point">The position to be evaluated on the origin.</param>
        public static T TryFindFromPoint<T>(UIElement reference, Point point) where T : FrameworkElement
        {
            var element = reference.InputHitTest(point) as DependencyObject;
            if (element == null)
            {
                return null;
            }
            if (element is T)
            {
                return (T) element;
            }
            return element.GetParent<T>();
        }
    }
}