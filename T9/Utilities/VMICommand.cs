/*
    Author: Alberto Scicali
    Custom ICommand implementation to marshal commands from UI to ViewModel
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace T9.Utilities
{
   public class VMICommand : ICommand {
        public Action<object> _TargetExecuteMethod;
        public Func<bool> _TargetCanExecuteMethod;

        // Constructor with a given action to execute
        public VMICommand(Action<object> executeMethod)
        {
            _TargetExecuteMethod = executeMethod;
        }

        // Constructor with execute action and can execute function
        public VMICommand(Action<Object> executeMethod, Func<bool> canExecuteMethod)
        {
            _TargetExecuteMethod = executeMethod;
            _TargetCanExecuteMethod = canExecuteMethod;
        }

        // Raises when the commands executability has changed
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        // Check if the command can be executed yet
        bool ICommand.CanExecute(object parameter)
        {
            if (_TargetCanExecuteMethod != null)
            {
                return _TargetCanExecuteMethod();
            }

            if (_TargetExecuteMethod != null)
            {
                return true;
            }

            return false;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        // Executes the function if not null
        void ICommand.Execute(object parameter)
        {
            _TargetExecuteMethod?.Invoke(parameter);
        }
    }
}
