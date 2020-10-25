using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeLibrary.Models
{
    public class BookLocation
    {
        public Book Book{get;set;}
        public Location Location { get; set; }
    }
}