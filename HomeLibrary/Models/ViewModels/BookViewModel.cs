using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace HomeLibrary.Models.ViewModels
{
    public class BookView
    {
        public BookView()
        {

        }

        public BookView(List<Book> books)
        {
            BookViews = new List<BookView>();
            foreach (var b in books)
            {
                BookViews.Add(new BookView()
                {
                    BookID = b.BookID,
                    Title = b.Title,
                    Category = b.Category,
                    Author = b.Author,
                    Language = b.Language,
                    Description = b.Description,
                    Pages = b.Pages,
                    Rate = b.Rate,
                    ImageSource = b.ImageSource
                });
            }
        }
        public List<BookView> BookViews;
        public int BookID { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Language { get; set; }
        public int Pages { get; set; }
        public int Rate { get; set; }
        public string ImageSource { get; set; }
    }
}