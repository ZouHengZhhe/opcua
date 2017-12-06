// Copyright (c) 2014 Converter Systems LLC

using System.Globalization;
using System.Windows.Data;

namespace ConverterSystems.Workstation.Data
{
    [ValueConversion(typeof (object), typeof (object))]
    public class DummyConverter : ValueConverter<object, object>
    {
        protected override object Convert(object obj, object parameter, CultureInfo culture)
        {
            return obj;
        }

        protected override object ConvertBack(object obj, object parameter, CultureInfo culture)
        {
            return obj;
        }
    }
}