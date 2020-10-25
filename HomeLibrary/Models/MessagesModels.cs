using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeLibrary.Models
{
    public class Messages
    {
        [Key]
        public int MessageID { get; set; }
        public int OwnerID { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }
        public int ReceiverID { get; set; }
    }
}