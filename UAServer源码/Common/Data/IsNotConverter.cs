// Copyright (c) 2014 Converter Systems LLC

using System.Globalization;
using System.Windows.Data;

namespace ConverterSystems.Workstation.Data
{
    /// <summary>
    /// Return inverted boolean (=!Value)
    /// </summary>
    [ValueConversion(typeof (bool), typeof (bool))]
    public class IsNotConverter : ValueConverter<bool, bool>
    {
        protected override bool Convert(bool value, object parameter, CultureInfo culture)
        {
            return !value;
        }

        protected override bool ConvertBack(bool value, object parameter, CultureInfo culture)
        {
            return !value;
        }
    }
}