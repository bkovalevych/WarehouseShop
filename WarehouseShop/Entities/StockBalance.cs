using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseShop.Entities
{
    using Helpers;
    public class StockBalance : Observable
    {
        public int StockBalanceId
        {
            get => id; set => Set(ref id, value);
        }
        private int id;
        public int GoodId
        {
            get => goodId; set => Set(ref goodId, value);
        }
        private int goodId;
        public Good Good
        {
            get => good; set => Set(ref good, value);
        }
        private Good good;
        public int WarehouseId
        {
            get => warehouseId; set => Set(ref warehouseId, value);
        }
        private int warehouseId;
        public Warehouse Warehouse
        {
            get => warehouse; set => Set(ref warehouse, value);
        }
        private Warehouse warehouse;
        public double Count
        {
            get => count; set => Set(ref count, value);
        }
        private double count;
        public override string ToString()
        {
            return $"id {id} count {count}";
        }
    }
}
