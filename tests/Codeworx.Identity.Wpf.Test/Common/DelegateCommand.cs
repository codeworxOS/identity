using System;
using System.Windows.Input;

namespace Codeworx.Identity.Wpf.Test.Common
{
    public class DelegateCommand<TParameter> : ICommand
    {
        private readonly Func<TParameter, bool> _canExecute;
        private readonly Action<TParameter> _executed;

        public DelegateCommand(Action<TParameter> executed, Func<TParameter, bool> canExecute = null)
        {
            _executed = executed;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke((TParameter)parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _executed((TParameter)parameter);
        }

        public void InvalidateCanExecute()
        {
            RaiseCanExecuteChanged();
        }

        protected virtual void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
