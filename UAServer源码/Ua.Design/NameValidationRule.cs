// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace ConverterSystems.Ua.Design
{
    internal class NameValidationRule : ValidationRule
    {
        private static readonly Regex ValidateDisplayNameRegex = new Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]");

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var proposedValue = value as String;
            if (proposedValue != null && (ValidateDisplayNameRegex.IsMatch(proposedValue) || !Char.IsLetter(proposedValue, 0)))
            {
                return new ValidationResult(false, "Name must be a valid CLR property name.");
            }
            return new ValidationResult(true, null);
        }
    }

    internal class CacheQueueSizeValidationRule : ValidationRule
    {
        private static readonly Regex ValidateDisplayNameRegex = new Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]");

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            uint proposedValue;
            var strValue = value as string;
            if (strValue != null && !uint.TryParse(strValue, out proposedValue))
            {
                return new ValidationResult(false, "Value must be a positive integer.");
            }
            return new ValidationResult(true, null);
        }
    }
}