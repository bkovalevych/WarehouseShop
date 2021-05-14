using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace WarehouseShop
{
    using Entities;
    using ViewModels;
    using Helpers;

    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public BaseVM vm;
        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;
            vm = new BaseVM(Resources, mainGrid.Columns);
            //AddData();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

        }
        public void AddData()
        {
            using(GoodContext db = new GoodContext())
            {
                List<Agent> agents = new List<Agent>() {
                    new Agent() { Code = "0000000001", Name = "ТОВ АГРОМОЛ", TypeAgent = Constants.TypeAgentLawEntity },
                    new Agent() { Code = "0000000002", Name = "Фабрика Господар", TypeAgent = Constants.TypeAgentLawEntity }
                };
                db.AddRange(agents);
                List<Good> goods = new List<Good>() {
                    new Good() { Description = "default", Measure = Constants.MeasureCount,
                        Name="Відро 10л залізне",
                        Percent = Constants.DefaultPercent,
                        Price = 20.0,
                        Producer = "ТОВ АГРОМОЛ",
                    },
                    new Good() {
                        Description = "default",
                        Measure = Constants.MeasureCount,
                        Name = "Граблі пластмасові",
                        Percent = Constants.DefaultPercent,
                        Price = 100.0,
                        Producer = "ТОВ АГРОМОЛ",
                    },
                    new Good() {
                        Description = "default",
                        Measure = Constants.MeasureVeight,
                        Name = "Добриво мінеральне",
                        Percent = Constants.DefaultPercent,
                        Price = 25.0,
                        Producer = "Фабрика Господар",
                    }, new Good() {
                        Description = "default",
                        Measure = Constants.MeasureVeight,
                        Name = "Добриво синтетичне",
                        Percent = Constants.DefaultPercent,
                        Price = 43.0,
                        Producer = "Фабрика Господар",
                    }
                };
                db.AddRange(goods);
                List<Warehouse> warehouses = new List<Warehouse>() {
                    new Warehouse() {
                        Name = "warehouse 1",
                        Location = "location 1"
                    },
                    new Warehouse() {
                        Name = "warehouse 2",
                        Location = "location 2"
                    }
                };
                db.AddRange(warehouses);
                db.SaveChanges();
                db.AddRange(new StockBalance()
                {
                    Count = 300,
                    Warehouse = warehouses[0],
                    Good = goods[0]
                }, new StockBalance()
                {
                    Count = 200,
                    Warehouse = warehouses[0],
                    Good = goods[1]
                }, new StockBalance()
                {
                    Count = 1000,
                    Warehouse = warehouses[0],
                    Good = goods[2]
                }, new StockBalance()
                {
                    Count = 30,
                    Warehouse = warehouses[1],
                    Good = goods[3]
                });
                List<Operation> operations = new List<Operation>() {
                    new Operation() {
                        Agent = agents[0],
                        DateOperation = new DateTime(DateTime.Now.Ticks - 1000 * 360 * 24 * 5),
                        TypeOperation = Constants.TypeOperationGain,
                    }, new Operation() {
                        Agent = agents[1],
                        DateOperation = new DateTime(DateTime.Now.Ticks - 1000 * 360 * 24 * 4),
                        TypeOperation = Constants.TypeOperationGain,
                    }, new Operation() {
                        Agent = agents[0],
                        DateOperation = new DateTime(DateTime.Now.Ticks - 1000 * 360 * 24 * 3),
                        TypeOperation = Constants.TypeOperationSold,
                    }
                };
                db.AddRange(operations);
                db.SaveChanges();
                db.AddRange(new GoodOperation()
                {
                    Count = 1,
                    FullPrice = (goods[0].Percent + 1) * goods[0].Price,
                    Price = goods[0].Price,
                    Operation = operations[0],
                    Warehouse = warehouses[0],
                    Good = goods[0]
                }, new GoodOperation()
                {
                    Count = 3,
                    FullPrice = (goods[0].Percent + 1) * goods[0].Price * 3,
                    Price = goods[0].Price * 3,
                    Operation = operations[1],
                    Warehouse = warehouses[0],
                    Good = goods[0]
                }, new GoodOperation()
                {
                    Count = 3,
                    FullPrice = (goods[1].Percent + 1) * goods[1].Price * 3,
                    Price = goods[0].Price * 3,
                    Operation = operations[1],
                    Warehouse = warehouses[1],
                    Good = goods[1]
                }, new GoodOperation()
                {
                    Count = 3.5,
                    FullPrice = (goods[2].Percent + 1) * goods[2].Price * 3.5,
                    Price = goods[2].Price * 3.5,
                    Operation = operations[2],
                    Warehouse = warehouses[1],
                    Good = goods[2]
                }, new GoodOperation()
                {
                    Count = 10.5,
                    FullPrice = (goods[3].Percent + 1) * goods[3].Price * 10.5,
                    Price = goods[3].Price * 10.5,
                    Operation = operations[2],
                    Warehouse = warehouses[1],
                    Good = goods[3]
                });
                db.SaveChanges();
            }
        }

        private async void MakeReport(object sender, RoutedEventArgs e)
        {
            await ToExcel.Some(vm.Reports);
        }


    }
}
