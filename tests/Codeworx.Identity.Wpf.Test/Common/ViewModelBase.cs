using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Codeworx.Identity.Wpf.Test.Common
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        private readonly ConcurrentDictionary<object, string> _busyJobs = new ConcurrentDictionary<object, string>();
        private string _busyMessage;

        private bool _isBusy;

        public event PropertyChangedEventHandler PropertyChanged;

        public string BusyMessage
        {
            get
            {
                return _busyMessage;
            }

            set
            {
                if (_busyMessage != value)
                {
                    _busyMessage = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    RaisePropertyChanged();
                }
            }
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual IDisposable StartJob(string message)
        {
            var key = new object();
            _busyJobs.AddOrUpdate(key, message, (p, q) => message);
            BusyMessage = message;

            return new Job(key, this);
        }

        private class Job : IDisposable
        {
            private readonly object _key;
            private readonly ViewModelBase _viewModelBase;
            private bool _disposedValue;

            public Job(object key, ViewModelBase viewModelBase)
            {
                _key = key;
                _viewModelBase = viewModelBase;
            }

            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposedValue)
                {
                    if (disposing)
                    {
                        if (_viewModelBase._busyJobs.TryRemove(_key, out var value))
                        {
                            _viewModelBase.BusyMessage = _viewModelBase._busyJobs.Values.LastOrDefault();
                        }
                    }

                    _disposedValue = true;
                }
            }
        }
    }
}
