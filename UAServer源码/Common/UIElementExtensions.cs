// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ConverterSystems.Workstation
{
    public static class UIElementExtensions
    {
        // Methods
        public static IList<T> ChildrenOfType<T>(this UIElement element) where T : UIElement
        {
            var list = new List<T>();
            if (element != null)
            {
                element.ChildrenOfType(ref list);
            }
            return list;
        }

        private static void ChildrenOfType<T>(this UIElement element, ref List<T> list) where T : UIElement
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                if (child is T)
                {
                    list.Add((T) child);
                }
                var uiElement = child as UIElement;
                if (uiElement != null)
                {
                    uiElement.ChildrenOfType(ref list);
                }
            }
        }

        internal static bool IsAncestorOf(this UIElement target, DependencyObject element)
        {
            if (target == element)
            {
                return true;
            }
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (!(element is UIElement))
            {
                throw new InvalidOperationException("element is not UIElement");
            }
            var reference = element;
            do
            {
                var parent = VisualTreeHelper.GetParent(reference);
                if ((parent == null) && (reference is FrameworkElement))
                {
                    reference = ((FrameworkElement) reference).Parent;
                }
                else
                {
                    reference = parent;
                }
                if (reference == target)
                {
                    return true;
                }
            } while (reference != null);
            return false;
        }

        public static T ParentOfType<T>(this UIElement element) where T : DependencyObject
        {
            if (element == null)
            {
                return default(T);
            }
            var parent = VisualTreeHelper.GetParent(element);
            while ((parent != null) && !(parent is T))
            {
                var obj3 = VisualTreeHelper.GetParent(parent);
                if (obj3 != null)
                {
                    parent = obj3;
                }
                else
                {
                    if (!(parent is FrameworkElement))
                    {
                        break;
                    }
                    parent = (parent as FrameworkElement).Parent;
                }
            }
            return (T) parent;
        }
    }
}