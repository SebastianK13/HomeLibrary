using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HomeLibrary.Models
{
    public class Location
    {
        [Key]
        public int LocationID { get; set; }
        [Required(ErrorMessage = "Complete bookstand field")]
        [MaxLength(10, ErrorMessage ="Maximum numbers of characters is 10")]
        public string BookstandDesignation { get; set; }
        [Required(ErrorMessage = "Complete shelf number")]
        public int ShelfNumber { get; set; }
        [Required(ErrorMessage = "Complete book position")]
        public int BookPosition { get; set; }
    }
}