using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Shared.Commands
{
    public class SetBufferSizeCommand : ICommand
    {
        private Action AdjustBufferFunction;

        private Predicate<object> Enabled;

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return Enabled(parameter);
        }

        public void Execute(object parameter)
        {
            AdjustBufferFunction();
        }

        public SetBufferSizeCommand(Action adjustBufferFunction, Predicate<object> enabled)
        {
            AdjustBufferFunction = adjustBufferFunction;
            Enabled = enabled;
        }
    }
}
