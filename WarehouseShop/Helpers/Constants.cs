using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseShop.Helpers
{
    public static class Constants
    {
        public static string TypeAgentPhisicEntity => "Фізична особа";
        public static string TypeAgentLawEntity => "Юридична особа";
        public static List<string> TypeAgent => new List<string>() { TypeAgentLawEntity, TypeAgentPhisicEntity };

        public static string MeasureVeight => " Кг";
        public static string MeasureCount => " шт.";
        public static List<string> Measure => new List<string>() { MeasureCount, MeasureVeight };

        public static double DefaultPercent => 0.05;

        public static string TypeOperationSold => "Сбыт";
        public static string TypeOperationGain => "Поставка";
        public static List<string> TypeOperation => new List<string>() { TypeOperationSold, TypeOperationGain };
    }
}
