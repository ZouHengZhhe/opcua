// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;
using Opc.Ua;
using Opc.Ua.Client;

namespace ConverterSystems.Workstation.Services
{
    /// <summary>
    /// Subscription to Opc UA server
    /// </summary>
    [ContentProperty("Properties"), RuntimeNameProperty("DisplayName")]
    public class UaSubscription : Subscription, INotifyPropertyChanged, ICustomTypeProvider, IDynamicMetaObjectProvider
    {
        private readonly UaItemCollection _commands;
        private readonly SynchronizationContext _context;
        private readonly UaItemCollection _datasources;
        private readonly UaItemCollection _methods;
        private readonly UaItemCollection _properties;
        private Type _customType;
        private PropertyChangedEventHandler _propertyChanged;
        private UaSession _session;

        public UaSubscription()
        {
            base.PublishingInterval = 250;
            PublishingEnabled = true;
            KeepAliveCount = 10;
            LifetimeCount = 100;
            DisableMonitoredItemCache = true;
            FastDataChangeCallback = OnDataChangeCallback;
            _properties = new UaItemCollection(this, MonitoringMode.Reporting, null, false);
            _commands = new UaItemCollection(this, MonitoringMode.Disabled, null, false);
            _datasources = new UaItemCollection(this, MonitoringMode.Reporting, new DataChangeFilter { Trigger = DataChangeTrigger.StatusValueTimestamp }, true);
            _methods = new UaItemCollection(this, MonitoringMode.Disabled, null, false);
            _context = SynchronizationContext.Current;
        }

        [Category("Common")]
        public new UaSession Session
        {
            get { return _session; }
            set
            {
                _session = value;
                NotifyPropertyChanged();
            }
        }

        [Browsable(false)]
        public new string DisplayName
        {
            get { return base.DisplayName; }
            set
            {
                base.DisplayName = value;
                NotifyPropertyChanged();
            }
        }

        [Category("Common"), DefaultValue(250)]
        public new int PublishingInterval
        {
            get { return base.PublishingInterval; }
            set
            {
                base.PublishingInterval = value;
                NotifyPropertyChanged();
            }
        }

        [Category("Common")]
        public UaItemCollection Properties
        {
            get { return _properties; }
        }

        [Category("Common")]
        public UaItemCollection Commands
        {
            get { return _commands; }
        }

        [Category("Common")]
        public UaItemCollection DataSources
        {
            get { return _datasources; }
        }

        [Category("Common")]
        public UaItemCollection Methods
        {
            get { return _methods; }
        }

