using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeLibrary.Models
{
    public class BasicProfileInfoModels
    {
        [Key]
        public int ProfileID { get; set; }
        public string Name { get; set; }
        [MaxLength(10, ErrorMessage = "Limit of characters is 10")]
        public string Nickname { get; set; }
        public string Country { get; set; }
        public int Age { get; set; }
        public string City { get; set; }
        [MaxLength(120, ErrorMessage = "Limit of characters is 120")]
        public string Quote { get; set; }
        public string FavouriteBookCategory { get; set; }
        public string FavouriteBook { get; set; }
        public string ProfilePic { get; set; }
        public string BackgroundPic { get; set; }
    }
}