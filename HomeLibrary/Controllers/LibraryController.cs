using HomeLibrary.DAL;
using HomeLibrary.Infrastructure;
using HomeLibrary.Models;
using HomeLibrary.Services;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace HomeLibrary.Controllers
{
    public class LibraryController : Controller
    {
        private HomeLibraryConnection db;
        private ISessionManager sessionManager { get; set; }
        private ApplicationUserManager _userManager;

        public ApplicationUserManager userManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

            private set
            {
                _userManager = value;
            }
        }
        public LibraryController()
        {
            db = new HomeLibraryConnection();
            sessionManager = new SessionManager();
        }
        // GET: Library
        //public ActionResult MyLibrary()
        //{
        //    MainLibraryService mls = new MainLibraryService();
        //    mls.ClearAll(db);
        //    var sorts = db.Sorts.ToList();
        //    return View(sorts);
        //}
        //[HttpPost]
        //public ActionResult MyLibrary(FormCollection formCollection)
        //{
        //    MainLibraryService mls = new MainLibraryService();
        //    mls.SortService(formCollection, db);
        //    mls.SaveLastStatus(db);
        //    var sorts = db.Sorts.ToList();
        //    return View(sorts);
        //}
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult ManageBooks(int actionNumber = 0)
        {
            MainLibraryService mls = new MainLibraryService();
            mls.ClearAll(db);
            UsersBooks books = new UsersBooks();
            //List<Book> usersBooks = new List<Book>();
            var name = User.Identity.Name;
            var mainPath = Path.Combine(Server.MapPath("~/Images/"));
            ViewData["bookClass"] = mls.ActionSwitcher(actionNumber);
            ViewData["actionID"] = actionNumber;

            return View(books.LoadUsersBooks(db,name,mainPath));
        }
        public ActionResult AddBook()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddBook(FormCollection formCollection, HttpPostedFileBase file, BookDetail detail)
        {
            if (ModelState.IsValid && User.Identity.IsAuthenticated)
            {
                //storing current user (logged) and image path both need to insert to dbo.book
                List<string> dbFields = new List<string>();
                var t = User.Identity.Name;
                var path = Path.Combine(Server.MapPath("~/Images/"+t+"/"));
                dbFields.Add(t);
                dbFields.Add(path);
                UpdateBookDatabases update = new UpdateBookDatabases();
                update.InputsSplitter(formCollection, file, dbFields);
                return RedirectToAction("ManageBooks");
            }

            return View();
        }
        //Remove
        //Modify
        //Show
        [HttpPost]
        public ActionResult ShowBookDetails(FormCollection formCollection)
        {
            MainLibraryService mls = new MainLibraryService();
            ManageBooksAllServices mbas = new ManageBooksAllServices();
            var temp = formCollection.Keys[0];
            temp = temp.Replace("undefined", "");
            string []tempServiceAttributes = temp.Split(new Char[]{','});
            List<string> serviceAttributes = new List<string>();
            string attr = "null";
            serviceAttributes = tempServiceAttributes.ToList();
            if(serviceAttributes.Count()<2)
            {
                serviceAttributes.Add("null");
                attr = serviceAttributes[0];
            }
            else
            {
                attr = serviceAttributes[1];
            }

            var serviceName = mbas.ServiceSwitcher(serviceAttributes);

            //in form collection there is a book id from db. Make redirection and data load for concrete book by id
            return RedirectToAction(serviceName, new {bookId=attr});
        }
        public ActionResult ShowBook(string bookId)
        {
            ManageBooksAllServices mbas = new ManageBooksAllServices();
            var model = mbas.BookInfoProvider(bookId);
            
            return View(model);
        }
        public ActionResult ModifyBook(string bookId)
        {
            ManageBooksAllServices mbas = new ManageBooksAllServices();
            var model = mbas.BookInfoProvider(bookId);
            List<string> fields = new List<string> { "", "", "", "", "", "", "", "", "" };
            ViewData["BookID"] = bookId;
            ViewData["FormFields"] = fields;
            ViewData["SourceIMG"] = model.Book.ImageSource;

            return View(model);
        }
        public async Task<ActionResult> Remove(string bookId)
        {
            ManageBooksAllServices mbas = new ManageBooksAllServices();
            await mbas.RemoveService(bookId);

            return RedirectToAction("ManageBooks");
        }
        [HttpPost]
        public async Task<ActionResult> ModifyBook(FormCollection form, BookDetail book, HttpPostedFileBase file)
        {
            ManageBooksAllServices mbas = new ManageBooksAllServices();
            List<bool> isEmpty = new List<bool>(mbas.EmptyFormField(form));
            List<string> fieldsName = new List<string>(mbas.CreateListField(form));
            var serverImagePath = mbas.ImagePathSetter(form);
            ViewData["FormFields"] = mbas.FieldsValue(form, isEmpty);
            int c = 0;

            foreach (var f in fieldsName)
            {
                if(isEmpty[c])
                    ModelState.Where(m => m.Key == f).FirstOrDefault().Value.Errors.Clear();            
                c++;
            }
            if (ModelState.IsValid && User.Identity.IsAuthenticated)
            {
                mbas.ChangeImgPath(file, serverImagePath);
                await mbas.ModifyBookInfo(form, isEmpty);
                return RedirectToAction("ManageBooks");
            }
            else
            {
                var model = mbas.BookInfoProvider(form[10]);
                ViewData["BookID"] = form[10];
                ViewData["SourceIMG"] = mbas.ViewDataSetter(file, serverImagePath);

                return View(model);
            }
        }
        [HttpPost]
        public ActionResult Sort(FormCollection formCollection, string viewName, int actionNumber=0)
        {
            MainLibraryService mls = new MainLibraryService();
            UsersBooks books = new UsersBooks();
            mls.SortService(formCollection, db);
            mls.SaveLastStatus(db);
            var name = User.Identity.Name;
            var mainPath = Path.Combine(Server.MapPath("~/Images/"));
            var model = books.LoadUsersBooks(db, name, mainPath);
            ViewData["StatusAmount"] = books.BookStatusAmount(db);
            ViewData["bookClass"] = mls.ActionSwitcher(actionNumber);
            ViewData["actionID"] = actionNumber;

            return View(viewName, model);
        }
        public ActionResult ChangeStatus()
        {
            MainLibraryService mls = new MainLibraryService();
            mls.ClearAll(db);
            UsersBooks books = new UsersBooks();
            var name = User.Identity.Name;
            var mainPath = Path.Combine(Server.MapPath("~/Images/"));
            var model = books.LoadUsersBooks(db, name, mainPath);
            ViewData["StatusAmount"] = books.BookStatusAmount(db);
             
            return View(model);
        }
        [HttpPost]
        public ActionResult ChangeStatus(FormCollection collection)
        {
            ChangeBookStatus cbs = new ChangeBookStatus();
            cbs.ChangeBookStatusManager(collection);

            return RedirectToAction("ChangeStatus");
        }
        public ActionResult BorrowedBooks()
        {
            MainLibraryService mls = new MainLibraryService();
            mls.ClearAll(db);
            UsersBooks books = new UsersBooks();
            var name = User.Identity.Name;
            var mainPath = Path.Combine(Server.MapPath("~/Images/"));
            ViewData["StatusAmount"] = books.BookStatusAmount(db);

            return View(books.LoadUsersBooks(db, name, mainPath));
        }
        public ActionResult LendBooks()
        {
            MainLibraryService mls = new MainLibraryService();
            mls.ClearAll(db);
            UsersBooks books = new UsersBooks();
            var name = User.Identity.Name;
            var mainPath = Path.Combine(Server.MapPath("~/Images/"));
            ViewData["StatusAmount"] = books.BookStatusAmount(db);

            return View(books.LoadUsersBooks(db, name, mainPath));
        }
        public ActionResult AvailableBooks()
        {
            MainLibraryService mls = new MainLibraryService();
            mls.ClearAll(db);
            UsersBooks books = new UsersBooks();
            var name = User.Identity.Name;
            var mainPath = Path.Combine(Server.MapPath("~/Images/"));
            ViewData["StatusAmount"] = books.BookStatusAmount(db);

            return View(books.LoadUsersBooks(db, name, mainPath));
        }
        public async Task<ActionResult> RateBook()
        {
            MainLibraryService mls = new MainLibraryService();
            mls.ClearAll(db);
            var model = await mls.GetBooksAsync();

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> RateBook(int rate, int id)
        {
            MainLibraryService mls = new MainLibraryService();
            bool result = await mls.BookRateManagerAsync(rate, id);

            return RedirectToAction("RateBook");
        }
        public async Task<ActionResult> SwapBook()
        {
            MainLibraryService mls = new MainLibraryService();
            mls.ClearAll(db);
            var model = await mls.GetAvailableBooksAsync();
            ViewBag.BookList = mls.CreateListItem(model);

            return View(model);
        }
        //done
        [HttpPost]
        public async Task<ActionResult> SwapBook(Swap swap)
        {
            if(ModelState.IsValid && User.Identity.IsAuthenticated)
            {
                MainLibraryService mls = new MainLibraryService();
                mls.ClearAll(db);
                bool result = await mls.InsertSwapManagerAsync(swap);
            }
            return RedirectToAction("SwapBook");
        }
        //done
        public async Task<ActionResult> TransactionOffers()
        {
            MainLibraryService mls = new MainLibraryService();
            mls.ClearAll(db);
            var model = await mls.TransactionManagerAsync();

            return View(model);
        }
        //done
        public async Task<ActionResult> SwapTransactions()
        {
            MainLibraryService mls = new MainLibraryService();
            mls.ClearAll(db);
            var model = await mls.SwapsManagerAsync();
            ViewBag.NotificationCount = await mls.NotificationCountManagerAsync();

            return View(model);
        }
        //SwitchTransaction
        [HttpPost]
        public ActionResult SwitchTransaction(int SwapID, string actionName)
        {
            switch (actionName)
            {
                case "Give it away":
                    return RedirectToAction("AwayTransaction", new { SwapID = SwapID });
                case "Sell":
                    return RedirectToAction("SellTransaction", new { SwapID = SwapID });
                case "Swap book":
                    return RedirectToAction("SwapTransaction", new { SwapID = SwapID });
            }
            return RedirectToAction("TransactionOffers");
        }
        //AwayTransaction - offer
        public async Task<ActionResult> AwayTransaction(int SwapID)
        {
            MainLibraryService mls = new MainLibraryService();
            mls.ClearAll(db);
            var model = await mls.SwapTManagerAsync(SwapID);

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> AwayTransaction(Offer offer)
        {
            MainLibraryService mls = new MainLibraryService();
            bool result = await mls.InsertAwayOfferAsync(offer);

            return RedirectToAction("TransactionOffers");
        }
        //SellTransaction - offer
        public async Task<ActionResult> SellTransaction(int SwapID)
        {
            MainLibraryService mls = new MainLibraryService();
            mls.ClearAll(db);
            var model = await mls.SwapTManagerAsync(SwapID);

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> SellTransaction(Offer offer, bool accept)
        {
            MainLibraryService mls = new MainLibraryService();
            bool result = await mls.InsertSellOfferAsync(offer, accept);

            return RedirectToAction("TransactionOffers");
        }
        //SwapTransaction - offer
        public async Task<ActionResult> SwapTransaction(int SwapID)
        {
            MainLibraryService mls = new MainLibraryService();
            mls.ClearAll(db);
            dynamic model = new ExpandoObject();
            model.Transaction = await mls.SwapTManagerAsync(SwapID);
            model.Books = await mls.GetAvailableBooksAsync();
            ViewBag.BookList = mls.CreateListItem(model.Books);

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> SwapTransaction(SwapPropose propose)
        {
            MainLibraryService mls = new MainLibraryService();
            bool result = await mls.InsertSwapOfferAsync(propose);

            return RedirectToAction("TransactionOffers");
        }
        public async Task<ActionResult> Offers()
        {
            MainLibraryService mls = new MainLibraryService();
            var model = await mls.OffersListManagerAsync();
            ViewBag.NotificationCount = await mls.NotificationCountManagerAsync();

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Offers(string parameters, string action)
        {
            MainLibraryService mls = new MainLibraryService();
            bool success = false;
            switch (action)
            {
                case "Accept":
                    success = await mls.UpdateMyOfferManagerAsync(parameters);
                    break;
                case "Decline":
                    success = await mls.RemoveMyOfferManagerAsync(parameters);
                    break;
                case "Commit":
                    success = await mls.CommitMyOfferManagerAsync(parameters);
                    break;
                case "Withdraw":
                    success = await mls.RemoveMyOfferManagerAsync(parameters);
                    break;
            }

            return RedirectToAction("Offers");
        }
        public async Task<ActionResult> MyOffers()
        {
            MainLibraryService mls = new MainLibraryService();
            var model = await mls.MyOffersListManagerAsync();
            ViewBag.NotificationCount = await mls.NotificationCountManagerAsync();

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> MyOffers(string parameters, string action)
        {
            MainLibraryService mls = new MainLibraryService();
            bool success = false;
            switch (action)
            {

                case "Commit":
                    success = await mls.CommitOfferManagerAsync(parameters);
                    break;
                case "Withdraw":
                    success = await mls.RemoveOfferManagerAsync(parameters);
                    break;
            }

            return RedirectToAction("MyOffers");
        }
        public async Task<ActionResult> CompletedDeals()
        {
            MainLibraryService mls = new MainLibraryService();
            var model = await mls.CompletedDealsListManagerAsync();
            ViewBag.NotificationCount = await mls.NotificationCountManagerAsync();

            return View(model);
        }
    }
}