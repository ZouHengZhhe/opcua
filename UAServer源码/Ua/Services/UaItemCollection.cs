// Copyright (c) 2014 Converter Systems LLC

using System.Collections.ObjectModel;
using System.Linq;
using Opc.Ua;

namespace ConverterSystems.Workstation.Services
{
    public class UaItemCollection : ObservableCollection<UaItem>
    {
        private readonly MonitoringFilter _monitoringFilter;
        private readonly MonitoringMode _monitoringMode;
        private readonly UaSubscription _subscription;
        private readonly bool _useQueue;

        public UaItemCollection(UaSubscription subscription, MonitoringMode monitoringMode, MonitoringFilter monitoringFilter, bool useQueue)
        {
            _subscription = subscription;
            _monitoringMode = monitoringMode;
            _monitoringFilter = monitoringFilter;
            _useQueue = useQueue;
        }

        public UaItem this[string displayName]
        {
            get { return this.FirstOrDefault(i => i.DisplayName == displayName); }
        }

        protected override void InsertItem(int index, UaItem item)
        {
            item.MonitoringMode = _monitoringMode;
            item.Filter = _monitoringFilter != null ? (MonitoringFilter) _monitoringFilter.Clone() : null;
            item.CacheQueue = _useQueue ? new ObservableQueue<DataValue>() : null;
            _subscription.AddItem(item);
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            var item = base[index];
            _subscription.RemoveItem(item);
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, UaItem item)
        {
            var old = base[index];
            _subscription.RemoveItem(old);
            item.MonitoringMode = _monitoringMode;
            item.Filter = _monitoringFilter != null ? (MonitoringFilter) _monitoringFilter.Clone() : null;
            item.CacheQueue = _useQueue ? new ObservableQueue<DataValue>() : null;
            _subscription.AddItem(item);
            base.SetItem(index, item);
        }

        protected override void ClearItems()
        {
            _subscription.RemoveItems(this);
            base.ClearItems();
        }
    }
}