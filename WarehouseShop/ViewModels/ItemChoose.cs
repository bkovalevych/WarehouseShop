using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace WarehouseShop.ViewModels
{
    using Helpers;
    public class ItemChoose
    {
        public string Variant
        {
            get; set;
        }
        public IEnumerable<object> Val
        {
            get; set;
        }
        public override string ToString()
        {
            return Variant;
        }
    }
}
