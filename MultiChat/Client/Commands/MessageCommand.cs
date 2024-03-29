﻿using System;
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
            //return enable;
        }

        public void Execute(object parameter)
        {
            MessageFunction();
        }

        //Feedback: This should also contain a function that provides canexecute.
        public MessageCommand(Action connectionFunction, Predicate<object> enabled)
        {
            MessageFunction = connectionFunction;
            Enabled = enabled;
            //Feedback: Set the canexecute function here
        }
    }
}
