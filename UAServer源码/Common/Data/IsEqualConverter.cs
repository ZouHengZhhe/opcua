// Copyright (c) 2014 Converter Systems LLC

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ConverterSystems.Workstation.Data
{
    /// <summary>
    /// Return true if Value equals Parameter 
    /// </summary>
    [ValueConversion(typeof (object), typeof (bool), ParameterType = typeof (object))]
    public class IsEqualConverter : ValueConverter<object, bool>
    {
        // Methods
        protected override bool Convert(object value, object parameter, CultureInfo culture)
        {
            return (Equals(value, parameter) || ((value != null) && value.Equals(parameter)));
        }

        protected override object ConvertBack(bool value, object parameter, CultureInfo culture)
        {
            if (value)
            {
                return parameter;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}