using HomeLibrary.DAL;
using HomeLibrary.Models;
using HomeLibrary.Services.Community;
using HomeLibrary.Services.ManageServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HomeLibrary.Controllers
{
    public class CommunityController : Controller
    {
        // GET: Community
        public ActionResult Messages()
        {
            return View();
        }
        public async Task<dynamic> Users(string searchValue)
        {
            FriendsServices fs = new FriendsServices();
            var model = await fs.FindUser(searchValue);
            ViewData["searchV"] = searchValue;
            //var model = db.Friends.ToList();

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Users(FormCollection collection)
        {
            FriendsServices fs = new FriendsServices();
            bool succes = await fs.IvitationManager(collection[0]);            

            return RedirectToAction("Users", new { searchValue = collection[1] });
        }
        public ActionResult AccountSettings()
        {
            return View();
        }
        public async Task<ActionResult> InformationManager()
        {
            InformationManagerServices ims = new InformationManagerServices();
            var model = await ims.SelectProfileInfoAsync();

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> InformationManager(FormCollection collection, BasicProfileInfoModels bpi)
        {

            InformationManagerServices ims = new InformationManagerServices();
            var model = await ims.SelectProfileInfoAsync();
            if (ModelState.IsValid && User.Identity.IsAuthenticated)
            {
                var t = await ims.ProfileUpdateManager(collection[2], collection[1]);

                return RedirectToAction("InformationManager"); 
            }
            else
            {
                return View(model);
            }

        }
        public async Task<ActionResult> UserProfile()
        {
            ProfileServices ps = new ProfileServices();
            var model = await ps.SelectProfile();

            return View(model);
        }
        public async Task<ActionResult> PicturesProfile()
        {
            ProfileServices ps = new ProfileServices();
            var model = await ps.PhotosManager();

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> PicturesProfile(HttpPostedFileBase file, string choseToRemove)
        {
            if (file != null && file.ContentLength > 0)
            {
                ProfileServices ps = new ProfileServices();
                bool result = await ps.PicturesManagerAsync(file);
            }
            else if (choseToRemove.Length > 0)
            {
                //ToDo...
                ProfileServices ps = new ProfileServices();
                await ps.RemovePcituresAsync(choseToRemove);
            }

            return RedirectToAction("PicturesProfile");
        }
        [HttpPost]
        public async Task<ActionResult> PicturesSetter(FormCollection collection)
        {
            ProfileServices ps = new ProfileServices();
            int t = -1;
            int c = -1;
            if (collection[0] != "")
            {
                 t = Convert.ToInt32(collection[0]);
            }
            else if (collection[1] != "")
            {
                 c = Convert.ToInt32(collection[1]);
            }
            

            bool result = await ps.SetPicture(t, c);

            return RedirectToAction("PicturesProfile");
        }
        public async Task<ActionResult> Friends()
        {
            FriendsServices fs = new FriendsServices();
            var model = await fs.CreateFriendListAsync();
            ViewData["notification"] = await fs.OverallNotificationAsync(); 

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> InvitationAccept(FormCollection collection)
        {
            HomeLibraryConnection db = new HomeLibraryConnection();
            FriendsServices fs = new FriendsServices();
            bool success = await fs.AddFriendManager(collection[0]);
            //var model = db.Friends.ToList();

            return RedirectToAction("Friends");
        }
        public async Task<ActionResult> Invitations(int notiAmount = 0)
        {
            FriendsServices fs = new FriendsServices();
            var model = await fs.CreateInvitationsAsync(notiAmount);

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Invitations(string invID)
        {
            FriendsServices fs = new FriendsServices();
            bool result = await fs.InvitationCloser(invID);
            //var model = await fs.CreateInvitationsAsync();

            return RedirectToAction("Invitations");
        }
        [HttpPost]
        public async Task<ActionResult> FriendsProfile(string fID)
        {
            FriendsServices fs = new FriendsServices();
            var model = await fs.FriendsProfileManager(Convert.ToInt32(fID));

            return View(model);
        }
        public async Task<ActionResult> FriendsPictures(int profileID)
        {
            FriendsServices fs = new FriendsServices();
            var model = await fs.friendPhotoManager(profileID);

            return View(model);
        }
        public async Task<ActionResult> FriendsInformations(int profileID)
        {
            FriendsServices fs = new FriendsServices();
            var model = await fs.FriendsProfileManager(profileID);

            return View(model);
        }
        public async Task<ActionResult> FriendsUserList(int profileID)
        {
            FriendsServices fs = new FriendsServices();
            var model = await fs.GetListFriendsAsync(profileID);

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> FriendUserList(string pId, int thisProfileId)
        {
            FriendsServices fs = new FriendsServices();
            bool result = await fs.IvitationManager(pId);

            return RedirectToAction("FriendsUserList", new { profileID = thisProfileId });
        }
    }
}