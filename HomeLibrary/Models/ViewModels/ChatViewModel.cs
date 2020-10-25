using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeLibrary.Models.ViewModels
{
    public class ChatViewModel
    {
        private List<ChatViewModel> tempFriendList;
        public ChatViewModel()
        {
        }

        public ChatViewModel(List<BasicProfileInfoModels>userFriends)
        {
            tempFriendList = new List<ChatViewModel>();
            foreach (var f in userFriends)
            {
                tempFriendList.Add(new ChatViewModel()
                {
                    FriendID = f.ProfileID,
                    Name = f.Nickname,
                    PictureSource = f.ProfilePic
                });
            }
            Friends = tempFriendList;
        }

        [Key]
        public int FriendID { get; set; }
        public string Name { get; set; }
        public string PictureSource { get; set; }
        public List<ChatViewModel> Friends { get; set; }
    }
}