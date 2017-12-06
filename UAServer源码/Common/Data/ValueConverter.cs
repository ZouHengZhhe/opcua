// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Globalization;
using System.Windows.Data;

namespace ConverterSystems.Workstation.Data
{
    public class ValueConverter<TSource, TTarget> : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == Binding.DoNothing)
            {
                return value;
            }
            if (!(value is TSource) && ((value != null) || typeof (TSource).IsValueType))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Error_ValueNotOfType {0}", new object[] { typeof (TSource).FullName }));
            }
            if (!targetType.IsAssignableFrom(typeof (TTarget)))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Error_TargetNotExtendingType {0}", new object[] { typeof (TTarget).FullName }));
            }
            return Convert((TSource) value, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TTarget) && ((value != null) || typeof (TTarget).IsValueType))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Error_ValueNotOfType {0}", new object[] { typeof (TTarget).FullName }));
            }
            if (!targetType.IsAssignableFrom(typeof (TSource)))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Error_TargetNotExtendingType {0}", new object[] { typeof (TSource).FullName }));
            }
            return ConvertBack((TTarget) value, parameter, culture);
        }

        #endregion

        protected virtual TTarget Convert(TSource value, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Error_ConverterFunctionNotDefined {0}", new object[] { "Convert" }));
        }

        protected virtual TSource ConvertBack(TTarget value, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Error_ConverterFunctionNotDefined {0}", new object[] { "ConvertBack" }));
        }
    }
}