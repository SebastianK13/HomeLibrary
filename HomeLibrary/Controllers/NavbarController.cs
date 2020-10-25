using HomeLibrary.DAL;
using HomeLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HomeLibrary.Controllers
{
    public class NavbarController : Controller
    {
        public ActionResult SelectAvatar()
        {
            HomeLibraryConnection db = new HomeLibraryConnection();
            string name = HttpContext.User.Identity.Name;

            var model = db.Profile.Where(n => n.Name == name).Single();

            return PartialView("_AvatarPartial", model);
        }
    }
}

//public class NavbarController : Controller
//{
//    public ActionResult SelectAvatar()
//    {
//        HomeLibraryConnection db = new HomeLibraryConnection();
//        string name = HttpContext.User.Identity.Name;

//        var model = db.Profile.Where(n => n.Name == name).Single();

//        return PartialView("_AvatarPartial", model);
//    }
//}

//var model = Task.Run(async () => { await db.Profile.Where(n => n.Name == name).SingleAsync(); });