using HomeLibrary.DAL;
using HomeLibrary.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;

namespace HomeLibrary.Services
{
    public class UsersBooks
    {
        private string imagePath{get;set;}
        public dynamic LoadUsersBooks(HomeLibraryConnection db, string name, string mainPath)
        {
            
            Sort sort = new Sort();
            List<Book> usersBooks = new List<Book>();
            var books = db.Books.ToList();
            var sortList = db.Sorts.ToList();
            var status = db.BookState.ToList();
            string temp;
            foreach(var b in books)
            {
                if (b.UserName == name)
                {
                    usersBooks.Add(b);
                    temp = b.ImageSource/*.Replace("/", "\\")*/;
                    imagePath = Path.Combine(mainPath + temp);
                    //imagePath = imagePath.Replace(@"\", @"\\");                  
                    if (!CheckImageExistance())
                    {
                        b.ImageSource = "default_img.png";
                        imagePath = " ";
                        db.SaveChanges();
                    }
                }
            }
            var list = Status(db, usersBooks);
            dynamic model = new ExpandoObject();
            model.Book = usersBooks;
            model.Sort = sortList;
            model.BookState = list;

            return model;
        }
        public bool CheckImageExistance()
        {
            if (!File.Exists(imagePath))
            {
                return false;
            }

            return true;
        }
        private List<StateModels> Status(HomeLibraryConnection db, List<Book> books)
        {
            List<StateModels> state = new List<StateModels>();
            var states = db.BookState.ToList();

            foreach(var b in books)
            {
                var t = states.Where(m => m.StateID == b.StateID).FirstOrDefault();
                if(t != null)
                {
                    state.Add(t);
                }
                else
                {
                    state.Add(null);
                }
                
            }
            return state;
        }
        public List<int> BookStatusAmount(HomeLibraryConnection db)
        {
            List<int> StatusAmount = new List<int> {0,0,0};
            var name = HttpContext.Current.User.Identity.Name;
            var StatusIDs = db.Books.Where(u=>u.UserName == name).Select(s=>s.StateID).ToList();
            var States = db.BookState.ToList();
            int c = 0;
            List<StateModels> StatusList = new List<StateModels>();

            foreach(var id in StatusIDs)
            {
                StatusList.Add(States.Where(i => i.StateID == id).Single());

                if (StatusList[c].LendStatus)
                    StatusAmount[1]++;
                else if (StatusList[c].BorrowedStatus)
                    StatusAmount[2]++;
                else
                    StatusAmount[0]++;

                c++;
            }           

            return StatusAmount;
        }
    }
}