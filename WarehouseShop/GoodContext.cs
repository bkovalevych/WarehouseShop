using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseShop
{
    using Entities;
    using System.IO;
    using Windows.Storage;

    public class GoodContext : DbContext
    {
        public DbSet<Agent> Agents
        {
            get; set;
        }
        public DbSet<Good> Goods
        {
            get; set;
        }
        public DbSet<GoodOperation> GoodOperations
        {
            get; set;
        }
        public DbSet<Operation> Operations
        {
            get; set;
        }
        public DbSet<StockBalance> StockBalances
        {
            get; set;
        }
        public DbSet<Warehouse> Warehouses
        {
            get; set;
        }

        public GoodContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=" + Path.Combine(ApplicationData.Current.LocalFolder.Path, "goods.db"));
        }
    }
}
