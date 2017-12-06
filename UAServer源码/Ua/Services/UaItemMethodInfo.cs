// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Globalization;
using System.Reflection;

namespace ConverterSystems.Workstation.Services
{
    public class UaItemMethodInfo : MethodInfo
    {
        private readonly UaItem _item;

        public UaItemMethodInfo(UaItem item)
        {
            _item = item;
        }

        public override RuntimeMethodHandle MethodHandle
        {
            get { throw new NotImplementedException(); }
        }

        public override MethodAttributes Attributes
        {
            get { return MethodAttributes.Public; }
        }

        public override Type DeclaringType
        {
            get { return typeof (UaSubscription); }
        }

        public override string Name
        {
            get { return _item.DisplayName; }
        }

        public override Type ReflectedType
        {
            get { return typeof (UaSubscription); }
        }

        public override ICustomAttributeProvider ReturnTypeCustomAttributes
        {
            get { throw new NotImplementedException(); }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return new object[0];
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return new object[0];
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return false;
        }

        public override ParameterInfo[] GetParameters()
        {
            throw new NotImplementedException();
        }

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            throw new NotImplementedException();
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
            if (parameters == null || parameters.Length < 1)
            {
                throw new ArgumentNullException("parameters", "parameters must not be null or empty");
            }
            return ((UaSubscription) obj).CallMethodAsync(_item, (object[]) parameters[0]);
        }

        public override MethodInfo GetBaseDefinition()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}