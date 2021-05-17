using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.XlsIO;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI;

namespace WarehouseShop.ViewModels
{
    using Helpers;
    using Entities;
    using Windows.UI.Xaml;
    using Microsoft.Toolkit.Uwp.UI.Controls;
    using System.Windows.Input;
    using System.Text.RegularExpressions;
    using System.Diagnostics;

    public class BaseVM : Observable
    {
        public ObservableCollection<Warehouse> Warehouses
        {
            get; set;
        }
        public ObservableCollection<StockBalance> StockBalances
        {
            get; set;
        }
        public ObservableCollection<StockBalance> MinStockBalances
        {
            get; set;
        } = new ObservableCollection<StockBalance>();
        public ObservableCollection<Operation> Operations
        {
            get; set;
        }
        public ObservableCollection<GoodOperation> GoodOperations
        {
            get; set;
        }
        public ObservableCollection<Good> Goods
        {
            get; set;
        }
        public ObservableCollection<Good> AppropriateGoods
        {
            get; set;
        } = new ObservableCollection<Good>();
        public ObservableCollection<Agent> Agents
        {
            get; set;
        }
        public ObservableCollection<Report> Reports
        {
            get; set;
        }
        public List<ItemChoose> Variants
        {
            get; set;
        }
        public ItemChoose SelectedVariant
        {
            get => selectedVariant; set
            {

                
                Set(ref selectedVariant, value);
                SelectedTemplate.Clear();
                Add(SelectedTemplate, dict[value.Variant]);
            }
        }
        private ItemChoose selectedVariant;
        private Dictionary<String, ObservableCollection<DataGridColumn>> dict;

        public int SelectedGlobalRowIndex
        {
            get => selectedGlobalRowIndex; set => Set(ref selectedGlobalRowIndex, value);
        }
        private int selectedGlobalRowIndex = -1;

        public int RowSpan
        {
            get => rowSpan; set => Set(ref rowSpan, value);
        }
        private int rowSpan = 1;
        public Observable SelectedRow
        {
            get => selectedRow; set
            {
                if(value is Warehouse w)
                {
                    LowLevel = w.StockBalances;
                }
                else if(value is Operation op)
                {
                    LowLevel = op.GoodOperations;
                }
                else if(value is Agent a)
                {
                    LowLevel = a.Operations;
                }
                else if(value is GoodOperation gp)
                {
                    gp.PropertyChanged += Gp_PropertyChanged;
                    if(gp.WarehouseId != 0)
                    {
                        AppropriateGoods.Clear();
                        var warehouseSel = Warehouses.Where(o => o.WarehouseId == gp.WarehouseId);
                        if(warehouseSel.Count() != 0 && warehouseSel.ToList()[0] is Warehouse warehouse)
                        {
                            warehouse.StockBalances.ToList().ForEach(sb =>
                            {
                                AppropriateGoods.Add(sb.Good);
                            });
                        }
                    }
                }
                else
                {
                    LowLevel = new List<object>();
                }
                Set(ref selectedRow, value);
            }
        }

