// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ConverterSystems.Workstation.Data
{
    /// <summary>
    /// Returns a double scaled by two points. 
    /// Provide string parameter in form of "0.0,0.0 1.0,1.0"  
    /// </summary>
    [ValueConversion(typeof (double), typeof (double), ParameterType = typeof (string))]
    public class ScalingConverter : ValueConverter<double, double>
    {
        private static readonly PointCollection Default = new PointCollection { new Point(0.0, 0.0), new Point(1.0, 1.0) };
        private static readonly Dictionary<String, PointCollection> Cache = new Dictionary<String, PointCollection>();

        protected override double Convert(double value, object parameter, CultureInfo culture)
        {
            var p = GetPointCollection(parameter);
            value = (value - p[0].X)/(p[1].X - p[0].X);
            value = Math.Min(1.0, Math.Max(0.0, value));
            value = value*(p[1].Y - p[0].Y) + p[0].Y;
            return value;
        }

        protected override double ConvertBack(double value, object parameter, CultureInfo culture)
        {
            var p = GetPointCollection(parameter);
            value = (value - p[0].Y)/(p[1].Y - p[0].Y);
            value = Math.Min(1.0, Math.Max(0.0, value));
            value = value*(p[1].X - p[0].X) + p[0].X;
            return value;
        }

        private static PointCollection GetPointCollection(object parameter)
        {
            var s = parameter as String;
            if (!String.IsNullOrEmpty(s))
            {
                PointCollection c;
                if (!Cache.TryGetValue(s, out c))
                {
                    try
                    {
                        c = PointCollection.Parse(s);
                        if (c.Count < 2)
                        {
                            c = Default;
                        }
                    }
                    catch
                    {
                        c = Default;
                    }
                    Cache.Add(s, c);
                }
                return c;
            }
            return Default;
        }
    }
}