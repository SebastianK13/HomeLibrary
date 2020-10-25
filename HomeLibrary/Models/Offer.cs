using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HomeLibrary.Models
{
    public class Offer
    {
        public int OfferID { get; set; }
        public bool Closed { get; set; }
        [ForeignKey("Swap")]
        public int SwapID { get; set; }
        public string ProposeDesc { get; set; }
        public decimal ProposingPrice { get; set; }
        public bool Notification { get; set; }
        public string CurrencyCode { get; set; }
        [ForeignKey("Profile")]
        public int ProfileID { get; set; }
        public virtual BasicProfileInfoModels Profile { get; set; }
        public virtual Swap Swap { get; set; }
    }
}