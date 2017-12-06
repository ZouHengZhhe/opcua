// Copyright (c) 2014 Converter Systems LLC

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ConverterSystems.Workstation.Data
{
    /// <summary>
    /// Returns Visibility.Collapsed if Value is Null 
    /// </summary>
    [ValueConversion(typeof (object), typeof (Visibility))]
    public class NullToVisibilityCollapsedConverter : ValueConverter<object, Visibility>
    {
        protected override Visibility Convert(object obj, object parameter, CultureInfo culture)
        {
            return obj != null ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}