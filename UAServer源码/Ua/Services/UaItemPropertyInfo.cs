// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using Opc.Ua;

namespace ConverterSystems.Workstation.Services
{
    public class UaItemPropertyInfo : PropertyInfo
    {
        private readonly UaItem _item;

        public UaItemPropertyInfo(UaItem item)
        {
            _item = item;
        }

        public override PropertyAttributes Attributes
        {
            get { return PropertyAttributes.None; }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override Type PropertyType
        {
            get { return _item.Type; }
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

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            return new MethodInfo[0];
        }

        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            return null;
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            return new ParameterInfo[0];
        }

        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            return null;
        }

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            return GetValueImpl((UaSubscription) obj, _item);
        }

        private static object GetValueImpl(UaSubscription subscription, UaItem item)
        {
            var dataValue = subscription.GetValue(item);
            return dataValue.StatusCode != StatusCodes.BadWaitingForInitialData ? dataValue.Value : DependencyProperty.UnsetValue;
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            SetValueImpl((UaSubscription) obj, _item, value);
        }

        private static async void SetValueImpl(UaSubscription subscription, UaItem item, object value)
        {
            // as discovered by rdgerken, Kepware doesn't like getting a WriteValue with a SourceTimeStamp
            await subscription.SetValueAsync(item, new DataValue(new Variant(value))).ConfigureAwait(false);
            //await subscription.SetValueAsync(item, new DataValue(new Variant(value), StatusCodes.Good, DateTime.UtcNow)).ConfigureAwait(false);
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

        public override string ToString()
        {
            return Name;
        }
    }
}