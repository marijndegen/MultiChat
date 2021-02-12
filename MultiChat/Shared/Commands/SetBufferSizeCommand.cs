using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Shared.Commands
{
    public class SetBufferSizeCommand : ICommand
    {
        private Action AdjustBufferFunction;

        private bool enable = false;

        public bool Enable
        {
            get { return enable; }
            set { enable = value; RaiseCanExecuteChanged(); }
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return enable;
        }

        public void Execute(object parameter)
        {
            AdjustBufferFunction();
        }

        public SetBufferSizeCommand(Action adjustBufferFunction)
        {
            AdjustBufferFunction = adjustBufferFunction;
        }
    }
}
