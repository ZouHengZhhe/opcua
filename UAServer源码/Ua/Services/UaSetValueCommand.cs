// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Opc.Ua;

namespace ConverterSystems.Workstation.Services
{
    public sealed class UaSetValueCommand : ICommand
    {
        private readonly UaItem _item;
        private bool _isExecuting;

        public UaSetValueCommand(UaItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            _item = item;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return !_isExecuting;
        }

        public event EventHandler CanExecuteChanged;

        async void ICommand.Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        private async Task ExecuteAsync(object parameter = null)
        {
            _isExecuting = true;
            RaiseCanExecuteChanged();
            try
            {
                var sub = _item.Subscription as UaSubscription;
                if (sub != null)
                {
                    // as discovered by rdgerken, Kepware doesn't like getting a WriteValue with a SourceTimeStamp
                    await sub.SetValueAsync(_item, new DataValue(new Variant(parameter)));
                    // await sub.SetValueAsync(_item, new DataValue(new Variant(parameter), StatusCodes.Good, DateTime.UtcNow));
                }
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        private void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}