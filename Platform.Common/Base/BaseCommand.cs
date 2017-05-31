using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Platform.Common.Base
{
    public class BaseCommand : ICommand
    {
        private Predicate<object> _canExecuteHandler;
        private Action<object> _executeHandler;
        
        /// <summary>
        /// 基础命令，对ICommand进行初步封装
        /// </summary>
        /// <param name="executeHandler">命令执行主体</param>
        /// <param name="canExecuteHandler">命令执行条件</param>
        public BaseCommand(Action<object> executeHandler, Predicate<object> canExecuteHandler = null)
        {
            _canExecuteHandler = canExecuteHandler;
            _executeHandler = executeHandler;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteHandler == null ? true : _canExecuteHandler(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if (_executeHandler != null)
                _executeHandler(parameter);
        }
    }
}
