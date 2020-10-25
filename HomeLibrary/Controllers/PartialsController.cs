using HomeLibrary.Services.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeLibrary.Controllers
{
    public class PartialsController : Controller
    {
        // GET: Partials
        public ActionResult Chat()
        {
            ChatServices cs = new ChatServices();
            var model = cs.CreateFriendsList();

            return PartialView("_ChatPartial", model);
        }
        public ActionResult ChatCurrent()
        {

            return PartialView("_ChatCurrent");
        }
    }
}