using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseShop.Entities
{
    using Helpers;
    using Windows.UI.Xaml.Controls;

    public class GoodOperation : Observable
    {
        public int GoodOperationId
        {
            get => id; set => Set(ref id, value);
        }
        private int id;
        public int GoodId
        {
            get => goodId; set => Set(ref goodId, value);
        }
        public int goodId;
        public Good Good
        {
            get => good; set
            {
                if(UpdateStock(count, null, value))
                {
                    Set(ref good, value);
                }
                
            }
        }
        private Good good;
        public int WarehouseId
        {
            get => warehouseId; set
            {
                Set(ref warehouseId, value);
            }
        }
        private int warehouseId;
        public Warehouse Warehouse
        {
            get => warehouse; set
            {
                if(UpdateStock(count, value))
                {
                    Set(ref warehouse, value);
                }
            }
        }
        private Warehouse warehouse;
        public int OperationId
        {
            get => operationId; set => Set(ref operationId, value);
        }
        private int operationId;
        public Operation Operation
        {
            get => operation; set
            {
                if(!Equals(operation.TypeOperation, value.TypeOperation))
                {
                    UpdateStock(0);
                }
                Set(ref operation, value);
                UpdateStock(count);
            }
        }
        private Operation operation;
        public double Count
        {
            get => count; set 
            {
                UpdateStock(value);
                if(Good != null)
                {
                    Price = value * Good.Price;
                    FullPrice = value * Good.Price * (1 + Good.Percent);
                }
                Set(ref count, value);
            }
        }
        private double count;
        public double FullPrice
        {
            get => fullPrice; set => Set(ref fullPrice, value);
        }
        private double fullPrice;
        public double Price
        {
            get => price; set => Set(ref price, value);
        }
        private double price;

        

        private bool UpdateStock(double currentCount, Warehouse currentWarehouse = null, Good currentGood = null)
        {
            bool result;
            if(Operation == null)
            {
                return true;
            }
            if(Operation.TypeOperation == Constants.TypeOperationSold)
            {
                result = DecrementStock(currentCount, currentWarehouse, currentGood);
            }
            else
            {
                result = DecrementStock(-currentCount, currentWarehouse, currentGood);
            }
            return result;
        }
        

        private bool DecrementStock(double currentCount, Warehouse currentWarehouse = null, Good currentGood = null)
        {
            if(currentWarehouse != null && warehouse != null)
            {
                var prevStock = warehouse.StockBalances.Where(o => Equals(o.Good, Good));
                if(prevStock.Count() > 0)
                {
                    prevStock.First().Count += currentCount;
                } 
                else {
                    PopUp("На данном складе нет такого товара");
                    return false;
                }
            }
            if(currentGood != null && good != null)
            {
                var prevStock = Warehouse.StockBalances.Where(o => Equals(o.Good, Good));
                if(prevStock.Count() > 0)
                {
                    prevStock.First().Count += currentCount;
                }
                else
                {
                    PopUp("На данном складе нет такого товара");
                    return false;
                }
            }
            var stock = (currentWarehouse ?? Warehouse).StockBalances.Where(o => Equals(o.Good, Good));
            var countStock = stock.Select(o => o.Count).Sum();
            
            if(currentCount < 0)
            {
                currentCount = -currentCount;
            }
            double diff = count - currentCount;
            if(diff + countStock < 0)
            {
                diff = -countStock;
                PopUp("На данном складе нет такого количества товара");
            }
            count -= diff;
            countStock += diff;
            stock.ToList().ForEach(o => o.Count = 0);
            if(stock.Count() > 0)
            {
                stock.ToList()[0].Count = countStock;
            }
            return true;
        }

        private async void PopUp(string text)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Невозможно выполнить",
                Content = text,
                CloseButtonText = "Ok"
            };
            await dialog.ShowAsync();
        } 

        public override string ToString()
        {
            return $"id {id} count {count} price {price} fullPrice {fullPrice}";
        }
    }
}
