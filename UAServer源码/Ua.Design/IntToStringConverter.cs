// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Globalization;
using System.Windows.Data;

namespace ConverterSystems.Ua.Design
{
    [ValueConversion(typeof (int), typeof (string), ParameterType = typeof (int))]
    public class IntToStringConverter : IValueConverter
    {
        public int DefaultInteger { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                var i = (int) value;
                return i.ToString(culture);
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s1 = value as string;
            if (s1 != null)
            {
                int i;
                if (int.TryParse(s1, NumberStyles.Integer, culture, out i))
                {
                    return i;
                }
            }
            return DefaultInteger;
        }
    }
}