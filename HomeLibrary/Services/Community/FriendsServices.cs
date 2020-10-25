using HomeLibrary.DAL;
using HomeLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace HomeLibrary.Services.Community
{
    public class FriendsServices
    {
        private string searchV;
        private string originalV;
        private HomeLibraryConnection db;
        private int profileID;
        private string operation;
        private int invID;
        private List<int> friendsMatches;
        public async Task<dynamic> FindUser(string searchValue)
        {
            dynamic model = new ExpandoObject();
            searchV = searchValue;
            originalV = searchValue;
            var profileList = await CreateUsersList();
            model.userList = profileList;
            if(profileList != null && profileList.Count>0)
            {
                model.friendList = await AllreadyFriend(profileList);
                model.invitedList = await AllreadyInvited(profileList);
            }

            return model;
        }
        private async Task<List<BasicProfileInfoModels>> CreateUsersList()
        {
            IEnumerable<BasicProfileInfoModels> model = new List<BasicProfileInfoModels>();
            IEnumerable<BasicProfileInfoModels> queriesTemp = new List<BasicProfileInfoModels>();
            string user = HttpContext.Current.User.Identity.Name;
            Regex newSearchV = new Regex(@"[^a-zA-ZĄ-Żą-ż\s]");
            searchV = newSearchV.Replace(searchV, "");
            string[] searchVArray = searchV.Split(' ');
            db = new HomeLibraryConnection();
            var profiles = db.Profile;
            foreach(var v in searchVArray)
            {
                queriesTemp = await profiles.Where(n => n.Nickname.Contains(v)).ToListAsync();
                model = model.Union(queriesTemp);
            }
            var currentProfile = await profiles.Where(n => n.Name == user).SingleOrDefaultAsync();
            var temp = model.ToList();

            if (temp.Contains(currentProfile))
            {                
                temp.Remove(currentProfile);
            }
                       
            var lStrike = await profiles.Where(n => n.Nickname == originalV).SingleOrDefaultAsync();

            if (temp.Contains(lStrike))
            {
                temp.Remove(lStrike);
            }

            if (currentProfile == lStrike)
            {
                temp.Insert(0, null);
            }
            else if(lStrike!=null)
            {
                temp.Insert(0, lStrike);
            }
            
            //if (temp.Contains(currentProfile))
            //{
            //    index = temp.FindIndex(i => i.Name == user);
            //    temp.Remove(currentProfile);
            //    if (index == 0)
            //        temp.Insert(0, null);
            //}
            
            //n.Nickname.Contains(searchV) ||

            //var model = await profiles.Where(n => (n.Nickname.Contains(searchV)) || (n.Nickname.Contains(searchV))).ToListAsync();

            return temp;
        }
        //Check if user is not your friend allready.
        private async Task<List<bool>> AllreadyFriend(List<BasicProfileInfoModels> model)
        {
            bool removed = false;
            if (model != null && model.Count>0)
            {
                if (model[0] == null)
                {
                    model.RemoveAt(0);
                    removed = true;
                }

                int range = model.Count();
                string name = HttpContext.Current.User.Identity.Name;
                var friends = await db.Friends.Where(n => n.Owner == name).ToListAsync();
                var isFriend = Enumerable.Repeat(false, range).ToList();
                if (CheckIfContainsFriend(friends, model))
                {
                    foreach (var f in friendsMatches)
                    {
                        isFriend[model.FindIndex(c => c.ProfileID == f)] = true;
                    }

                    if (removed)
                    {
                        model.Insert(0, null);
                        isFriend.Insert(0, false);
                    }
                }

                return isFriend;
            }

            return null;
        }
        private bool CheckIfContainsFriend(List<Friends> friends, List<BasicProfileInfoModels> model)
        {
            List<int> IDs = new List<int>();
            foreach(var f in friends)
            {
                IDs.Add(model.Where(m => m.ProfileID == f.ProfileID).Select(p => p.ProfileID).FirstOrDefault());
            }
            IDs.RemoveAll(e => e == 0);
            if (IDs.Count > 0)
            {
                friendsMatches = IDs;
                return true;
            }
                

            return false;
        }
        private async Task<List<bool>> AllreadyInvited(List<BasicProfileInfoModels> model)
        {
            int range = model.Count();
            string name = HttpContext.Current.User.Identity.Name;
            var isInvited = Enumerable.Repeat(false, range).ToList();
            int profileID = await db.Profile.Where(n=>n.Name == name).Select(p=>p.ProfileID).SingleAsync();
            var invitations = await db.Invitations.Where(r=>(r.InvitationSender == profileID || r.InvitationReceiver == profileID) && r.Accept == false).ToListAsync();

            List<int> indexes = new List<int>();

            foreach (var i in invitations)
            {
                if (i.Accept == false)
                {
                    indexes.Add(model.FindIndex(p => (p.ProfileID == i.InvitationReceiver) || (p.ProfileID == i.InvitationSender)));
                    //isInvited[model.FindIndex(p => (p.ProfileID == i.InvitationReceiver)||(p.ProfileID == i.InvitationSender))] = true;
                }
            }
            indexes.RemoveAll(i => i == -1);
            if (indexes.Count > 0)
            {
                foreach(var ind in indexes)
                {
                    isInvited[ind] = true;
                }
            }

            return isInvited;
        }
        public async Task<bool> AddFriendManager(string profileId)
        {
            string[] valueArray = profileId.Split(',');
            profileID = Convert.ToInt32(valueArray[0]);
            operation = valueArray[1];
            bool result = await ProfileExist();
            if(result)
            {
                switch (operation)
                {
                    case "add":
                        result = await UpdateFriendList();
                        break;
                    case "remove":
                        result = await DropFriend();
                        result = await DeclineInv();
                        break;
                    default:
                        break;
                }
            }

            return result;
        }
        private async Task<bool> ProfileExist()
        {
            db = new HomeLibraryConnection();
            var result = await db.Profile.Where(i => i.ProfileID == profileID).SingleAsync() != null;

            return result;
        }
        private async Task<bool> UpdateFriendList()
        {
            Friends friend = new Friends {
                ProfileID = profileID,
                Owner = HttpContext.Current.User.Identity.Name,
                Block = false,
            };
            db.Friends.Add(friend);
            bool result = await db.SaveChangesAsync() > 0;

            return result;
        }
        private async Task<bool> DropFriend()
        {
            string name = HttpContext.Current.User.Identity.Name;
            var ownerID = await db.Profile.Where(n => n.Name == name).Select(p => p.ProfileID).SingleAsync();
            var friendName = await db.Profile.Where(p=>p.ProfileID == profileID).Select(n=>n.Name).SingleAsync();

            var friend = await db.Friends
                .Where(i => (i.ProfileID == profileID && i.Owner == name) || (i.ProfileID == ownerID && i.Owner == friendName))
                .ToListAsync();
            foreach(var f in friend)
            {
                db.Friends.Remove(f);
                invID = f.InvitationID;
            }


            bool result = await db.SaveChangesAsync() > 0;

            return false;
        }
        private async Task<bool> RemoveInvitationAsync()
        {
            var inv = await db.Invitations.Where(i => i.InvitationID == invID).FirstOrDefaultAsync();
            bool result = false;
            db.Invitations.Remove(inv);
            result = await db.SaveChangesAsync() > 0;

            return result;
        }
        public async Task<bool> IvitationManager(string postParameters)
        {
            string[] valueArray = postParameters.Split(',');
            profileID = Convert.ToInt32(valueArray[0]);
            operation = valueArray[1];
            bool result = await ProfileExist();
            if (result)
            {
                switch(operation)
                {
                    case "add":
                        result = await Invitation(profileID);
                        break;
                    case "remove":
                        result = await DropFriend();
                        break;
                    case "repeat":
                        result = await InvitationRepeat(profileID);
                        break;
                    default:
                        break;
                }
            }

            return result;
        }
        private async Task<bool> Invitation(int idReceiver)
        {
            db = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            int profileID = await db.Profile.Where(n => n.Name == name).Select(i => i.ProfileID).SingleAsync();
            Invitations inv = new Invitations {
                Notification = true,
                InvitationSender = profileID,
                InvitationReceiver = idReceiver,
                Accept = false
            };
            db.Invitations.Add(inv);
            bool result = await db.SaveChangesAsync() > 0;

            return result;
        }
        private async Task<bool> InvitationRepeat(int idReceiver)
        {
            db = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            int profileID = await db.Profile.Where(n => n.Name == name).Select(i => i.ProfileID).SingleAsync();
            bool result = false;
            var invitation = await db.Invitations
                .Where(r => r.InvitationSender == profileID && r.InvitationReceiver == idReceiver)
                .FirstOrDefaultAsync();

            if (invitation != null && !invitation.Notification)
            {
                invitation.Notification = true;
                var i = db.Entry(invitation);
                i.Property("Notification").IsModified = true;
                result = await db.SaveChangesAsync() > 0;
            }

            return result;
        }
        public async Task<List<Friends>> CreateFriendListAsync()
        {
            var model = await SelectProfilesAsync();

            return model;
        }
        private async Task<List<Friends>> SelectProfilesAsync()
        {
            db = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            var model = await db.Friends.Where(o => o.Owner == name).ToListAsync();

            return model;
        }
        public async Task<int> OverallNotificationAsync()
        {
            var model = await SelectInvitationsAsync();
            var noti = model.Where(n => n.Notification == true).Select(i => i.Notification).ToList();
            int counter = 0;

            foreach (var n in noti)
            {
                if (n)
                    counter++;
            }
                //queriesTemp = await profiles.Where(n => n.Nickname.Contains(v)).ToListAsync();

            return counter;
        }
        public async Task<List<Invitations>> CreateInvitationsAsync(int notiAmount)
        {
            var model = await SelectInvitationsAsync();
            if(notiAmount > 0)
            {
                var result = await ClearNotifications();
            }

            return model;
        }
        private async Task<List<bool>> ClearNotifications()
        {
            var invitationList = await db.Invitations.Where(n => n.InvitationReceiver == profileID).ToListAsync();
            List<bool> result = new List<bool>();

            foreach(var i in invitationList)
            {
                i.Notification = false;
                var entry = db.Entry(i);
                entry.Property("Notification").IsModified = true;
                result.Add(await db.SaveChangesAsync()>0);
            }

            return result;
        }
        private async Task<List<Invitations>> SelectInvitationsAsync()
        {
            db = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            profileID = await db.Profile.Where(n => n.Name == name).Select(i => i.ProfileID).SingleAsync();
            //List<BasicProfileInfoModels> profileList = new List<BasicProfileInfoModels>();
            //var invitations = await db.Invitations
            //    .Where(r => r.InvitationReceiver == profileId && r.Accept == false)
            //    .ToListAsync();
            //foreach (var i in invitations)
            //{
            //     profileList.Add( await db.Profile.Where(p => p.ProfileID == i.InvitationSender).SingleAsync());
            //}
            var invitationList = await db.Invitations.Where(n => n.InvitationReceiver == profileID).ToListAsync();

            return invitationList;
        }
        public async Task<bool> InvitationCloser(string parametersInv)
        {
            string[] parameters = parametersInv.Split(',');
            invID = Convert.ToInt32(parameters[0]);
            operation = parameters[1];
            bool result = false;
            switch (operation)
            {
                case "accept":
                    result = await AcceptInv();
                    break;
                case "decline":
                    result = await DeclineInv();
                    break;
                default:
                    break;
            }

            return result;
        }
        private async Task<bool> AcceptInv()
        {
            db = new HomeLibraryConnection();
            //string name = HttpContext.Current.User.Identity.Name;
            //string senderName = await db.Profile.Where(p => p.ProfileID == profileID).Select(n => n.Name).FirstOrDefaultAsync();
            //var profile = await db.Profile.Where(n => n.Name == name).Select(p=>p.ProfileID).SingleAsync();
            //var inv = await db.Invitations
            //    .Where(s => s.InvitationSender == profileID && s.InvitationReceiver == profile)
            //    .SingleAsync();
            var inv = await db.Invitations.Where(i => i.InvitationID == invID).SingleAsync();
            int id1 = inv.Receiver.ProfileID;
            int id2 = inv.Sender.ProfileID;
            var profileF = await db.Profile.Where(d => d.ProfileID == id1).SingleAsync();
            var profileF2 = await db.Profile.Where(g => g.ProfileID == id2).SingleAsync();


            //assign friend to this profile(receiver)
            Friends f = new Friends {
                ProfileID = inv.Sender.ProfileID,
                Owner = inv.Sender.Name,
                Block = false,
                InvitationID = inv.InvitationID,
                Profile = profileF,
                Invitations = inv
            };
            //assign friend to sender profile
            Friends f2 = new Friends {
                ProfileID = inv.Receiver.ProfileID,
                Owner = inv.Receiver.Name,
                Block = false,
                InvitationID = inv.InvitationID,
                Profile = profileF2,
                Invitations = inv
            };
            inv.Accept = true;
            var b = db.Entry(inv);
            b.Property("Accept").IsModified = true;
            bool result = await db.SaveChangesAsync()>0;
            db.Friends.Add(f);
            //inv.FriendID = f.FriendID;
            //string y = "null";
            result = await db.SaveChangesAsync() > 0;
            db.Friends.Add(f2);
            result = await db.SaveChangesAsync() > 0;

            return result;
        }
        private async Task<bool> DeclineInv()
        {
            db = new HomeLibraryConnection();
            var invitation = await db.Invitations.Where(i => i.InvitationID == invID).SingleAsync();
            db.Invitations.Remove(invitation);
            bool result = await db.SaveChangesAsync()>0;

            return result;
        }
        public async Task<BasicProfileInfoModels> FriendsProfileManager(int friendID)
        {
            profileID = friendID;
            var model = await SelectFriendsProfileAsync();

            return model;
        }
        private async Task<BasicProfileInfoModels> SelectFriendsProfileAsync()
        {
            db = new HomeLibraryConnection();
            var model = await db.Profile.Where(p => p.ProfileID == profileID).FirstOrDefaultAsync();

            return model;
        }
        public async Task<dynamic> friendPhotoManager(int pID)
        {
            profileID = pID;
            dynamic model = new ExpandoObject();
            model.Profile = await SelectFriendsProfileAsync();
            model.Pictures = await friendsPicturesAsync();

            return model;
        }
        private async Task<List<ProfilePicturesModels>> friendsPicturesAsync()
        {
            var model = await db.Pictures.Where(i => i.ProfileID == profileID).ToListAsync();

            return model;
        }
        public async Task<dynamic> GetListFriendsAsync(int pID)
        {
            profileID = pID;
            dynamic model = new ExpandoObject();
            model.Profile = await SelectFriendsProfileAsync();
            model.userList = await GetProfilesAsync();

            if (model.userList != null && model.userList.Count > 0)
            {
                model.friendList = await AllreadyFriend(model.userList);
                model.invitedList = await AllreadyInvited(model.userList);
            }

            return model;
        }
        private async Task<List<BasicProfileInfoModels>> GetProfilesAsync()
        {
            db = new HomeLibraryConnection();
            string thisName = HttpContext.Current.User.Identity.Name;
            string name = await db.Profile.Where(i => i.ProfileID == profileID).Select(n => n.Name).FirstOrDefaultAsync();
            var friends = await db.Friends.Where(o => o.Owner == name && o.Profile.Name != thisName).Select(f=>f.Profile).ToListAsync();

            return friends;
        }
    }
}
