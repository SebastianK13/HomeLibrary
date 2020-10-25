using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HomeLibrary.Models
{
    public class Sort
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public bool LastStatus { get; set; }
    }
}