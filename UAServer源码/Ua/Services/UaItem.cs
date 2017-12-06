// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using Opc.Ua;
using Opc.Ua.Client;

namespace ConverterSystems.Workstation.Services
{
    /// <summary>
    /// The UaItem class.
    /// </summary>
    public class UaItem : MonitoredItem, INotifyPropertyChanged
    {
        internal ObservableQueue<DataValue> CacheQueue;
        internal DataValue CacheValue;
        private Type _type;

        public UaItem()
        {
            _type = typeof (object);
            CacheValue = new DataValue(StatusCodes.BadWaitingForInitialData);
        }

        [DefaultValue(13), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new int AttributeId
        {
            get { return (int) base.AttributeId; }
            set
            {
                base.AttributeId = (uint) value;
                NotifyPropertyChanged();
            }
        }

        [DefaultValue(1), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new int CacheQueueSize
        {
            get { return base.CacheQueueSize; }
            set
            {
                base.CacheQueueSize = value;
                NotifyPropertyChanged();
            }
        }

        [DefaultValue(true), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new bool DiscardOldest
        {
            get { return base.DiscardOldest; }
            set
            {
                base.DiscardOldest = value;
                NotifyPropertyChanged();
            }
        }

        [Category("Common")]
        public new string DisplayName
        {
            get { return base.DisplayName; }
            set
            {
                base.DisplayName = value;
                NotifyPropertyChanged();
            }
        }

        [DefaultValue(null), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new QualifiedName Encoding
        {
            get { return base.Encoding; }
            set
            {
                base.Encoding = value;
                NotifyPropertyChanged();
            }
        }

        [DefaultValue(null), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new MonitoringFilter Filter
        {
            get { return base.Filter; }
            internal set
            {
                base.Filter = value;
                NotifyPropertyChanged();
            }
        }

        [DefaultValue(null), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new object Handle
        {
            get { return base.Handle; }
            set
            {
                base.Handle = value;
                NotifyPropertyChanged();
            }
        }

        [DefaultValue(null), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new string IndexRange
        {
            get { return base.IndexRange; }
            set
            {
                base.IndexRange = value;
                NotifyPropertyChanged();
            }
        }

        [DefaultValue(MonitoringMode.Reporting), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new MonitoringMode MonitoringMode
        {
            get { return base.MonitoringMode; }
            internal set
            {
                base.MonitoringMode = value;
                NotifyPropertyChanged();
            }
        }

        [DefaultValue(NodeClass.Variable), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new NodeClass NodeClass
        {
            get { return base.NodeClass; }
            set
            {
                base.NodeClass = value;
                NotifyPropertyChanged();
            }
        }

        [DefaultValue(0), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new int QueueSize
        {
            get { return (int) base.QueueSize; }
            set
            {
                base.QueueSize = (uint) value;
                NotifyPropertyChanged();
            }
        }

        [DefaultValue(null), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new string RelativePath
        {
            get { return base.RelativePath; }
            set
            {
                base.RelativePath = value;
                NotifyPropertyChanged();
            }
        }

        [DefaultValue(-1), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new int SamplingInterval
        {
            get { return base.SamplingInterval; }
            set
            {
                base.SamplingInterval = value;
                NotifyPropertyChanged();
            }
        }

        [Category("Common"), TypeConverter(typeof (NodeIdConverter))]
        public new NodeId StartNodeId
        {
            get { return base.StartNodeId; }
            set
            {
                base.StartNodeId = value;
                NotifyPropertyChanged();
            }
        }

        [Category("Common"), TypeConverter(typeof (TypeNameConverter))]
        public Type Type
        {
            get { return _type; }
            set
            {
                _type = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}