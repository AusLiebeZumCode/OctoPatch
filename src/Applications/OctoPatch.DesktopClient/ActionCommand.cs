using System;
using System.Windows.Input;

namespace OctoPatch.DesktopClient
{
    public sealed class ActionCommand : ICommand
    {
        private readonly Action<object> _callback;

        private bool _enabled;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public ActionCommand(Action<object> callback, bool enabled = true)
        {
            _callback = callback;
            _enabled = enabled;
        }

        public bool CanExecute(object parameter)
        {
            return _enabled;
        }

        public void Execute(object parameter)
        {
            _callback?.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
