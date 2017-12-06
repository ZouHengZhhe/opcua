// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ConverterSystems.Workstation.Data
{
    /// <summary>
    /// Returns a string selected by index. 
    /// Provide list of strings in parameter, eg "string0,string1,string2".  
    /// </summary>
    [ValueConversion(typeof (int), typeof (String), ParameterType = typeof (string))]
    public class StringSelectionConverter : IValueConverter
    {
        private static readonly Dictionary<String, List<String>> Cache = new Dictionary<String, List<String>>();

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = System.Convert.ToInt32(value, culture);
            string key = System.Convert.ToString(parameter, culture);

            if (!String.IsNullOrEmpty(key))
            {
                List<String> list;
                if (!Cache.TryGetValue(key, out list))
                {
                    try
                    {
                        list = new List<String>(key.Split(new[] { ',' }));
                    }
                    catch
                    {
                        list = new List<string>(0);
                    }
                    Cache.Add(key, list);
                }
                return list.ElementAtOrDefault(index);
            }
            return default(String);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        #endregion
    }
}