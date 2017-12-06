// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Globalization;
using System.Windows.Data;

namespace ConverterSystems.Workstation.Data
{
    [ValueConversion(typeof (int?), typeof (Int32))]
    public class NullIntToNegOneConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var i = value as int?;
            if (i != null)
            {
                return i.GetValueOrDefault(-1);
            }
            return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Int32)
            {
                var i = (Int32) value;
                if (i == -1)
                {
                    return new int?();
                }
                return (int?) i;
            }

            return Binding.DoNothing;
        }

        #endregion
    }
}