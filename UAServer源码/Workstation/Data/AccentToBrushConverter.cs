// Copyright (c) 2014 Converter Systems LLC

using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MahApps.Metro;

namespace ConverterSystems.Workstation.Data
{
    [ValueConversion(typeof (Accent), typeof (Brush))]
    public class AccentToBrushConverter : ValueConverter<Accent, Brush>
    {
        protected override Brush Convert(Accent obj, object parameter, CultureInfo culture)
        {
            return obj.Resources["AccentColorBrush"] as Brush;
        }
    }
}