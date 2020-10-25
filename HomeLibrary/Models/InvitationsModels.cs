using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HomeLibrary.Models
{
    public class Invitations
    {
        [Key]
        public int InvitationID { get; set; }
        public bool Notification { get; set; }
        public int InvitationSender { get; set; }
        public bool Accept { get; set; }
        public int InvitationReceiver { get; set; }
        [ForeignKey("InvitationSender")]
        public virtual BasicProfileInfoModels Sender { get; set; }
        [ForeignKey("InvitationReceiver")]
        public virtual BasicProfileInfoModels Receiver { get; set; }
    }
}