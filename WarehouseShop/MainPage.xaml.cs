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
    using Microsoft.Toolkit.Uwp.UI.Controls;
    using System.Collections.ObjectModel;

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
            DeleteAll();
            AddData();
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
                    new Agent() { Code = "0000000002", Name = "Фабрика Господар", TypeAgent = Constants.TypeAgentLawEntity },
                    new Agent() { Code = "0000000003", Name = "ФОП Лукачі", TypeAgent = Constants.TypeAgentPhisicEntity },
                    new Agent() { Code = "0000000004", Name = "Фірма \"МЕД\"", TypeAgent = Constants.TypeAgentLawEntity },
                    new Agent() { Code = "0000000005", Name = "ТОВ \"Хозяйкин дом\"", TypeAgent = Constants.TypeAgentPhisicEntity }
                };
                db.AddRange(agents);
                List<Good> goods = new List<Good>() {
                    new Good() { 
                        Description = "Відро містке, ручка залізна", 
                        Measure = Constants.MeasureCount,
                        Name="Відро 10л залізне",
                        Percent = Constants.DefaultPercent,
                        Price = 20.0,
                        Producer = "Фабрика транш",
                    },
                    new Good() {
                        Description = "Граблі гарної комплектації з держаком",
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
                    }, new Good() {
                        Description = "Чорний колір, 8мм",
                        Measure = Constants.MeasureVeight,
                        Name = "Цвях залізний 8мм",
                        Percent = Constants.DefaultPercent,
                        Price = 1.0,
                        Producer = "Залізометробуд",
                    }, new Good() {
                        Description = "Ланцюг 50мм одне кільце, ціна за метр",
                        Measure = Constants.MeasureWidth,
                        Name = "Ланцюг 50мм",
                        Percent = Constants.DefaultPercent,
                        Price = 2.0,
                        Producer = "Залізометробуд",
                    }, new Good() {
                        Description = "Мотузка, 20мм діаметр, 4 волокна в основі, лляна",
                        Measure = Constants.MeasureWidth,
                        Name = "Мотузка 20мм",
                        Percent = Constants.DefaultPercent,
                        Price = 3.50,
                        Producer = "Добра марка",
                    }, new Good() {
                        Description = "Петлі для дверей, звичайні",
                        Measure = Constants.MeasureCount,
                        Name = "Петлі",
                        Percent = Constants.DefaultPercent,
                        Price = 21.50,
                        Producer = "Залізометробуд",
                    }
                };
                db.AddRange(goods);
                List<Warehouse> warehouses = new List<Warehouse>() {
                    new Warehouse() {
                        Name = "Склад 1",
                        Location = "Точка 1"
                    },
                    new Warehouse() {
                        Name = "Склад 2",
                        Location = "Точка 2, схід"
                    }
                };
                db.AddRange(warehouses);
                db.SaveChanges();
                List<StockBalance> stocks = new List<StockBalance>();
                for(int war = 0; war < 2; ++war)
                {
                    int index = 1; 
                    foreach(var g in goods)
                    {
                        var new_sb = new StockBalance()
                        {
                            Count = 10 * index * (war + 1),
                            Warehouse = warehouses[war],
                            Good = g
                        };
                        db.Add(new_sb);
                        stocks.Add(new_sb);
                        index = (index + 1) % 6; 
                    }
                    
                }
                List<Operation> operations = new List<Operation>();
                for(int index = 0; index < 10; ++index)
                {
                    for(int indexAgent = 0; indexAgent < agents.Count; ++indexAgent)
                    {
                        operations.Add(new Operation()
                        {
                            Agent = agents[indexAgent],
                            DateOperation = new DateTime(DateTime.Now.Ticks - 1000 * 360 * 24 * (30 - index + indexAgent)),
                            TypeOperation = index % 2 == 0 ? Constants.TypeOperationSold : Constants.TypeOperationGain,
                        });
                    }
                }

                db.AddRange(operations);
                db.SaveChanges();
                for(int index = 0; index < operations.Count; ++index)
                {
                    var op = operations[index];
                    int countProducts = index % 5;
                    for(int gpIndex = 0; gpIndex < countProducts; ++gpIndex)
                    {
                        var sb = stocks[(gpIndex + index) % stocks.Count];
                        var g = sb.Good;
                        var count = gpIndex * 2 + index;
                        if(op.TypeOperation == Constants.TypeOperationGain)
                        {
                            count += 10 * gpIndex;
                            sb.Count += count;
                        }
                        db.Add(new GoodOperation()
                        {
                            Count = count,
                            FullPrice = (g.Percent + 1) * g.Price * count,
                            Price = g.Price * count,
                            Operation = op,
                            Warehouse = sb.Warehouse,
                            Good = g
                        });
                    }
                }
                db.UpdateRange(stocks);
                db.SaveChanges();
            }
        }
        private void DeleteAll()
        {
            using(var db = new GoodContext())
            {
                db.RemoveRange(db.StockBalances);
                db.RemoveRange(db.Warehouses);
                db.RemoveRange(db.GoodOperations);
                db.RemoveRange(db.Goods);
                db.RemoveRange(db.Agents);
                db.RemoveRange(db.Operations);
                db.SaveChanges();
            }
        }

        private async void MakeReport(object sender, RoutedEventArgs e)
        {
            await ToExcel.Some(vm.Reports);
        }

       
    }
}
