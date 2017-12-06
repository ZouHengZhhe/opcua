// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Windows.Design.Model;

namespace ConverterSystems.Ua.Design
{
    public class ObservableModelItemCollection : IList<ModelItem>, IList, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly ModelItemCollection _collection;

        public ObservableModelItemCollection(ModelItemCollection collection)
        {
            _collection = collection;
        }

        public void Add(ModelItem item)
        {
            int index = ((IList) _collection).Add(item);
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public void Clear()
        {
            ((IList) _collection).Clear();
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(ModelItem item)
        {
            return ((IList) _collection).Contains(item);
        }

        public void CopyTo(ModelItem[] array, int arrayIndex)
        {
            ((IList) _collection).CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return ((IList) _collection).Count; }
        }

        public int IndexOf(ModelItem item)
        {
            return ((IList) _collection).IndexOf(item);
        }

        public void Insert(int index, ModelItem item)
        {
            ((IList) _collection).Insert(index, item);
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public bool IsReadOnly
        {
            get { return ((IList) _collection).IsReadOnly; }
        }

        public bool Remove(ModelItem item)
        {
            int index = ((IList) _collection).IndexOf(item);
            if (index < 0)
            {
                return false;
            }
            ((IList) _collection).Remove(item);
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            return true;
        }

        public void RemoveAt(int index)
        {
            var item = ((IList) _collection)[index];
            ((IList) _collection).RemoveAt(index);
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        public ModelItem this[int index]
        {
            get { return ((IList) _collection)[index] as ModelItem; }
            set
            {
                var oldvalue = ((IList) _collection)[index];
                ((IList) _collection)[index] = value;
                OnPropertyChanged("Item[]");
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldvalue, index));
            }
        }

        #region IList Members

        public int Add(object value)
        {
            int index = ((IList) _collection).Add(value);
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
            return index;
        }

        public bool Contains(object value)
        {
            return ((IList) _collection).Contains(value);
        }

        public int IndexOf(object value)
        {
            return ((IList) _collection).IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            ((IList) _collection).Insert(index, value);
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
        }

        public bool IsFixedSize
        {
            get { return ((IList) _collection).IsFixedSize; }
        }

        public void Remove(object value)
        {
            int index = ((IList) _collection).IndexOf(value);
            ((IList) _collection).Remove(value);
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
        }

        public void CopyTo(Array array, int index)
        {
            ((IList) _collection).CopyTo(array, index);
        }

        public bool IsSynchronized
        {
            get { return ((IList) _collection).IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return ((IList) _collection).SyncRoot; }
        }

        object IList.this[int index]
        {
            get { return ((IList) _collection)[index] as ModelItem; }
            set
            {
                var oldvalue = ((IList) _collection)[index];
                ((IList) _collection)[index] = value;
                OnPropertyChanged("Item[]");
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldvalue, index));
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList) _collection).GetEnumerator();
        }

        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IEnumerable<ModelItem> Members

        public IEnumerator<ModelItem> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        #endregion

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}