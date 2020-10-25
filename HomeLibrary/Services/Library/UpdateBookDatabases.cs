using HomeLibrary.DAL;
using HomeLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeLibrary.Services
{
    public class UpdateBookDatabases
    {
        private HomeLibraryConnection db;
        private List<string> bookInputs { get; set; }
        private List<string> locationInputs { get; set; }
        private string sourceImg { get; set; }
        private int locationId { get; set; }
        private int bookStateID { get; set; }
        private string userName { get; set; }
        private string path { get; set; }

        //Add book location info to dbo.Location, add book info to dbo.Book
        public void InputsSplitter(FormCollection collection, HttpPostedFileBase file, List<string> dbFields)
        {
            userName = dbFields[0];
            path = dbFields[1];
            int emptyFields = 0;
            bookInputs = new List<string>();
            locationInputs = new List<string>();
            for (int i = 1; i < collection.Keys.Count; i++)
            {
                if (collection[i] == "")
                    emptyFields++;
                if (i > 5 & i < 9)
                {
                    locationInputs.Add(collection[i]);
                }
                else
                {
                    bookInputs.Add(collection[i]);
                }
            }
            if(emptyFields == 0)
            {
                UpdateLocation();
                SaveBookImg(file);
                UpdateBookState();
                UpdateBookDb();                
            }

        }
        private void UpdateLocation()
        {
            db = new HomeLibraryConnection();
            var location = db.Locations;

            location.Add(new Location
            {
                BookstandDesignation = locationInputs[0],
                ShelfNumber = Convert.ToInt32(locationInputs[1]),
                BookPosition = Convert.ToInt32(locationInputs[2])
            });
            db.SaveChanges();
            var index = db.Locations.OrderBy(p => p.LocationID).GroupBy(p => p.LocationID).ToList().Last();
            locationId = index.Select(c => c.LocationID).Single();
        }
        private void SaveBookImg(HttpPostedFileBase file)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);               
                file.SaveAs(path+fileName);
                sourceImg = userName+@"/"+ fileName;
            }
            else
            {
                sourceImg = "default_img.png";
            }
        }
        private void UpdateBookDb()
        {
            var book = db.Books;
            string name = HttpContext.Current.User.Identity.Name;
            var profileID = db.Profile.Where(i => i.Name == name).Select(p => p.ProfileID).Single();

            book.Add(new Book
            {
                Title = bookInputs[0],
                Category = bookInputs[1],
                Author = bookInputs[2],
                Language = bookInputs[3],
                Pages = Convert.ToInt32(bookInputs[4]),
                Description = bookInputs[5],
                ImageSource = sourceImg,
                LocationID = locationId,
                StateID = bookStateID,
                UserName = userName,
                ProfileID = profileID
            });
            db.SaveChanges();
        }
        private void UpdateBookState()
        {
            var state = db.BookState;

            state.Add(new StateModels 
            { 
                OverallStatus = true,
                LendStatus = false,
                BorrowedStatus = false,

            });

            db.SaveChanges();

            bookStateID = db.BookState
                .OrderBy(i => i.StateID)
                .GroupBy(i => i.StateID)
                .ToList()
                .Last()
                .Select(t => t.StateID)
                .Single();
        }
    }
}
