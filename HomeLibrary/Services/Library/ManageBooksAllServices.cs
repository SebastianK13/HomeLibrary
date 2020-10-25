using HomeLibrary.DAL;
using HomeLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HomeLibrary.Services
{
    public class ManageBooksAllServices
    {
        private List<bool> ListOfEmpties;//if true, field from form is empty
        private List<bool> ListOfEmptiesBook;// spliting ListOfEmpties on books field
        private List<bool> ListOfEmptiesLocation;// splitting ListOfEmpties on Locations fields
        private List<string> formFields;//contains form values 
        private List<string> formFieldsBook;//contains names of books fields
        private List<string> formFieldsLocation;//contains names of locations fields
        private string path { get; set; }
        HomeLibraryConnection db;
        private Book book;

        public string ServiceSwitcher(List<string> attributes)
        {
            if (attributes[0] == "Remove")
            {

                return "Remove";
            }
            else if (attributes[0] == "Modify")
            {

                return "ModifyBook";
            }
            else
            {

                return "ShowBook";
            }

        }
        public async Task<bool> RemoveService(string bookId)
        {
            return await RemoveBook(bookId);

        }
        private async Task<bool> RemoveBook(string bookId)
        {
            db = new HomeLibraryConnection();
            var id = Convert.ToInt32(bookId);
            book = db.Books.Where(p => p.BookID == id).Single();

            return await RemoveBookChildren(); 
        }
        private async Task<bool> RemoveBookChildren()
        {
            List<bool> success = new List<bool>();

            var ownProposes = await db.Proposes.Where(i => i.ProposeID == book.BookID).FirstOrDefaultAsync();

            success.Add(await RemoveOfferAsync());
            success.Add(await RemoveProposeAsync(ownProposes));
            success.Add(await RemoveSwapAsync());
            success.Add(await RemoveLocationAsync());
            success.Add(await RemoveStateModelsAsync());//may couse a problem added not tested(tested-works)

            return await db.SaveChangesAsync() > 0;
        }
        private async Task<bool> RemoveSwapAsync()
        {
            try
            {
                db.Swaps.Remove(book.Swap);
            }
            catch(ArgumentNullException)
            {
                return false;
            }

            return await db.SaveChangesAsync()>0;
        }
        private async Task<bool> RemoveProposeAsync(SwapPropose propose)
        {
            if(book.Swap != null)
            {
                try
                {
                    foreach (var p in book.Swap.Proposes.ToList())
                    {
                        db.Proposes.Remove(p);
                    }
                }
                catch (NullReferenceException)
                {
                    return false;
                }

            }

            if(propose != null)
            {
                db.Proposes.Remove(propose);
            }


            return await db.SaveChangesAsync() > 0;


        }
        private async Task<bool> RemoveLocationAsync()
        {
            try
            {
                db.Locations.Remove(book.Location);
            }
            catch (ArgumentNullException)
            {
                return false;
            }
            return await db.SaveChangesAsync() > 0;
        }
        private async Task<bool> RemoveStateModelsAsync()
        {
            var state = await db.BookState.Where(i => i.StateID == book.StateID).SingleOrDefaultAsync();
            if (state != null)
            {
                try
                {
                    db.BookState.Remove(state);
                }
                catch (NullReferenceException)
                {
                    return false;
                }
                return await db.SaveChangesAsync() > 0;
            }
            else
            {
                return false;
            }
        }
        private async Task<bool> RemoveOfferAsync()
        {
            if(book.Swap != null)
            {
                try
                {
                    foreach (var o in book.Swap.Offers.ToList())
                    {
                        db.Offers.Remove(o);
                    }

                }
                catch (ArgumentNullException)
                {
                    return false;
                }
                return await db.SaveChangesAsync() > 0;
            }
            else
            {
                return false;
            }
        }

        public Book ShowChoosenBook(string bookId)
        {
            HomeLibraryConnection db = new HomeLibraryConnection();
            Book book = new Book();
            var id = Convert.ToInt32(bookId);
            book = db.Books.Where(b => b.BookID == id).Single();

            return book;
        }
        public Location ShowBookLocation(string bookId)
        {
            HomeLibraryConnection db = new HomeLibraryConnection();
            Location location = new Location();
            Book book = new Book();
            var id = Convert.ToInt32(bookId);
            var locId = db.Books.Single(b => b.BookID == id).LocationID;
            location = db.Locations.Where(l => l.LocationID == locId).Single();

            return location;
        }
        public async Task<bool> ModifyBookInfo(FormCollection collection, List<bool> isEmpty)
        {
            ListOfEmpties = isEmpty;
            FormFieldsSetter(collection);
            EmptyFormField(collection);
            var bookId = Convert.ToInt32(formFields[9]);
            var book = SelectBookAsync(bookId);
            Book book1 = await book;
            var update = UpdateBookAsync(book1, collection);
            return await update;
        }
        private async Task<Book> SelectBookAsync(int bookId)
        {
            HomeLibraryConnection db = new HomeLibraryConnection();
            var entity = db.Books.Where(b => b.BookID == bookId);
            Book book = await entity.SingleOrDefaultAsync();
            return book;
        }

        private async Task<bool> UpdateBookAsync(Book book1, FormCollection collection)
        {
            HomeLibraryConnection db = new HomeLibraryConnection();

            Book book = new Book
            {
                BookID = book1.BookID,
                Title = formFields[0],
                Category = formFields[1],
                Description = formFields[8],
                Author = formFields[2],
                Language = formFields[3],
                Pages = Convert.ToInt32(formFields[4]),
                ImageSource = path
            };

            //path = book.ImageSource;
            DbSet dbSet = db.Set(book.GetType());
            dbSet.Attach(book);
            var b = db.Entry(book);

            int c = 0;

            foreach (var e in ListOfEmptiesBook)
            {
                if (!e)
                {
                    b.Property(formFieldsBook[c]).IsModified = true;
                }
                c++;
            }
            c = 0;

            if (!book1.ImageSource.Equals(path))
            {
                b.Property("ImageSource").IsModified = true;
            }

            bool success = await db.SaveChangesAsync() > 0;

            Location location = new Location
            {
                LocationID = book1.LocationID,
                BookstandDesignation = formFields[5],
                ShelfNumber = Convert.ToInt32(formFields[6]),
                BookPosition = Convert.ToInt32(formFields[7])
            };
            db.Entry(book).State = EntityState.Detached;
            dbSet = db.Set(location.GetType());
            dbSet.Attach(location);
            var l = db.Entry(location);

            foreach (var e in ListOfEmptiesLocation)
            {
                if (!e)
                {
                    l.Property(formFieldsLocation[c]).IsModified = true;
                }
                c++;
            }

            success = await db.SaveChangesAsync() > 0;
            return success;
        }
        public dynamic BookInfoProvider(string bookId)
        {

            Book book = new Book();
            Location location = new Location();
            book = ShowChoosenBook(bookId);
            location = ShowBookLocation(bookId);
            dynamic modelNew = new ExpandoObject();
            modelNew.Book = book;
            modelNew.Location = location;

            return modelNew;
        }
        //Checking if field is empty or not. Gives a properly flag. Need to validator in controller
        public List<bool> EmptyFormField(FormCollection collection)
        {
            List<bool> empty = new List<bool>();
            string t = "";
            foreach (var f in collection)
            {
                if (Equals(collection[f.ToString()], t))
                    empty.Add(true);
                else
                    empty.Add(false);
            }

            empty.RemoveAt(0);
            empty.RemoveAt(9);

            ListOfEmptiesBook = empty.GetRange(0, 5);
            ListOfEmptiesBook.Add(empty[8]);
            ListOfEmptiesLocation = empty.GetRange(5, 3);

            return empty;
        }
        //Creating a list of fields names from Form - easy to use
        public List<string> CreateListField(FormCollection collection)
        {
            List<string> fieldsList = new List<string>();

            foreach (var f in collection)
            {
                fieldsList.Add(f.ToString());
            }
            fieldsList.RemoveAt(0);
            fieldsList.RemoveAt(9);

            formFieldsBook = fieldsList.GetRange(0, 5);
            formFieldsBook.Add(fieldsList[8]);
            formFieldsLocation = fieldsList.GetRange(5, 3);

            return fieldsList;
        }
        //Refactor for Check() and NameSeparator() -> formFields
        private void FormFieldsSetter(FormCollection form)
        {
            formFields = new List<string>();

            for (int i = 0; i < ListOfEmpties.Count(); i++)
            {
                if (ListOfEmpties[i])
                {
                    formFields.Add("0");
                }
                else
                    formFields.Add(form[i + 1]);
            }
            formFields.Add(form[10]);
        }
        public void ChangeImgPath(HttpPostedFileBase file, List<string> serverImgPath)
        {
            var bookId = Convert.ToInt32(serverImgPath[3]);
            HomeLibraryConnection db = new HomeLibraryConnection();
            var entity = db.Books.Where(b => b.BookID == bookId);
            Book book = entity.SingleOrDefault();


            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                file.SaveAs(serverImgPath[1] + fileName);
                path = /*"~/Images/"+*/serverImgPath[0] + @"/" + fileName;
            }
            else
            {
                path = serverImgPath[2];
            }
        }
        public string ViewDataSetter(HttpPostedFileBase file, List<string> serverImagePath)
        {
            if (file != null)
            {
                var fileName = Path.GetFileName(file.FileName);
                var t1 = HttpContext.Current.User.Identity.Name;
                var path1 = Path.Combine(HttpContext.Current.Server.MapPath("~/Images/" + t1 + "/" + fileName));
                var path2 = t1 + "/" + fileName;

                file.SaveAs(path1);
                return path2;
            }
            else
            {
                return serverImagePath[2];
            }
        }
        public List<string> ImagePathSetter(FormCollection form)
        {
            List<string> serverImagePath = new List<string>();
            serverImagePath.Add(HttpContext.Current.User.Identity.Name);
            var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Images/" + serverImagePath[0] + "/"));
            serverImagePath.Add(path);
            serverImagePath.Add(form[11]);
            serverImagePath.Add(form[10]);

            return serverImagePath;
        }
        public List<string> FieldsValue(FormCollection form, List<bool> isEmpty = null)
        {
            List<string> formValues;//contains form values 
            formValues = new List<string>();
            if (isEmpty != null)
                ListOfEmpties = isEmpty;

            for (int i = 0; i < ListOfEmpties.Count(); i++)
            {
                if (ListOfEmpties[i])
                {
                    formValues.Add("");
                }
                else
                    formValues.Add(form[i + 1]);
            }
            formValues.Add(form[10]);

            return formValues;
        }

    }
}