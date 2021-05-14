using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseShop.Helpers
{

    public class Observable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;




        protected void OnPropertyChanged(string name)
        {
            if(name != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
        protected void Set<T>(ref T source, T value, [CallerMemberName] string propertyName = null)
        {
            if(value == null || !value.Equals(source))
            {
                source = value;
                OnPropertyChanged(propertyName);
            }

        }


    }
}
