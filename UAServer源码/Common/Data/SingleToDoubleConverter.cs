// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Globalization;
using System.Windows.Data;

namespace ConverterSystems.Workstation.Data
{
    [ValueConversion(typeof (Single), typeof (Double))]
    public class SingleToDoubleConverter : ValueConverter<Single, Double>
    {
        protected override Double Convert(Single obj, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(obj);
        }

        protected override Single ConvertBack(Double obj, object parameter, CultureInfo culture)
        {
            return System.Convert.ToSingle(obj);
        }
    }
}