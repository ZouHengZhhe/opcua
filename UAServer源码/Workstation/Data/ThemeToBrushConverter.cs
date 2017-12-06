// Copyright (c) 2014 Converter Systems LLC

using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MahApps.Metro;

namespace ConverterSystems.Workstation.Data
{
    [ValueConversion(typeof (Theme), typeof (Brush))]
    public class ThemeToBrushConverter : ValueConverter<Theme, Brush>
    {
        protected override Brush Convert(Theme obj, object parameter, CultureInfo culture)
        {
            if (obj == Theme.Dark)
            {
                return Brushes.Black;
            }
            return Brushes.White;
        }
    }
}