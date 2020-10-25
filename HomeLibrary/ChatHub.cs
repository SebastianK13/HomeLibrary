using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using HomeLibrary.Services.Chat;
using Microsoft.AspNet.SignalR;

namespace HomeLibrary
{
    public class ChatHub : Hub
    {
        //public void Send(string name, string message)
        //{
        //    string user = HttpContext.Current.User.Identity.Name;
        //    Clients.All.addNewMessageToPage(name, message);
        //}
        [Authorize]
        public Task SendPrivateMessage(int friendID, string message)
        {
            ChatHubServices chs = new ChatHubServices();
            string user = chs.MessageManager(friendID, message);
            string basicSenderInfo = chs.SenderInfoManager();

            return Clients.User(user).SendAsync(message, friendID, basicSenderInfo);
        }
        [Authorize]
        public void LoadMessages(int profileID)
        {
            ChatHubServices chs = new ChatHubServices();
            var messages = chs.LoadMessagesManager(profileID);
            int MyID = chs.GetProfileID();
            string user = HttpContext.Current.User.Identity.Name;
            Clients.User(user).loadMessagesToView(messages, profileID, MyID);
        }

    }
}