using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseShop.Entities
{
    using Helpers;
    using System.Collections.ObjectModel;

    public class Good : Observable
    {
        public int GoodId
        {
            get => id; set => Set(ref id, value);
        }
        private int id;
        public string Name
        {
            get => name; set => Set(ref name, value);
        }
        private string name;
        public string Description
        {
            get => description; set => Set(ref description, value);
        }
        private string description;
        public string Producer
        {
            get => producer; set => Set(ref producer, value);
        }
        private string producer;
        public string Measure
        {
            get => measure; set => Set(ref measure, value);
        }
        private string measure;
        public double Percent
        {
            get => percent; set => Set(ref percent, value);
        }
        private double percent;
        public double Price
        {
            get => price; set => Set(ref price, value);
        }
        private double price;
        public ObservableCollection<GoodOperation> GoodOperations
        {
            get; set;
        }
        public ObservableCollection<StockBalance> StockBalances
        {
            get; set;
        }
        public override string ToString()
        {
            return $"id {id} name {name}";
        }
    }
}
