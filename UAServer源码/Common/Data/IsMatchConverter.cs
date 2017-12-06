// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace ConverterSystems.Workstation.Data
{
    /// <summary>
    /// Return true if Value matches the regular expression (=Parameter) ?
    /// </summary>
    [ValueConversion(typeof (string), typeof (bool), ParameterType = typeof (string))]
    public class IsMatchConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value == null) || (parameter == null))
            {
                return false;
            }

            var regex = new Regex((string) parameter);
            return regex.IsMatch((string) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}