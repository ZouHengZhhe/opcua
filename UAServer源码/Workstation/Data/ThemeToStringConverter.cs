// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Globalization;
using System.Windows.Data;
using MahApps.Metro;

namespace ConverterSystems.Workstation.Data
{
    [ValueConversion(typeof (Theme), typeof (String))]
    public class ThemeToStringConverter : ValueConverter<Theme, String>
    {
        protected override String Convert(Theme obj, object parameter, CultureInfo culture)
        {
            if (obj == Theme.Dark)
            {
                return "Dark";
            }
            return "Light";
        }
    }
}