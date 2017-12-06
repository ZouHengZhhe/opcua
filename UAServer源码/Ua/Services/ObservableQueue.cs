// Copyright (c) 2014 Converter Systems LLC

using System.Collections.Generic;
using System.Collections.Specialized;

namespace ConverterSystems.Workstation.Services
{
    public class ObservableQueue<T> : Queue<T>, INotifyCollectionChanged
    {
        public ObservableQueue()
        {
        }

        public ObservableQueue(int capacity) : base(capacity)
        {
        }

        public ObservableQueue(IEnumerable<T> collection) : base(collection)
        {
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public new void Clear()
        {
            base.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new void Enqueue(T item)
        {
            base.Enqueue(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, Count - 1));
        }

        public new T Dequeue()
        {
            var item = base.Dequeue();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, 0));
            return item;
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }
    }
}