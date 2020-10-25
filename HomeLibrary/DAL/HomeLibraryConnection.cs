using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using HomeLibrary.Models;

namespace HomeLibrary.DAL
{
    public class HomeLibraryConnection : IdentityDbContext<ApplicationUser>
    {
        public HomeLibraryConnection() : base("HomeLibraryConnection", throwIfV1Schema: false)
        {

        }
        //static HomeLibraryConnection()
        //{
        //    Database.SetInitializer<HomeLibraryConnection>(new HomeLibraryConnection());
        //}
        public DbSet<Book> Books { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Sort> Sorts { get; set; }
        public DbSet<BookDetail> BookDetails { get; set; }
        public DbSet<StateModels> BookState { get; set; }
        public DbSet<BasicProfileInfoModels> Profile { get; set; }
        public DbSet<ProfilePicturesModels> Pictures { get; set; }
        public DbSet<Friends> Friends { get; set; }
        public DbSet<Invitations> Invitations { get; set; }
        public DbSet<Swap> Swaps { get; set; }
        public DbSet<SwapPropose> Proposes { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Messages> Messages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

        }

        public System.Data.Entity.DbSet<HomeLibrary.Models.ViewModels.ChatViewModel> ChatViewModels { get; set; }
    }

}