using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseShop.Entities
{
    using Helpers;
    using System.Collections.ObjectModel;

    public class Warehouse : Observable
    {
        public int WarehouseId
        {
            get => id; set => Set(ref id, value);
        }
        private int id;
        public string Name
        {
            get => name; set => Set(ref name, value);
        }
        private string name;
        public string Location
        {
            get => location; set => Set(ref location, value);
        }
        private string location;
        public ObservableCollection<StockBalance> StockBalances
        {
            get; set;
        }
        public ObservableCollection<GoodOperation> GoodOperations
        {
            get; set;
        }
        public override string ToString()
        {
            return $"id {id} name {name}";
        }
    }
}
