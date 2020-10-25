using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HomeLibrary.Models
{
    public class SwapPropose
    {
        [Key,ForeignKey("Book")]
        public int ProposeID { get; set; }
        [ForeignKey("Swap")]
        public int SwapID { get; set; }
        public bool Closed { get; set; }
        public bool Notification { get; set; }
        public virtual Swap Swap { get; set; }
        public virtual Book Book { get; set; }
    }
}