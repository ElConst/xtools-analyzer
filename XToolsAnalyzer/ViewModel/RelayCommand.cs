using System;
using System.Windows.Input;

namespace XToolsAnalyzer.ViewModel
{
    /// <summary>General class representing the Command pattern to separate UI components from the logic</summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute = null;
        private readonly Func<object, bool> canExecute = null;

        public RelayCommand(Action<object> actionToExecute, Func<object, bool> executeCondition)
        {
            execute = actionToExecute ?? throw new ArgumentNullException("actionToExecute");
            canExecute = executeCondition;
        }

        public RelayCommand(Action<object> actionToExecute) : this(actionToExecute, null) { }

        public bool CanExecute(object parameter) => canExecute == null || canExecute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
