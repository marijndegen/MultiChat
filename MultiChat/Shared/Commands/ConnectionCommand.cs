using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shared.Commands
{
    public class ConnectionCommand : ICommand
    {
        private Action ConnectionFunction;

        private Predicate<object> Enabled;

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        //private bool enable = true;

        //public bool Enable
        //{
        //    get { return enable; }
        //    set { enable = value; RaiseCanExecuteChanged(); }
        //}

        //public void RaiseCanExecuteChanged()
        //{
        //    if (CanExecuteChanged != null)
        //        CanExecuteChanged(this, new EventArgs());
        //}

        //public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return Enabled(parameter);
            //return enable;
        }

        public void Execute(object parameter)
        {
            ConnectionFunction();
        }

        public ConnectionCommand(Action connectionFunction, Predicate<object> enabled)
        {
            ConnectionFunction = connectionFunction;
            Enabled = enabled;
        }
    }
}
