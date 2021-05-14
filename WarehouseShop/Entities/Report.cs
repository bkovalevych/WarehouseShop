using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseShop.Entities
{
    using Helpers;
    public class Report : Observable
    {
        public double FullPriceSum
        {
            get; set;
        }
        public double PriceSum
        {
            get; set;
        }
        public string Agent
        {
            get; set;
        }
        public DateTime DateOperation
        {
            set; get;
        }
        public int OperationId
        {
            set; get;
        }
        public string TypeOperation
        {
            get; set;
        }
    }
}

