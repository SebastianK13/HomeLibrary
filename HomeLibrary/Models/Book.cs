using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace HomeLibrary.Models
{
    public class Book
    {
        public int BookID { get; set; }
        [Required(ErrorMessage = "Complete title")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Complete category")]
        public string Category { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Complete author")]
        public string Author { get; set; }
        [Required(ErrorMessage = "Complete language")]
        public string Language { get; set; }
        [Required(ErrorMessage = "Complete pages")]
        public int Pages { get; set; }
        public int LocationID { get; set; }
        public string UserName { get; set; }
        public int Rate { get; set; }
        [ForeignKey("Profile")]
        public int ProfileID { get; set; }
        [Required]
        [ForeignKey("LocationID")]
        public virtual Location Location { get; set; }
        public string ImageSource { get; set; }
        public int StateID { get; set; }
        [Required]
        [ForeignKey("StateID")]
        public virtual StateModels StateModels { get; set; }
        public virtual Swap Swap { get; set; }
        [ForeignKey("BookID")]
        public virtual SwapPropose Propose { get; set; }
        public virtual BasicProfileInfoModels Profile { get; set; }
    }
    //public class BookMap : EntityTypeConfiguration<Book>
    //{
    //    public BookMap()
    //    {
    //        this.HasKey(c => c.BookID);
    //        this.Property(c => c.BookID)
    //            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
    //        this.HasRequired(c1 => c1.Swap).WithRequiredPrincipal(c2 => c2.Book);
    //    }
    //}
}