        private void Gp_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "WarehouseId")
            {
                AppropriateGoods.Clear();
                var gp = sender as GoodOperation;
                if(gp.WarehouseId != 0)
                {
                    AppropriateGoods.Clear();
                    var warehouseSel = Warehouses.Where(o => o.WarehouseId == gp.WarehouseId);
                    if(warehouseSel.Count() != 0 && warehouseSel.ToList()[0] is Warehouse warehouse)
                    {
                        warehouse.StockBalances.ToList().ForEach(sb =>
                        {
                            AppropriateGoods.Add(sb.Good);
                        });
                        gp.Warehouse = warehouse;
                    }
                }
            }
            else if(e.PropertyName == "OperationId")
            {
                var gp = sender as GoodOperation;
                var operationList = Operations.Where(o => o.OperationId == gp.OperationId);
                if(operationList.Count() > 0)
                {
                    gp.Operation = operationList.ToList()[0];
                }

            }
            else if(e.PropertyName == "GoodId")
            {
                var gp = sender as GoodOperation;
                var goodList = Goods.Where(o => o.GoodId == gp.GoodId);
                if(goodList.Count() > 0)
                {
                    gp.Good = goodList.ToList()[0];
                }
            }
        }

        private Observable selectedRow;
        public ICommand SaveAllCommand => new RelayCommand(SaveAll);
        private void SaveAll()
        {
            using(GoodContext db = new GoodContext())
            {
                db.UpdateRange(Agents);
                db.UpdateRange(Goods);
                db.UpdateRange(Operations);
                db.UpdateRange(Warehouses);
                db.UpdateRange(StockBalances);
                db.UpdateRange(GoodOperations);
                db.AddRange(needAdd);
                db.RemoveRange(needDelete);
                db.SaveChangesAsync();
            }
        }

        public ICommand UpdateFromDBCommand => new RelayCommand(UpdateAll);


        public ObservableCollection<DataGridColumn> SelectedTemplate => selectedTemplate;
        private readonly ObservableCollection<DataGridColumn> selectedTemplate;
        private List<Observable> needAdd = new List<Observable>();
        public IEnumerable<object> LowLevel
        {
            get => lowLevel; set => Set(ref lowLevel, value);
        }
        private IEnumerable<object> lowLevel = new List<object>();

        public ICommand AddEntityCommand => new RelayCommand(AddEntity);
        private void AddEntity()
        {
            if(selectedVariant.Val is ObservableCollection<Agent> oc)
            {
                var new_val = new Agent();
                needAdd.Add(new_val);
                oc.Insert(0, new_val);
            }
            else if(selectedVariant.Val is ObservableCollection<Operation> operation)
            {
                var new_val = new Operation();
                needAdd.Add(new_val);
                operation.Insert(0, new_val);
            }
            else if(selectedVariant.Val is ObservableCollection<Good> good)
            {
                var new_val = new Good();
                needAdd.Add(new_val);
                good.Insert(0, new_val);
            }
            else if(selectedVariant.Val is ObservableCollection<GoodOperation> goodOperation)
            {
                var new_val = new GoodOperation();
                needAdd.Add(new_val);
                goodOperation.Insert(0, new_val);
            }
            else if(selectedVariant.Val is ObservableCollection<Warehouse> warehouse)
            {
                var new_val = new Warehouse();
                needAdd.Add(new_val);
                warehouse.Insert(0, new_val);
            }
            else if(selectedVariant.Val is ObservableCollection<StockBalance> stockBalance)
            {
                var new_val = new StockBalance();
                needAdd.Add(new_val);
                stockBalance.Insert(0, new_val);
            }
            SelectedGlobalRowIndex = 0;
        }
        private ResourceDictionary r;
        private void UpdateAll()
        {
            using(GoodContext db = new GoodContext())
            {
                Warehouses.Clear();
                Add(Warehouses, db.Warehouses.ToList());
                StockBalances.Clear();
                Add(StockBalances, db.StockBalances.ToList());
                Operations.Clear();
                Add(Operations, db.Operations.ToList());
                GoodOperations.Clear();
                Add(GoodOperations, db.GoodOperations.ToList());
                Goods.Clear();
                Add(Goods, db.Goods.ToList());
                Agents.Clear();
                Add(Agents, db.Agents.ToList());
                Add(AppropriateGoods, Goods);
                Reports.Clear();
                needDelete.Clear();
                needAdd.Clear();
                var query = from u in db.Operations
                            orderby u.DateOperation
                            select new Report()
                            {
                                FullPriceSum = u.GoodOperations.Select(o => o.FullPrice).Sum(),
                                PriceSum = u.GoodOperations.Select(o => o.Price).Sum(),
                                Agent = u.Agent.ToString(),
                                DateOperation = u.DateOperation,
                                OperationId = u.OperationId,
                                TypeOperation = u.TypeOperation
                            };
                Add(Reports, query);
                MinStockBalances.Clear();
                Add(MinStockBalances, StockBalances.Where(o => o.Count <= 50));
            }
        }
        private void Add<T>(ObservableCollection<T> src, IEnumerable<T> list)
        {
            foreach(var e in list)
            {
                src.Add(e);
            }
        }

        private List<Observable> needDelete = new List<Observable>();

        public ICommand DeleteGlobalOperation => new RelayCommand(DeleteGlobal);
        private void DeleteGlobal()
        {
            if(selectedRow != null)
            {
                needDelete.Add(selectedRow);
                if(SelectedRow is Agent a)
                {
                    Agents.Remove(a);
                }
                else if(SelectedRow is Operation op)
                {
                    Operations.Remove(op);
                }
                else if(SelectedRow is Good g)
                {
                    Goods.Remove(g);
                }
                else if(SelectedRow is GoodOperation go)
                {
                    GoodOperations.Remove(go);
                }
                else if(SelectedRow is Warehouse w)
                {
                    Warehouses.Remove(w);
                }
                else if(SelectedRow is StockBalance s)
                {
                    StockBalances.Remove(s);
                }
            }
        }

        private void InitCollections()
        {
            Warehouses = new ObservableCollection<Warehouse>();
            StockBalances = new ObservableCollection<StockBalance>();
            Operations = new ObservableCollection<Operation>();
            GoodOperations = new ObservableCollection<GoodOperation>();
            Goods = new ObservableCollection<Good>();
            Agents = new ObservableCollection<Agent>();
            Reports = new ObservableCollection<Report>();
        }

        public BaseVM(ResourceDictionary r, ObservableCollection<DataGridColumn> cols)
        {
            selectedTemplate = cols;
            this.r = r;
            InitCollections();
            UpdateAll();
            InitVariants();
        }

        public ObservableCollection<string> Suggestions
        {
            get;
            set;
        } = new ObservableCollection<string>();

        private string queryText;
        public string QueryText
        {
            get => queryText;
            set
            {
                if(selectedVariant.Val is ObservableCollection<Agent>)
                {
                    SearchAgent(value);
                }
                else if(selectedVariant.Val is ObservableCollection<Operation>)
                {
                    SearchOperation(value);
                }
                else if(selectedVariant.Val is ObservableCollection<Good>)
                {
                    SearchGood(value);
                }
                else if(selectedVariant.Val is ObservableCollection<GoodOperation>)
                {
                    SearchGoodOperation(value);
                }
                else if(selectedVariant.Val is ObservableCollection<Warehouse>)
                {
                    SearchWarehouse(value);
                }
                else if(selectedVariant.Val is ObservableCollection<StockBalance>)
                {
                    SearchStockBalance(value);
                }
                Set(ref queryText, value);
            }
        }
        public ICommand QuerySubmitted => new RelayCommand((o) =>
        {
            if(o is string param)
            {
                UpdateAll();
                if(selectedVariant.Val is ObservableCollection<Agent>)
                {
                     
                    var agents = SearchAgent(param);
                    for(int i = Agents.Count - 1; i >= 0; --i)
                    {
                        var ag = Agents[i];
                        if(!agents.Contains(ag))
                        {
                            Agents.RemoveAt(i);
                        }
                    }
                }
                else if(selectedVariant.Val is ObservableCollection<Operation>)
                {
                    var operations = SearchOperation(param);
                    for(int i = Operations.Count - 1; i >= 0; --i)
                    {
                        var ag = Operations[i];
                        if(!operations.Contains(ag))
                        {
                            Operations.RemoveAt(i);
                        }
                    }
                }
                else if(selectedVariant.Val is ObservableCollection<Good>)
                {
                    var goods = SearchGood(param);
                    for(int i = Goods.Count - 1; i >= 0; --i)
                    {
                        var ag = Goods[i];
                        if(!goods.Contains(ag))
                        {
                            Goods.RemoveAt(i);
                        }
                    }
                }
                else if(selectedVariant.Val is ObservableCollection<GoodOperation>)
                {
                    var goodOperations = SearchGoodOperation(param);
                    for(int i = GoodOperations.Count - 1; i >= 0; --i)
                    {
                        var ag = GoodOperations[i];
                        if(goodOperations.Contains(ag))
                        {
                            GoodOperations.RemoveAt(i);
                        }
                    }
                    
                }
                else if(selectedVariant.Val is ObservableCollection<Warehouse>)
                {
                    var warehouses = SearchWarehouse(param);
                    for(int i = Warehouses.Count - 1; i >= 0; --i)
                    {
                        var ag = Warehouses[i];
                        if(!warehouses.Contains(ag))
                        {
                            Warehouses.RemoveAt(i);
                        }
                    }
                }
                else if(selectedVariant.Val is ObservableCollection<StockBalance>)
                {
                    var stockBalances = SearchStockBalance(param);
                    for(int i = StockBalances.Count - 1; i >= 0; --i)
                    {
                        var ag = StockBalances[i];
                        if(!stockBalances.Contains(ag))
                        {
                            StockBalances.RemoveAt(i);
                        }
                    }
                }
            } 
        });

        public IEnumerable<Agent> SearchAgent(string param)
        {
            Suggestions.Clear();
            var candidates = Agents.Where(agent => Regex.IsMatch(agent.Name.ToLower(), param.ToLower()));
            var candidatesByType = Agents.Where(agent => Regex.IsMatch(agent.TypeAgent.ToLower(), param.ToLower()));
            var candidatesByCode = Agents.Where(agent => Regex.IsMatch(agent.Code.ToLower(), param.ToLower()));
            Add(Suggestions, candidates.Select(o => o.Name).Distinct());
            Add(Suggestions, candidatesByType.Select(o => o.TypeAgent).Distinct());
            Add(Suggestions, candidatesByCode.Select(o => o.Code).Distinct());
            return candidates.Union(candidatesByCode).Union(candidatesByType);
        }

        public IEnumerable<Good> SearchGood(string param)
        {
            Suggestions.Clear();
            var candidates = Goods.Where(o => Regex.IsMatch(o.Name.ToLower(), param.ToLower()));
            var candidatesByProducer = Goods.Where(o => Regex.IsMatch(o.Producer.ToLower(), param.ToLower()));
            var candidatesByDescription = Goods.Where(o => Regex.IsMatch(o.Description.ToLower(), param.ToLower()));
            var candidatesByMeasure = Goods.Where(o => Regex.IsMatch(o.Measure.ToLower(), param.ToLower()));
            var candidatesByPrice = Goods.Where(o => Regex.IsMatch(o.Price.ToString().ToLower(), param.ToLower()));

            Add(Suggestions, candidates.Select(o => o.Name).Distinct());
            Add(Suggestions, candidatesByProducer.Select(o => o.Producer).Distinct());
            Add(Suggestions, candidatesByDescription.Select(o => o.Description).Distinct());
            Add(Suggestions, candidatesByMeasure.Select(o => o.Measure).Distinct());
            Add(Suggestions, candidatesByPrice.Select(o => o.Price.ToString()).Distinct());
            
            return candidates.Union(candidatesByProducer).Union(candidatesByDescription)
                .Union(candidatesByProducer)
                .Union(candidatesByMeasure)
                .Union(candidatesByPrice);
        }

        public IEnumerable<Operation> SearchOperation(string param)
        {
            Suggestions.Clear();
            var candidates = Operations.Where(o => Regex.IsMatch(o.Agent.Name.ToLower(), param.ToLower()));
            var candidatesByTypeOperation = Operations.Where(o => Regex.IsMatch(o.TypeOperation.ToLower(), param.ToLower()));
            var candidatesByDateOperation = Operations.Where(o => Regex.IsMatch(o.DateOperation.ToString().ToLower(), param.ToLower()));
            
            Add(Suggestions, candidates.Select(o => o.Agent.Name.ToLower()).Distinct());
            Add(Suggestions, candidatesByTypeOperation.Select(o => o.TypeOperation).Distinct());
            Add(Suggestions, candidatesByDateOperation.Select(o => o.DateOperation.ToString()).Distinct());
            

            return candidates.Union(candidatesByTypeOperation).Union(candidatesByDateOperation);
        }

        public IEnumerable<Warehouse> SearchWarehouse(string param)
        {
            Suggestions.Clear();
            var candidates = Warehouses.Where(o => Regex.IsMatch(o.Name.ToLower(), param.ToLower()));
            var candidatesByLocation = Warehouses.Where(o => Regex.IsMatch(o.Location.ToLower(), param.ToLower()));
            
            Add(Suggestions, candidates.Select(o => o.Name).Distinct());
            Add(Suggestions, candidatesByLocation.Select(o => o.Location).Distinct());

            return candidates.Union(candidatesByLocation);
        }


        public IEnumerable<GoodOperation> SearchGoodOperation(string param)
        {
            Suggestions.Clear();
            var candidates = GoodOperations.Where(o => Regex.IsMatch(o.Count.ToString().ToLower(), param.ToLower()));
            var candidatesByLocation = GoodOperations.Where(o => Regex.IsMatch(o.Operation.TypeOperation.ToLower(), param.ToLower()));

            Add(Suggestions, candidates.Select(o => o.Count.ToString()).Distinct());
            Add(Suggestions, candidatesByLocation.Select(o => o.Operation.TypeOperation).Distinct());

            return candidates.Union(candidatesByLocation);
        }

        public IEnumerable<StockBalance> SearchStockBalance(string param)
        {
            Suggestions.Clear();
            var candidates = StockBalances.Where(o => Regex.IsMatch(o.Count.ToString().ToLower(), param.ToLower()));
            var candidatesByLocation = StockBalances.Where(o => Regex.IsMatch(o.Good.Name.ToLower(), param.ToLower()));
            var candidatesByWareHouse = StockBalances.Where(o => Regex.IsMatch(o.Warehouse.Name.ToLower(), param.ToLower()));

            Add(Suggestions, candidates.Select(o => o.Count.ToString()).Distinct());
            Add(Suggestions, candidatesByLocation.Select(o => o.Good.Name).Distinct());
            Add(Suggestions, candidatesByWareHouse.Select(o => o.Warehouse.Name).Distinct());
            return candidates.Union(candidatesByLocation).Union(candidatesByWareHouse);
        }

        private void InitVariants()
        {
            Variants = new List<ItemChoose>() {
                new ItemChoose() {Variant = "Агент", Val = Agents },
                new ItemChoose() {Variant = "Товар", Val = Goods },
                new ItemChoose() {Variant = "Товар в операції", Val = GoodOperations },
                new ItemChoose() {Variant = "Операція", Val = Operations },
                new ItemChoose() {Variant = "Залишок", Val = StockBalances },
                new ItemChoose() {Variant = "Склад", Val = Warehouses },
                new ItemChoose() {Variant = "Мінімальний залишок", Val = MinStockBalances},
                new ItemChoose() {Variant = "Звіт", Val = Reports}
            };
            dict = new Dictionary<string, ObservableCollection<DataGridColumn>>();
            dict.Add("Агент", new ObservableCollection<DataGridColumn>() {
                BCol("AgentId", true),
                BCol("TypeAgent", Constants.TypeAgent),
                BCol("Name"),
                BCol("Code")
            });
            dict.Add("Товар", new ObservableCollection<DataGridColumn>() {
                BCol("GoodId", true),
                BCol("Name"),
                BCol("Description"),
                BCol("Producer"),
                BCol("Measure", Constants.Measure),
                BCol("Percent"),
                BCol("Price")
            });
            dict.Add("Товар в операції", new ObservableCollection<DataGridColumn>() {
                BCol("GoodOperationId", true),
                BCol("GoodId", AppropriateGoods, "Name", "good"),
                BCol("WarehouseId", Warehouses, "Name","warehouse"),
                BCol("Count"),
                BCol("FullPrice", true),
                BCol("Price", true),
                BCol("Operation.TypeOperation", true),
                BCol("OperationId", Operations, "OperationId", "operation")
            });
            dict.Add("Операція", new ObservableCollection<DataGridColumn>() {
                BCol("OperationId", true),
                BCol("AgentId", Agents, "Name", "agent"),
                BCol("DateOperation"),
                BCol("TypeOperation", Constants.TypeOperation)
            });
            dict.Add("Мінімальний залишок", new ObservableCollection<DataGridColumn>()
            {
                BCol("StockBalanceId", true),
                BCol("GoodId", Goods, "Name", "goods"),
                BCol("WarehouseId", Warehouses, "Name", "warehouse"),
                BCol("Count")
            });
            dict.Add("Залишок", new ObservableCollection<DataGridColumn>() {
                BCol("StockBalanceId", true),
                BCol("GoodId", Goods, "Name", "goods"),
                BCol("WarehouseId", Warehouses, "Name", "warehouse"),
                BCol("Count")
            });
            dict.Add("Склад", new ObservableCollection<DataGridColumn>() {
                BCol("WarehouseId", true),
                BCol("Name"),
                BCol("Location")
            });
            dict.Add("Звіт", new ObservableCollection<DataGridColumn>() {
               BCol("OperationId", true),
               BCol("TypeOperation", true),
               BCol("DateOperation", true),
               BCol("Agent", true),
               BCol("PriceSum", true),
               BCol("FullPriceSum", true)
            });
            SelectedVariant = Variants[0];
        }

        private DataGridColumn BCol(string path, bool disableIt = false)
        {
            DataGridTextColumn column = new DataGridTextColumn() { CanUserSort = true};
            column.Binding = new Windows.UI.Xaml.Data.Binding() { Path = new PropertyPath(path) };
            column.Header = path;
            if(disableIt)
            {
                column.IsReadOnly = true;
                column.Binding.Mode = Windows.UI.Xaml.Data.BindingMode.OneWay;
            }
            return column;
        }
        private DataGridColumn BCol(string path, IEnumerable<string> variants, string header = null, bool disableIt = false)
        {
            DataGridComboBoxColumn column = new DataGridComboBoxColumn() { CanUserSort = true };
            column.Binding = new Windows.UI.Xaml.Data.Binding() { Path = new PropertyPath(path) };
            if(header == null)
            {
                header = path;
            }
            column.Header = header;
            column.ItemsSource = variants;
            if(disableIt)
            {
                column.IsReadOnly = true;
                column.Binding.Mode = Windows.UI.Xaml.Data.BindingMode.OneWay;
            }
            return column;
        }

        private DataGridColumn BCol(string path, IEnumerable<object> variants, string displayProp, string header, bool disableIt = false)
        {
            DataGridComboBoxColumn column = new DataGridComboBoxColumn() { CanUserSort = true };
            column.Binding = new Windows.UI.Xaml.Data.Binding() { Path = new PropertyPath(path) };

            column.Header = header;
            column.ItemsSource = variants;
            column.DisplayMemberPath = displayProp;
            if(disableIt)
            {
                column.IsReadOnly = true;
                column.Binding.Mode = Windows.UI.Xaml.Data.BindingMode.OneWay;
            }
            return column;
        }

        public ICommand SortCommand => new RelayCommand(Sort);
        private void Sort(object o)
        {
            if(o is DataGridColumnEventArgs e)
            {
                switch(selectedVariant.Variant)
                {
                    case "Товар":
                        switch(e.Column.Header.ToString())
                        {
                            case "GoodId":
                                SortData(
                                    Goods,
                                    (o1, o2) => o1.GoodId > o2.GoodId ? 1 : -1,
                                    (o1, o2) => o1.GoodId < o2.GoodId ? 1 : -1,
                                    e.Column
                                    );
                                break;
                            case "Name":
                                SortData(Goods,
                                    (o1, o2) => o1.Name.CompareTo(o2.Name),
                                    (o1, o2) => o1.Name.CompareTo(o2.Name)*(-1),
                                    e.Column
                                    );
                                break;
                            case "Producer":
                                SortData(Goods,
                                    (o1, o2) => o1.Producer.CompareTo(o2.Producer),
                                    (o1, o2) => o1.Producer.CompareTo(o2.Producer) * (-1),
                                    e.Column
                                    );
                                break;
                        }
                          
                            
                        break;
                }
            }
        }

        private void SortData<T>(
            ObservableCollection<T> collection, 
            Comparison<T> compDesc, 
            Comparison<T> compAsc,
            DataGridColumn col)
        {
            var arr = new T[collection.Count];
            collection.CopyTo(arr, 0);
            if(col.SortDirection == null || col.SortDirection == DataGridSortDirection.Descending)
            {
                Array.Sort(arr, compAsc);
                col.SortDirection = DataGridSortDirection.Ascending;
            }
            else
            {
                col.SortDirection = DataGridSortDirection.Descending;
                Array.Sort(arr, compDesc);
            } 
            Goods.Clear();
            Add(collection, arr);
            
        }

        /*BCol("GoodId", true),
                BCol("Name"),
                BCol("Description"),
                BCol("Producer"),
                BCol("Measure", Constants.Measure),
                BCol("Percent"),
                BCol("Price")
         * 
         * 
         * "Агент", Val = Agents },
                new ItemChoose() {Variant = "Товар", Val = Goods },
                new ItemChoose() {Variant = "Товар в операції", Val = GoodOperations },
                new ItemChoose() {Variant = "Операція", Val = Operations },
                new ItemChoose() {Variant = "Залишок", Val = StockBalances },
                new ItemChoose() {Variant = "Склад", Val = Warehouses },
                new ItemChoose() {Variant = "Мінімальний залишок", Val = MinStockBalances},
                new ItemChoose() {Variant = "Звіт", Val = Reports}
         */
    }
}
