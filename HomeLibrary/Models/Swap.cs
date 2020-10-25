using HomeLibrary.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace HomeLibrary.Models
{
    public class Swap
    {
        [ForeignKey("Book")]
        public int SwapID { get; set; }
        public bool Closed { get; set; }
        public int OwnerID { get; set; }
        public bool Away { get; set; }
        public bool SwapB { get; set; }
        public string DescSwap { get; set; }
        public bool Sell { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public int OfferTransaction { get; set; }
        public int SwapTransaction { get; set; }
        public virtual ICollection<Offer> Offers{get;set;}
        public virtual ICollection<SwapPropose> Proposes{get;set;}
        public virtual Book Book { get; set; }
    }
    //public class SwapMap : EntityTypeConfiguration<Swap>
    //{
    //    public SwapMap()
    //    {
    //        this.HasKey(c => c.SwapID);
    //        this.Property(c => c.SwapID)
    //            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
    //        this.HasRequired(c1 => c1.Book).WithRequiredPrincipal(c2 => c2.Swap);
    //    }
    //}
}