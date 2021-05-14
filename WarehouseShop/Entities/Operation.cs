using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseShop.Entities
{
    using Helpers;
    using System.Collections.ObjectModel;

    public class Operation : Observable
    {
        public int OperationId
        {
            get => id; set => Set(ref id, value);
        }
        private int id;
        public int AgentId
        {
            get; set;
        }
        public Agent Agent
        {
            get => agent; set => Set(ref agent, value);
        }
        private Agent agent;
        public DateTime DateOperation
        {
            get => dateOperation; set => Set(ref dateOperation, value);
        }
        private DateTime dateOperation;
        public string TypeOperation
        {
            get => typeOperation; set => Set(ref typeOperation, value);
        }
        private string typeOperation;
        public ObservableCollection<GoodOperation> GoodOperations
        {
            get; set;
        }
        public Operation()
        {
            DateOperation = DateTime.Now;
        }
        public override string ToString()
        {
            return $"id {id} typeOperation {typeOperation}";
        }
    }
}
