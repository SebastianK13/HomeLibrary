using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeLibrary.Models
{
    public class BookDetail
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Complete title")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Complete category")]
        public string Category { get; set; }
        [Required(ErrorMessage = "Complete author")]
        public string Author { get; set; }
        [Required(ErrorMessage = "Complete language")]
        public string Language { get; set; }
        [Required(ErrorMessage = "Complete pages")]
        public int Pages { get; set; }
        [Required(ErrorMessage = "Complete bookstand field")]
        [MaxLength(10, ErrorMessage ="Limit of characters is 10")]
        public string BookstandDesignation { get; set; }
        [Required(ErrorMessage = "Complete shelf number")]
        public int ShelfNumber { get; set; }
        [Required(ErrorMessage = "Complete book position")]
        public int BookPosition { get; set; }
        public string Description { get; set; }
    }
}