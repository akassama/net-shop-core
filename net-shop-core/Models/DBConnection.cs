using Microsoft.EntityFrameworkCore;
using net_shop_core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net_shop_core.Models
{
    public class DBConnection : DbContext
    {

        public DBConnection()
        {
        }

        public DBConnection(DbContextOptions<DBConnection> options) : base(options)
        {
        }


        public DbSet<AccountsModel> Accounts { get; set; }
        public DbSet<ProductsModel> Products { get; set; }
        public DbSet<CategoriesModel> Categories { get; set; }
        public DbSet<OrdersModel> Orders { get; set; }
        public DbSet<OrderDetailsModel> OrderDetails { get; set; }
        public DbSet<ContactMessageModel> ContactMessage { get; set; }
        public DbSet<LocationsModel> Locations { get; set; }
        public DbSet<PaymentsModel> Payments { get; set; }
        public DbSet<ProductTagsModel> ProductTags { get; set; }
        public DbSet<SubscribersModel> Subscribers { get; set; }
        public DbSet<LoginInfoModel> LoginInfo { get; set; }
        public DbSet<ProductImagesModel> ProductImages { get; set; }
        public DbSet<ProductVideosModel> ProductVideos { get; set; }
        public DbSet<SiteDataLookupModel> SiteDataLookup { get; set; }
        public DbSet<ProductColorsModel> ProductColors { get; set; }
        public DbSet<ProductSizeModel> ProductSize { get; set; }
        public DbSet<ProductViewsModel> ProductViews { get; set; }
        public DbSet<PopularThisWeekModel> PopularThisWeek { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=DESKTOP-O81UVC0\\SQLEXPRESS;database=NetShopCore;trusted_connection=true;");
        }
    }
}
