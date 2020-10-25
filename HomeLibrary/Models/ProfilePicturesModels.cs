using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeLibrary.Models
{
    public class ProfilePicturesModels
    {
        [Key]
        public int PictureID { get; set; }
        public string PicturePath { get; set; }
        public int ProfileID { get; set; }
        public virtual BasicProfileInfoModels Profile { get; set; }
    }
}