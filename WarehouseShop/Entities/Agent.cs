using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseShop.Entities
{
    using Helpers;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    public class Agent : Observable
    {
        public int AgentId
        {
            get => id; set => Set(ref id, value);
        }
        private int id;
        public string TypeAgent
        {
            get => typeAgent; set => Set(ref typeAgent, value);
        }
        private string typeAgent;
        public string Name
        {
            get => name; set => Set(ref name, value);
        }
        private string name;
        public string Code
        {
            get => code; set => Set(ref code, value);
        }
        private string code;
        public ObservableCollection<Operation> Operations
        {
            get; set;
        }
        public override string ToString()
        {
            return $"id {id} name {name} typeAgent {typeAgent}";
        }
    }
}
