using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeLibrary.Models
{
    public class StateModels
    {
        [Key]
        public int StateID { get; set; }
        public bool OverallStatus { get; set; }
        public bool LendStatus { get; set; }
        public bool BorrowedStatus { get; set; }
    }
}