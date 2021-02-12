using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.Commands
{
    public class MessageCommand : ICommand
    {
        private Action MessageFunction;

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
            MessageFunction();
        }

        public MessageCommand(Action connectionFunction)
        {
            MessageFunction = connectionFunction;
        }
    }
}
