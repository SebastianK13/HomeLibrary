using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HomeLibrary.Models
{
    public class Friends
    {
        [Key]
        public int FriendID { get; set; }
        public int ProfileID { get; set; }
        public string Owner { get; set; }
        public bool Block { get; set; }
        public int InvitationID { get; set; }
        //[ForeignKey("Owner")]
        public virtual BasicProfileInfoModels Profile { get; set; }
        [Required]
        public virtual Invitations Invitations { get; set; }
    }
}