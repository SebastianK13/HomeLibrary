using HomeLibrary.DAL;
using HomeLibrary.Models;
using HomeLibrary.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeLibrary.Services.Chat
{
    public class ChatServices
    {
        private HomeLibraryConnection db;
        public List<ChatViewModel> CreateFriendsList()
        {
            db = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            int profileID = db.Profile.Where(n => n.Name == name).Select(i=>i.ProfileID).SingleOrDefault();
            var friendsIDList = db.Friends
                .Where(i => i.ProfileID == profileID)
                .Select(i => i.Owner)
                .ToList();
            var userFriends = db.Profile
                .Where(i=> friendsIDList.Contains(i.Name))
                .ToList();

            ChatViewModel cvm = new ChatViewModel(userFriends);

            return cvm.Friends;
        }
    }
    public class ChatHubServices
    {
        private HomeLibraryConnection db;
        public List<Messages> LoadMessagesManager(int friendID)
        {
            var messageList = LoadMessages(friendID);

            return messageList;
        }
        private List<Messages> LoadMessages(int friendID)
        {
            db = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            int userID = db.Profile.Where(n => n.Name == name).Select(i => i.ProfileID).SingleOrDefault();
            var messageList = db.Messages.Where(i => (i.OwnerID == userID && i.ReceiverID == friendID)
                ||(i.OwnerID == friendID && i.ReceiverID == userID)).ToList();

            return messageList;
        }
        public int GetProfileID()
        {
            return GetMyID();
        }
        private int GetMyID()
        {
            db = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            int myID = db.Profile.Where(n => n.Name == name).Select(i => i.ProfileID).FirstOrDefault();

            return myID;
        }
        public string MessageManager(int friendID, string message)
        {
            SendMsgToFriend(friendID, message);

            return GetFriendName(friendID);
        }
        private void SendMsgToFriend(int friendID, string message)
        {
            db = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            var SenderID = db.Profile
                .Where(n => n.Name == name)
                .Select(i => i.ProfileID)
                .SingleOrDefault();
            Messages newMessage = new Messages
            {
                OwnerID = SenderID,
                Date = DateTime.Now,
                Content = message,
                ReceiverID = friendID
            };
            db.Messages.Add(newMessage);
            db.SaveChanges();
        }
        private string GetFriendName(int friendID)
        {
            db = new HomeLibraryConnection();
            string name = db.Profile.Where(i => i.ProfileID == friendID).Select(n=>n.Name).FirstOrDefault();
            
            return name;
        }
        public string SenderInfoManager() 
        {
            return GetSenderInfo();
        }
        private string GetSenderInfo()
        {
            db = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            var info = db.Profile
                .Where(n => n.Name == name)
                .Select(i => new {i.ProfileID, i.Nickname, i.ProfilePic})
                .SingleOrDefault();
            string senderInfo = info.ProfileID + "," + info.Nickname + "," + info.ProfilePic;

            return senderInfo;
        }

    }
}