        public Type GetCustomType()
        {
            return _customType ?? (_customType = new UaSubscriptionCustomType(Properties.Select(p => (PropertyInfo) new UaItemPropertyInfo(p)).Concat(Commands.Select(p => (PropertyInfo) new UaItemCommandInfo(p))).Concat(DataSources.Select(p => (PropertyInfo) new UaItemDataSourceInfo(p))).ToArray(), Methods.Select(m => (MethodInfo) new UaItemMethodInfo(m)).ToArray()));
        }

        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new UaSubscriptionMetaObject(parameter, this);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (_propertyChanged == null)
                {
                    var session = Session;
                    if (session != null && !session.IsInDesignMode)
                    {
                        session.AddSubscription(this);
                    }
                }
                _propertyChanged += value;
            }
            remove
            {
                _propertyChanged -= value;
                if (_propertyChanged == null)
                {
                    var session = Session;
                    if (session != null && !session.IsInDesignMode)
                    {
                        session.RemoveSubscription(this);
                    }
                }
            }
        }

        public override string ToString()
        {
            return string.Format("UaSubscription DisplayName: {0}, Id: {1}", DisplayName, Id);
        }

        protected override void Dispose(bool disposing)
        {
            Trace.TraceInformation("Success disposing subscription '{0}'.", DisplayName);
            base.Dispose(disposing);
        }

        internal void AddItem(UaItem item)
        {
            base.AddItem(item);
            item.PropertyChanged += OnItemPropertyChanged;
            _customType = null;
            if (Created)
            {
                CreateItems();
            }
        }

        internal void RemoveItem(UaItem item)
        {
            base.RemoveItem(item);
            item.PropertyChanged -= OnItemPropertyChanged;
            _customType = null;
            if (Created)
            {
                DeleteItems();
            }
        }

        internal DataValue GetValue(UaItem item)
        {
            return item.CacheValue;
        }

        internal async Task SetValueAsync(UaItem item, DataValue value)
        {
            item.CacheValue = value;
            NotifyPropertyChanged(item.DisplayName);
            var session = Session;
            if (session != null && session.Connected)
            {
                try
                {
                    var writeRequest = new WriteRequest { NodesToWrite = { new WriteValue { NodeId = item.ResolvedNodeId, AttributeId = (uint) item.AttributeId, Value = value } } };
                    var writeResponse = await session.WriteAsync(writeRequest).ConfigureAwait(false);
                    for (int i = 0; i < writeResponse.Results.Count; i++)
                    {
                        if (StatusCode.IsNotGood(writeResponse.Results[i]))
                        {
                            Trace.TraceError("Error writing value for NodeId {0} : {1}", writeRequest.NodesToWrite[i].NodeId, writeResponse.Results[i]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Error writing value for NodeId {0} : {1}", item.ResolvedNodeId, ex.Message);
                }
            }
            else
            {
                Trace.TraceError("Error writing value for NodeId {0} : {1}", item.ResolvedNodeId, "Session is null or not connected");
            }
        }

        internal async Task<object[]> CallMethodAsync(UaItem item, object[] inputArguments)
        {
            var session = Session;
            if (session != null && session.Connected)
            {
                try
                {
                    var callRequest = new CallRequest { MethodsToCall = { new CallMethodRequest { ObjectId = item.StartNodeId, MethodId = item.ResolvedNodeId, InputArguments = inputArguments.Select(a => new Variant(a)).ToArray() } } };
                    var callResponse = await session.CallAsync(callRequest).ConfigureAwait(false);
                    for (int i = 0; i < callResponse.Results.Count; i++)
                    {
                        if (StatusCode.IsNotGood(callResponse.Results[i].StatusCode))
                        {
                            Trace.TraceError("Error calling method with MethodId {0} : {1}", callRequest.MethodsToCall[i].MethodId, callResponse.Results[i].StatusCode);
                        }
                    }
                    return callResponse.Results[0].OutputArguments.Select(a => a.Value).ToArray();
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Error calling method with MethodId {0} : {1}", item.ResolvedNodeId, ex.Message);
                }
            }
            else
            {
                Trace.TraceError("Error calling method with MethodId {0} : {1}", item.ResolvedNodeId, "Session is null or not connected");
            }
            return null;
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = _propertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _customType = null;
            if (Created)
            {
                ModifyItems();
            }
        }

        private void OnDataChangeCallback(Subscription subscription, DataChangeNotification notification, IList<string> stringTable)
        {
            _context.Send(o =>
            {
                var state = (Tuple<UaSubscription, DataChangeNotification>) o;
                foreach (var itemNotification in state.Item2.MonitoredItems)
                {
                    var item = state.Item1.FindItemByClientHandle(itemNotification.ClientHandle) as UaItem;
                    if (item != null)
                    {
                        var cacheQueue = item.CacheQueue;
                        if (cacheQueue != null)
                        {
                            cacheQueue.Enqueue(itemNotification.Value);
                            while (cacheQueue.Count > item.CacheQueueSize)
                            {
                                cacheQueue.Dequeue();
                            }
                            continue;
                        }
                        item.CacheValue = itemNotification.Value;
                        state.Item1.NotifyPropertyChanged(item.DisplayName);
                    }
                }
            }, Tuple.Create(this, notification));
        }

        internal object GetValueByName(string name)
        {
            var pi = GetCustomType().GetProperty(name);
            if (pi != null && pi.CanRead)
            {
                return pi.GetValue(this);
            }
            Trace.TraceError("Error getting value of property '{0}' of UaSubscription '{1}'", name, DisplayName);
            return null;
        }

        internal object SetValueByName(string name, object value)
        {
            var pi = GetCustomType().GetProperty(name);
            if (pi != null && pi.CanWrite)
            {
                pi.SetValue(this, value);
                return value;
            }
            Trace.TraceError("Error setting value of property '{0}' of UaSubscription '{1}'", name, DisplayName);
            return value;
        }

        internal object CallMethodByName(string name, object value)
        {
            var mi = GetCustomType().GetMethod(name);
            if (mi != null)
            {
                return mi.Invoke(this, new[] { value });
            }
            Trace.TraceError("Error calling method '{0}' of UaSubscription '{1}'", name, DisplayName);
            return null;
        }

        private class UaSubscriptionCustomType : TypeDelegator
        {
            private readonly MethodInfo[] _methods;
            private readonly PropertyInfo[] _properties;

            public UaSubscriptionCustomType(PropertyInfo[] properties, MethodInfo[] methods) : base(typeof (UaSubscription))
            {
                _properties = properties;
                _methods = methods;
            }

            public override Type BaseType
            {
                get { return null; }
            }

            public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
            {
                return _methods;
            }

            protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
            {
                return GetMethods(bindingAttr).FirstOrDefault(mi => mi.Name == name);
            }

            public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
            {
                return _properties;
            }

            protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
            {
                return GetProperties(bindingAttr).FirstOrDefault(pi => pi.Name == name);
            }
        }

        private class UaSubscriptionMetaObject : DynamicMetaObject
        {
            private static readonly MethodInfo GetMemberMethodInfo = typeof (UaSubscription).GetMethod("GetValueByName", BindingFlags.Instance | BindingFlags.NonPublic);
            private static readonly MethodInfo SetMemberMethodInfo = typeof (UaSubscription).GetMethod("SetValueByName", BindingFlags.Instance | BindingFlags.NonPublic);
            private static readonly MethodInfo CallMethodMethodInfo = typeof (UaSubscription).GetMethod("CallMethodByName", BindingFlags.Instance | BindingFlags.NonPublic);

            internal UaSubscriptionMetaObject(Expression parameter, UaSubscription value) : base(parameter, BindingRestrictions.Empty, value)
            {
            }

            public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            {
                return new DynamicMetaObject(Expression.Call(Expression.Convert(Expression, LimitType), GetMemberMethodInfo, new Expression[] { Expression.Constant(binder.Name) }), BindingRestrictions.GetTypeRestriction(Expression, LimitType));
            }

            public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
            {
                return new DynamicMetaObject(Expression.Call(Expression.Convert(Expression, LimitType), SetMemberMethodInfo, new Expression[] { Expression.Constant(binder.Name), Expression.Convert(value.Expression, typeof (object)) }), BindingRestrictions.GetTypeRestriction(Expression, LimitType));
            }

            public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
            {
                return new DynamicMetaObject(Expression.Call(Expression.Convert(Expression, LimitType), CallMethodMethodInfo, new[] { Expression.Constant(binder.Name), args[0].Expression }), BindingRestrictions.GetTypeRestriction(Expression, LimitType));
            }
        }
    }
}