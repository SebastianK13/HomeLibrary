using HomeLibrary.DAL;
using HomeLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
 
namespace HomeLibrary.Services
{
    public class ChangeBookStatus
    {
        private List<bool> StatusList;
        public void ChangeBookStatusManager(FormCollection collection)
        {
            string s = collection.AllKeys[0];
            string s2 = collection.AllKeys[1];
            var bookInfo = BasicBookInfo(s, s2);
            UpdateBookStatus(bookInfo, collection);
        }
        //splitting information which collection form contains
        private List<string> BasicBookInfo(string s, string s2)
        {
            List<string> bookInfo = new List<string>();
            bookInfo.Add(Regex.Replace(s, @"[A-Za-z ]", ""));
            if (s2.Contains("available"))
            {
                bookInfo.Add("available");
                StatusList = new List<bool> { true, false, false };
            }
            else if (s2.Contains("lend"))
            {
                bookInfo.Add("lend");
                StatusList = new List<bool> { false, true, false };
            }
            else if (s2.Contains("borrowed"))
            {
                bookInfo.Add("borrowed");
                StatusList = new List<bool> { false, false, true };
            }

            return bookInfo;
        }
        private void UpdateBookStatus(List<string> bI, FormCollection collection)
        {
            HomeLibraryConnection db = new HomeLibraryConnection();
            int temp = Convert.ToInt32(bI[0]);
            var StatusID = db.Books.Where(m => m.BookID == temp).Select(n => n.StateID).Single();

            StateModels BookStatus = new StateModels
            {
                StateID = Convert.ToInt32(StatusID),
                OverallStatus = StatusList[0],
                LendStatus = StatusList[1],
                BorrowedStatus = StatusList[2],
            };

            DbSet dbSet = db.Set(BookStatus.GetType());
            dbSet.Attach(BookStatus);
            var b = db.Entry(BookStatus);

            b.Property("OverallStatus").IsModified = true;
            b.Property("LendStatus").IsModified = true;
            b.Property("BorrowedStatus").IsModified = true;

            db.SaveChanges();
        }

    }
}