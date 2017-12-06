// Copyright (c) 2014 Converter Systems LLC

using System.Globalization;
using System.Windows.Data;

namespace ConverterSystems.Workstation.Data
{
    /// <summary>
    /// Returns TrueResult if Value equals true, FalseResult if Value equals false 
    /// </summary>
    [ValueConversion(typeof (bool), typeof (string))]
    public class BooleanToStringConverter : ValueConverter<bool, string>
    {
        private string _falseResult = "False";
        private string _trueResult = "True";

        public string TrueResult
        {
            get { return _trueResult; }
            set { _trueResult = value; }
        }

        public string FalseResult
        {
            get { return _falseResult; }
            set { _falseResult = value; }
        }

        protected override string Convert(bool value, object parameter, CultureInfo culture)
        {
            return value ? TrueResult : FalseResult;
        }
    }
}