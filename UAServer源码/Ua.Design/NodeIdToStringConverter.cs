// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Globalization;
using System.Windows.Data;
using Opc.Ua;

namespace ConverterSystems.Ua.Design
{
    [ValueConversion(typeof (NodeId), typeof (String))]
    public class NodeIdToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var id = value as NodeId;
            if (id != null)
            {
                return (NodeId) value.ToString();
            }
            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            if (s != null)
            {
                return NodeId.Parse(s);
            }
            return NodeId.Null;
        }

        #endregion
    }
}