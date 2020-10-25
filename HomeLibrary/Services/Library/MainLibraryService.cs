using HomeLibrary.DAL;
using HomeLibrary.Models;
using HomeLibrary.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HomeLibrary.Services
{
    public class MainLibraryService
    {
        private int swapID;
        private HomeLibraryConnection dba;
        public void SortService(FormCollection formCollection, HomeLibraryConnection db)
        {
            Sort sort = new Sort();
            for (int i = 1; i < formCollection.Keys.Count; i++)
            {
                string tmpName = formCollection.Keys[i];
                var changeSort = db.Sorts.Find(i - 1); 

                if (!string.IsNullOrEmpty(formCollection[i]))
                {
                    string tmpStatus = formCollection[i];
                    if (tmpStatus.Contains(",") && !changeSort.LastStatus)
                    {
                        changeSort.Status = true;
                    }
                    else
                    {
                        changeSort.Status = false;
                    }
                }
            }
            db.SaveChanges();
        }
        public void SaveLastStatus(HomeLibraryConnection db)
        {
            var removePrevious = db.Sorts;
            List<int> lastStatus = new List<int>();
            List<int> currentStatus = new List<int>();
            try
            {
                currentStatus = db.Sorts.Where(p => p.Status == true).Select(p => p.Id).ToList();
                lastStatus = db.Sorts.Where(p => p.LastStatus == true).Select(p => p.Id).ToList();
            }
            catch (InvalidOperationException) { /*error prevention if currentStatus or lasStatus have no element*/}
            switch (CaseReturn(lastStatus, currentStatus))
            {
                case 0:
                    UpdateStatus(removePrevious);
                    currentStatus.Clear();
                    lastStatus.Clear();
                    break;
                case 1:
                    UpdateLastStatus(currentStatus, removePrevious);
                    break;
                case 2:
                    ClearAll(db);
                    break;
                default:
                    break;
            }
            db.SaveChanges();
        }
        private int CaseReturn(List<int>lastStatus,List<int>currentStatus)
        {
            if (currentStatus.Count > 1)
            {
                return 0;
            }
            else if (lastStatus.Count() > 0 && currentStatus.Count() > 0)
            {
                return 1;
            }
            else if (lastStatus.Count() == 0 && currentStatus.Count() != 0)
            {
                return 1;
            }
            else if (currentStatus.Count() == 0)
            {
                return 2;
            }
            else
            {
                return -1;
            }
        }
        private void UpdateStatus(DbSet<Sort> removePrevious)
        {
            foreach (var s in removePrevious)
            {
                if (s.LastStatus)
                {
                    s.Status = true;
                }
                else
                {
                    s.Status = false;
                }
            }
        }
        private void UpdateLastStatus(List<int> currentStatus, DbSet<Sort> removePrevious)
        {
            foreach (var s in removePrevious)
            {
                if (s.Id == currentStatus[0])
                {
                    s.LastStatus = true;
                }
                else
                {
                    s.LastStatus = false;
                }
            }
        }
        public void ClearAll(HomeLibraryConnection db)
        {
            var ClearDb = db.Sorts;
            foreach (var s in ClearDb)
            {
                s.Status = false;               
                s.LastStatus = false;
            }
            db.SaveChanges();
        }
        public List<string> ActionSwitcher(int actionNumber)
        {
            List<string> actionName = new List<string>();
            switch(actionNumber)
            {
                case 0:
                    actionName = DefaultAction();
                    break;
                case 1:
                    actionName = ModifyAction();
                    break;
                case 2:
                    actionName = RemoveAction();
                    break;
                default:
                    actionName = DefaultAction();
                    break;
            }
            return actionName;
        }
        //return list of classes for ManageBook view 
        private List<string> ModifyAction()
        {
            List<string> ModifyClassList = new List<string>();
            ModifyClassList.Add("single-book-label single-book-label-modify");
            ModifyClassList.Add("Modify");
            ModifyClassList.Add("class = label-icon");
            ModifyClassList.Add("/Images/Modify_icon.png");
            ModifyClassList.Add("Modify,");

            return ModifyClassList;
        }
        private List<string> RemoveAction()
        {
            List<string> RemoveClassList = new List<string>();
            RemoveClassList.Add("single-book-label single-book-label-remove");
            RemoveClassList.Add("Remove");
            RemoveClassList.Add("class = label-icon");
            RemoveClassList.Add("/Images/Remove_icon.png");
            RemoveClassList.Add("Remove,");

            return RemoveClassList;
        }
        private List<string> DefaultAction()
        {
            List<string> DefaultClassList = new List<string>();
            DefaultClassList.Add("single-book-label");
            DefaultClassList.Add("More details");
            DefaultClassList.Add("");
            DefaultClassList.Add("/Images/bookImageLabel.png");
            DefaultClassList.Add("");

            return DefaultClassList;
        }
        public async Task<dynamic> GetBooksAsync()
        {
            dynamic model = new ExpandoObject();
            model.Books = await GetBooksListAsync();
            model.Sort = dba.Sorts.ToList();

            return model;
        }
        private async Task<List<BookView>> GetBooksListAsync()
        {
            dba = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            var books = await dba.Books.Where(n => n.UserName == name).ToListAsync();
            BookView booksView = new BookView(books);
            

            return booksView.BookViews;
        }
        public async Task<bool> BookRateManagerAsync(int rate, int id)
        {
            bool result = await SetBookRateAsync(rate, id);

            return result;
        }
        private async Task<bool> SetBookRateAsync(int rate, int id)
        {
            dba = new HomeLibraryConnection();
            var book = dba.Books.Where(i => i.BookID == id).FirstOrDefault();
            book.Rate = rate;
            bool result = await dba.SaveChangesAsync() > 0;

            return result;
        }
        public async Task<List<BookView>> GetAvailableBooksAsync()
        {
            var model = await GetAvailableBooksListAsync();

            return model;
        }
        private async Task<List<BookView>> GetAvailableBooksListAsync()
        {
            dba = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            List<int> offeredList = await dba.Proposes
                .Where(b => b.Book.UserName == name)
                .Select(i=>i.Book.BookID)
                .ToListAsync();

            var books = await dba.Books
                .Where(n => n.UserName == name && n.Swap == null && !offeredList.Contains(n.BookID))
                .ToListAsync();
            BookView booksView = new BookView(books);

            return booksView.BookViews;
        }
        public List<SelectListItem> CreateListItem(List<BookView> books)
        {
            List<SelectListItem> ListItem = new List<SelectListItem>();
            
            foreach(var b in books)
            {
                ListItem.Add(new SelectListItem { Text = b.Title, Value = b.BookID.ToString(), Selected = true});
            }

            return ListItem;
        }
        public async Task<dynamic> SwapsManagerAsync()
        {
            dynamic model = new ExpandoObject();
            model.Swaps = await GetSwapsAsync();
            model.Sort = dba.Sorts.ToList();

            return model;
        }
        private async Task<List<SwapViewModel>> GetSwapsAsync()
        {
            dba = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            var idList = await dba.Books.Where(n => n.UserName == name).Select(b => b.BookID).ToListAsync();
            var swaps = await dba.Swaps.Where(i => idList.Contains(i.SwapID)).ToListAsync();
            SwapViewModel swapView = new SwapViewModel(swaps);

            return swapView.SwapView;
        }
        public async Task<dynamic> TransactionManagerAsync()
        {
            dynamic model = new ExpandoObject();
            model.Swaps = await GetAllSwapsAsync();
            model.Sort = dba.Sorts.ToList();

            return model;
        }
        private async Task<List<SwapViewModel>> GetAllSwapsAsync() 
        {
            dba = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            List<int> offers = await dba.Offers
                .Where(p => p.Profile.Name == name && p.Closed == false)
                .Select(b=>b.Swap.Book.BookID)
                .ToListAsync();

            List<int> swapsOff = await dba.Proposes
                .Where(p => p.Book.UserName == name && p.Closed == false)
                .Select(b => b.Swap.Book.BookID)
                .ToListAsync();

            var idOfBooks = offers.Concat(swapsOff);

            List <Swap> swaps = await dba.Swaps
                .Where(c => c.Closed == false && c.Book.Profile.Name != name && !idOfBooks.Contains(c.Book.BookID))
                .ToListAsync();

            SwapViewModel swapView = new SwapViewModel(swaps);

            return swapView.SwapView;
        }
        public async Task<bool> InsertSwapManagerAsync(Swap swap)
        {
            bool result = await InsertSwapAsync(swap);

            return result;
        }
        private async Task<bool> InsertSwapAsync(Swap swap)
        {
            dba = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            int profileID = dba.Profile.Where(n => n.Name == name).Select(i => i.ProfileID).FirstOrDefault();
            swap.OwnerID = profileID;
            dba.Swaps.Add(swap);
            bool result = await dba.SaveChangesAsync() > 0;

            return result;
        }
        public async Task<SwapViewModel> SwapTManagerAsync(int SwapID)
        {
            swapID = SwapID;
            var model = await SwapTAsync();

            return model;
        }
        private async Task<SwapViewModel> SwapTAsync()
        {
            dba = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            List<Swap> swaps = await dba.Swaps.Where(i=>i.SwapID == swapID).ToListAsync();
            SwapViewModel swapView = new SwapViewModel(swaps);

            return swapView;
        }
        public async Task<bool> InsertAwayOfferAsync(Offer offer)
        {
            bool result = await InsertAOfferAsync(offer);
            
            return result;
        }
        private async Task<bool> InsertAOfferAsync(Offer offer)
        {
            dba = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            var profileID = await dba.Profile.Where(n => n.Name == name).Select(i=>i.ProfileID).SingleAsync();
            offer.ProfileID = profileID;
            offer.Notification = true;
            dba.Offers.Add(offer);
            bool result = await dba.SaveChangesAsync() > 0;

            return result;
        }

        public async Task<bool> InsertSellOfferAsync(Offer offer, bool accept)
        {
            bool result = await InsertSOfferAsync(offer, accept);

            return result;
        }
        private async Task<bool> InsertSOfferAsync(Offer offer, bool accept)
        {
            dba = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            var profileID = await dba.Profile.Where(n => n.Name == name).Select(i => i.ProfileID).SingleAsync();
            if (accept)
            {
                var PriceFields = await dba.Swaps
                    .Where(i => i.SwapID == offer.SwapID)
                    .Select(c=> new {c.Currency, c.Price})
                    .SingleAsync();
                offer.ProposingPrice = PriceFields.Price;
                offer.CurrencyCode = PriceFields.Currency;
            }
            offer.Notification = true;
            offer.ProfileID = profileID;
            dba.Offers.Add(offer);
            bool result = await dba.SaveChangesAsync() > 0;

            return result;
        }

        public async Task<bool> InsertSwapOfferAsync(SwapPropose propose)
        {
            bool result = await InsertSwOfferAsync(propose);

            return result;
        }        
        private async Task<bool> InsertSwOfferAsync(SwapPropose propose)
        {
            dba = new HomeLibraryConnection();
            propose.Notification = true;
            dba.Proposes.Add(propose);
            bool result = await dba.SaveChangesAsync() > 0;

            return result;
        }
        public async Task<dynamic> OffersListManagerAsync()
        {
            dynamic model = new ExpandoObject();
            model.SwapOffers = await SwapOffersListAsync();
            model.AwayOffers = await AwayOffersListAsync();
            model.SellOffers = await SellOffersListAsync();
            model.Pending = await PendingListOfferAsync();
            model.Closed = await ClosedListOfferAsync();
            model.Sort = await dba.Sorts.ToListAsync();

            return model;
        }
        private async Task<List<PendingOffers>> PendingListOfferAsync()
        {
            dba = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            var profileID = await dba.Profile.Where(n => n.Name == name).Select(i => i.ProfileID).SingleOrDefaultAsync();
            var offers = await dba.Offers
                .Where(i => i.Swap.Book.ProfileID == profileID && i.OfferID == i.Swap.OfferTransaction
                 && !((i.Closed == true && i.Swap.Closed == true)))
                .ToListAsync();

            var proposes = await dba.Proposes
                .Where(i => i.Swap.Book.ProfileID == profileID && i.ProposeID == i.Swap.SwapTransaction
                 //&& ((i.Closed == true || i.Swap.Closed == true))
                 && !((i.Closed == true && i.Swap.Closed == true)))
                .ToListAsync();

            PendingOffers PO1 = new PendingOffers(offers);
            PendingOffers PO2 = new PendingOffers(proposes);

            var model = PO1.PendingOffersList.Union(PO2.PendingOffersList).ToList();


            return model;
        }
        private async Task<List<PendingOffers>> ClosedListOfferAsync()
        {
            dba = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            var profileID = await dba.Profile.Where(n => n.Name == name).Select(i => i.ProfileID).SingleOrDefaultAsync();
            var offers = await dba.Offers
                .Where(i => i.Swap.Book.ProfileID == profileID && i.Swap.OfferTransaction != 0 && i.Swap.Closed == true)
                .ToListAsync();

            var proposes = await dba.Proposes
                .Where(i => i.Swap.Book.ProfileID == profileID && i.Swap.SwapTransaction != 0 && i.Swap.Closed == true)
                .ToListAsync();

            PendingOffers PO1 = new PendingOffers(offers);
            PendingOffers PO2 = new PendingOffers(proposes);

            var model = PO1.PendingOffersList.Union(PO2.PendingOffersList).ToList();


            return model;
        }
        private async Task<List<Offer>> AwayOffersListAsync()
        {
            dba = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            var profileID = await dba.Profile.Where(n => n.Name == name).Select(i => i.ProfileID).SingleOrDefaultAsync();
            var model = await dba.Offers
                .Where(i => i.Swap.Book.ProfileID == profileID && i.ProposeDesc != null && i.Swap.OfferTransaction == 0)
                .ToListAsync();

            return model;
        }
        private async Task<List<Offer>> SellOffersListAsync()
        {
            dba = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            var profileID = await dba.Profile.Where(n => n.Name == name).Select(i => i.ProfileID).SingleOrDefaultAsync();
            var model = await dba.Offers
                .Where(i => i.Swap.Book.ProfileID == profileID && i.ProposeDesc == null && i.Swap.OfferTransaction == 0)
                .ToListAsync();

            return model;
        }
        private async Task<List<SwapPropose>> SwapOffersListAsync()
        {
            dba = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            var profileID = await dba.Profile
                .Where(n => n.Name == name)
                .Select(i => i.ProfileID)
                .SingleOrDefaultAsync();

            var model = await dba.Proposes
                .Where(i => i.Swap.Book.ProfileID == profileID && i.Swap.SwapTransaction == 0)
                .ToListAsync();

            return model;
        }
        public async Task<dynamic> MyOffersListManagerAsync()
        {
            dynamic model = new ExpandoObject();
            model.SwapOffers = await SwapMyOffersListAsync();
            model.AwayOffers = await AwayMyOffersListAsync();
            model.SellOffers = await SellMyOffersListAsync();
            model.Pending = await PendingListMyOfferAsync();
            model.Sort = await dba.Sorts.ToListAsync();

            return model;
        }
        private async Task<List<PendingOffers>> PendingListMyOfferAsync()
        {
            dba = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            var profileID = await dba.Profile.Where(n => n.Name == name).Select(i => i.ProfileID).SingleOrDefaultAsync();
            var offers = await dba.Offers
                .Where(i => i.ProfileID == profileID && i.OfferID == i.Swap.OfferTransaction
                 && !((i.Closed == true && i.Swap.Closed == true)))
                .ToListAsync();

            var proposes = await dba.Proposes
                .Where(i => i.Book.ProfileID == profileID && i.ProposeID == i.Swap.SwapTransaction
                 && !((i.Closed == true && i.Swap.Closed == true)))
                .ToListAsync();

            PendingOffers PO1 = new PendingOffers(offers);
            PendingOffers PO2 = new PendingOffers(proposes);

            var model = PO1.PendingOffersList.Union(PO2.PendingOffersList).ToList();
            //&& ((i.Closed == true && i.Swap.Closed == false) || (i.Closed == false && i.Swap.Closed == true)))

            return model;
        }
        private async Task<List<Offer>> AwayMyOffersListAsync()
        {
            dba = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            var profileID = await dba.Profile
                .Where(n => n.Name == name)
                .Select(i => i.ProfileID)
                .SingleOrDefaultAsync();
            
            var model = await dba.Offers
                .Where(i => i.ProfileID == profileID && i.ProposeDesc != null && i.Swap.OfferTransaction == 0)
                .ToListAsync();

            return model;
        }
        private async Task<List<Offer>> SellMyOffersListAsync()
        {
            dba = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            var profileID = await dba.Profile
                .Where(n => n.Name == name)
                .Select(i => i.ProfileID)
                .SingleOrDefaultAsync();

            var model = await dba.Offers
                .Where(i => i.ProfileID == profileID && i.ProposeDesc == null && i.Swap.OfferTransaction == 0)
                .ToListAsync();

            return model;
        }
        private async Task<List<SwapPropose>> SwapMyOffersListAsync()
        {
            dba = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            var profileID = await dba.Profile
                .Where(n => n.Name == name)
                .Select(i => i.ProfileID)
                .SingleOrDefaultAsync();
            
            var model = await dba.Proposes
                .Where(i => i.Book.ProfileID == profileID && i.Swap.SwapTransaction == 0)
                .ToListAsync();

            return model;
        }
        public async Task<int> NotificationCountManagerAsync()
        {
            int notiAmount = await NotificationCountAsync();

            return notiAmount;
        }
        private async Task<int> NotificationCountAsync()
        {
            dba = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            var profileID = await dba.Profile
                .Where(n => n.Name == name)
                .Select(i => i.ProfileID)
                .SingleAsync();

            var amountP = await dba.Proposes
                .Where(i => i.Swap.Book.ProfileID == profileID && i.Notification == true)            
                .ToListAsync();

            var amountO = await dba.Offers
                .Where(i => i.Swap.Book.ProfileID == profileID && i.Notification == true)
                .ToListAsync();

            return amountP.Count()+amountO.Count();
        }
        public async Task<bool> RemoveMyOfferManagerAsync(string parameters)
        {
            string[] parameter = parameters.Split(',');
            bool success = false;
            switch (parameter[1])
            {
                case "Swap":
                    success = await RemoveSwapMyOfferAsync(Convert.ToInt32(parameter[0]));
                    break;
                case "Sell":
                    success = await RemoveSellMyOfferAsync(Convert.ToInt32(parameter[0]));
                    break;
                case "Away":
                    success = await RemoveAwayMyOfferAsync(Convert.ToInt32(parameter[0]));
                    break;
            }
            return success;
        }
        private async Task<bool> RemoveSwapMyOfferAsync(int ProposeID)
        {
            dba = new HomeLibraryConnection();
            var swapOffer = await dba.Proposes.Where(i => i.ProposeID == ProposeID).SingleOrDefaultAsync();
            if (swapOffer.Swap.SwapTransaction != 0)
            {
                swapOffer.Swap.SwapTransaction = 0;
            }
            dba.Proposes.Remove(swapOffer);
            bool success = await dba.SaveChangesAsync() > 0;

            return success;
        }
        private async Task<bool> RemoveSellMyOfferAsync(int OfferID)
        {
            dba = new HomeLibraryConnection();
            var offer = await dba.Offers.Where(i => i.OfferID == OfferID).SingleOrDefaultAsync();
            if (offer.Swap.OfferTransaction != 0)
            {
                offer.Swap.OfferTransaction = 0;
            }
            dba.Offers.Remove(offer);
            bool success = await dba.SaveChangesAsync() > 0;

            return success;
        }
        private async Task<bool> RemoveAwayMyOfferAsync(int OfferID)
        {
            dba = new HomeLibraryConnection();
            var offer = await dba.Offers.Where(i => i.OfferID == OfferID).SingleOrDefaultAsync();
            if (offer.Swap.OfferTransaction != 0)
            {
                offer.Swap.OfferTransaction = 0;
            }
            dba.Offers.Remove(offer);
            bool success = await dba.SaveChangesAsync() > 0;

            return success;
        }
        public async Task<bool> UpdateMyOfferManagerAsync(string parameters)
        {
            string[] parameter = parameters.Split(',');
            bool success = false;
            switch (parameter[1])
            {
                case "Swap":
                    success = await UpdateSwapMyOfferAsync(Convert.ToInt32(parameter[0]));
                    break;
                case "Sell":
                    success = await UpdateSellMyOfferAsync(Convert.ToInt32(parameter[0]));
                    break;
                case "Away":
                    success = await UpdateAwayMyOfferAsync(Convert.ToInt32(parameter[0]));
                    break;
            }
            return success;
        }
        private async Task<bool> UpdateSwapMyOfferAsync(int ProposeID)
        {
            dba = new HomeLibraryConnection();
            var propose = await dba.Proposes.Where(i => i.ProposeID == ProposeID).SingleOrDefaultAsync();
            propose.Swap.SwapTransaction = ProposeID;
            bool success = await dba.SaveChangesAsync() > 0;

            return success;
        }
        private async Task<bool> UpdateSellMyOfferAsync(int OfferID)
        {
            dba = new HomeLibraryConnection();
            var offer = await dba.Offers.Where(i => i.OfferID == OfferID).SingleOrDefaultAsync();
            offer.Swap.OfferTransaction = OfferID;
            bool success = await dba.SaveChangesAsync() > 0;

            return success;
        }
        private async Task<bool> UpdateAwayMyOfferAsync(int OfferID)
        {
            dba = new HomeLibraryConnection();
            var offer = await dba.Offers.Where(i => i.OfferID == OfferID).SingleOrDefaultAsync();
            offer.Swap.OfferTransaction = OfferID;
            bool success = await dba.SaveChangesAsync() > 0;

            return success;
        }
        public async Task<bool> CommitMyOfferManagerAsync(string parameters)
        {
            string[] parameter = parameters.Split(',');
            bool success = false;
            switch (parameter[1])
            {
                case "Swap":
                    success = await CommitSwapMyOfferAsync(Convert.ToInt32(parameter[0]));
                    break;
                case "Sell":
                    success = await CommitSellMyOfferAsync(Convert.ToInt32(parameter[0]));
                    break;
                case "Away":
                    success = await CommitAwayMyOfferAsync(Convert.ToInt32(parameter[0]));
                    break;
            }
            return success;
        }
        private async Task<bool> CommitSwapMyOfferAsync(int ProposeID)
        {
            dba = new HomeLibraryConnection();
            bool success = false;
            var SwapCommit = await dba.Proposes
                .Where(i => i.ProposeID == ProposeID)
                .Select(s => s.Swap)
                .FirstOrDefaultAsync();
            SwapCommit.Closed = true;
            var SwapsToRemove = SwapCommit.Proposes.Where(i => i.ProposeID != ProposeID);
            foreach (var p in SwapsToRemove.ToList())
            {
                dba.Proposes.Remove(p);
            }
            success = await dba.SaveChangesAsync() > 0;

            return success;
        }
        private async Task<bool> CommitSellMyOfferAsync(int OfferID)
        {
            dba = new HomeLibraryConnection();
            var SwapCommit = await dba.Offers
                .Where(i => i.OfferID == OfferID)
                .Select(s => s.Swap)
                .FirstOrDefaultAsync();
            SwapCommit.Closed = true;

            var OffersToRemove = SwapCommit.Offers.Where(i => i.OfferID != OfferID);
            foreach (var o in OffersToRemove)
            {
                dba.Offers.Remove(o);
            }
            bool success = await dba.SaveChangesAsync() > 0;

            return success;
        }
        private async Task<bool> CommitAwayMyOfferAsync(int OfferID)
        {
            dba = new HomeLibraryConnection();
            var SwapCommit = await dba.Offers
                .Where(i => i.OfferID == OfferID)
                .Select(s => s.Swap)
                .FirstOrDefaultAsync();
            SwapCommit.Closed = true;
            var OffersToRemove = SwapCommit.Offers.Where(i => i.OfferID != OfferID);
            foreach (var o in OffersToRemove)
            {
                dba.Offers.Remove(o);
            }
            bool success = await dba.SaveChangesAsync() > 0;

            return success;
        }
        //MyOffer
        public async Task<bool> RemoveOfferManagerAsync(string parameters)
        {
            string[] parameter = parameters.Split(',');
            bool success = false;
            switch (parameter[1])
            {
                case "Swap":
                    success = await RemoveSwapOfferAsync(Convert.ToInt32(parameter[0]));
                    break;
                case "Offer":
                    success = await RemoveOfferAsync(Convert.ToInt32(parameter[0]));
                    break;
            }
            return success;
        }
        private async Task<bool> RemoveOfferAsync(int OfferID)
        {
            dba = new HomeLibraryConnection();
            var offer = await dba.Offers.Where(i => i.OfferID == OfferID).SingleOrDefaultAsync();
            dba.Offers.Remove(offer);
            bool success = await dba.SaveChangesAsync() > 0;

            return success;
        }
        private async Task<bool> RemoveSwapOfferAsync(int ProposeID)
        {
            dba = new HomeLibraryConnection();
            var swapOffer = await dba.Proposes.Where(i => i.ProposeID == ProposeID).SingleOrDefaultAsync();
            dba.Proposes.Remove(swapOffer);
            bool success = await dba.SaveChangesAsync() > 0;

            return success;
        }
        public async Task<bool> CommitOfferManagerAsync(string parameters)
        {
            string[] parameter = parameters.Split(',');
            bool success = false;
            switch (parameter[1])
            {
                case "Swap":
                    success = await CommitSwapOfferAsync(Convert.ToInt32(parameter[0]));
                    break;
                case "Sell":
                    success = await CommitSellOfferAsync(Convert.ToInt32(parameter[0]));
                    break;
                case "Away":
                    success = await CommitAwayOfferAsync(Convert.ToInt32(parameter[0]));
                    break;
            }
            return success;
        }
        private async Task<bool> CommitSwapOfferAsync(int ProposeID)
        {
            dba = new HomeLibraryConnection();
            bool success = false;
            var ProposeCommit = await dba.Proposes
                .Where(i => i.ProposeID == ProposeID)
                .FirstOrDefaultAsync();
            ProposeCommit.Closed = true;

            success = await dba.SaveChangesAsync() > 0;

            return success;
        }
        private async Task<bool> CommitSellOfferAsync(int OfferID)
        {
            dba = new HomeLibraryConnection();
            var OfferCommit = await dba.Offers
                .Where(i => i.OfferID == OfferID)
                .FirstOrDefaultAsync();
            OfferCommit.Closed = true;

            bool success = await dba.SaveChangesAsync() > 0;

            return success;
        }
        private async Task<bool> CommitAwayOfferAsync(int OfferID)
        {
            dba = new HomeLibraryConnection();
            var OfferCommit = await dba.Offers
                .Where(i => i.OfferID == OfferID)
                .FirstOrDefaultAsync();
            OfferCommit.Closed = true;
            bool success = await dba.SaveChangesAsync() > 0;

            return success;
        }
        //CompletedDeals
        public async Task<dynamic> CompletedDealsListManagerAsync()
        {
            dynamic model = new ExpandoObject();
            model.Completed = await CompletedListMyOfferAsync();
            model.Sort = await dba.Sorts.ToListAsync();

            return model;
        }
        private async Task<List<PendingOffers>> CompletedListMyOfferAsync()
        {
            dba = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            var profileID = await dba.Profile.Where(n => n.Name == name).Select(i => i.ProfileID).SingleOrDefaultAsync();
            var offers = await dba.Offers
                .Where(i => (i.ProfileID == profileID || i.Swap.OwnerID == profileID) && i.OfferID == i.Swap.OfferTransaction
                 && ((i.Closed == true && i.Swap.Closed == true)))
                .ToListAsync();

            var proposes = await dba.Proposes
                .Where(i => (i.Book.ProfileID == profileID || i.Swap.OwnerID == profileID) && i.ProposeID == i.Swap.SwapTransaction
                 && ((i.Closed == true && i.Swap.Closed == true)))
                .ToListAsync();

            PendingOffers PO1 = new PendingOffers(offers);
            PendingOffers PO2 = new PendingOffers(proposes);

            var model = PO1.PendingOffersList.Union(PO2.PendingOffersList).ToList();

            return model;
        }
    }
} 