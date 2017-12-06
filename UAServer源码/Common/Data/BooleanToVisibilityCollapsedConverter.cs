// Copyright (c) 2014 Converter Systems LLC

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ConverterSystems.Workstation.Data
{
    /// <summary>
    /// Returns Visibility.Collapsed if Value equals true 
    /// </summary>
    [ValueConversion(typeof (bool), typeof (Visibility))]
    public class BooleanToVisibilityCollapsedConverter : ValueConverter<bool, Visibility>
    {
        protected override Visibility Convert(bool value, object parameter, CultureInfo culture)
        {
            return value ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}