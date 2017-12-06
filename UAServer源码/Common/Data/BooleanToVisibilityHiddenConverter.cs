// Copyright (c) 2014 Converter Systems LLC

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ConverterSystems.Workstation.Data
{
    /// <summary>
    /// Return Visibility.Hidden if Value equals true 
    /// </summary>
    [ValueConversion(typeof (bool), typeof (Visibility))]
    public class BooleanToVisibilityHiddenConverter : ValueConverter<bool, Visibility>
    {
        protected override Visibility Convert(bool value, object parameter, CultureInfo culture)
        {
            return value ? Visibility.Hidden : Visibility.Visible;
        }
    }
}