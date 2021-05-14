using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WarehouseShop.Helpers
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Func<bool> canExecute;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute();
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            if(execute == null)
            {
                throw new ArgumentNullException();
            }
            this.execute = (o) => execute();
            this.canExecute = canExecute;
        }
        public RelayCommand(Action<object> execute, Func<bool> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException();
            this.canExecute = canExecute;
        }

    }
}
