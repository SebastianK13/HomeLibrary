using HomeLibrary.DAL;
using HomeLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace HomeLibrary.Services.ManageServices
{
    public class InformationManagerServices
    {
        private HomeLibraryConnection db;
        private string Value;

        public async Task<BasicProfileInfoModels> SelectProfileInfoAsync()
        {
            string name = HttpContext.Current.User.Identity.Name;
            db = new HomeLibraryConnection();
            var t = db.Profile.ToList();
            var BPI = await db.Profile.Where(n => n.Name == name).SingleOrDefaultAsync();

            return BPI;
        }
        public async Task<bool> ProfileUpdateManager(string fieldName, string newValue)
        {
            Value = newValue;
            var t = await FieldSwitcher(fieldName);

            return t;
        }
        private async Task<bool> FieldSwitcher(string fieldName)
        {
            var result = fieldName.Replace("-li", "").Replace("#", "");
            bool success = false;
            switch (result)
            {
                case "Nickname":
                    success = await UpdateProfileInfoAsync(result);
                    break;
                case "Age":
                    success = await UpdateProfileInfoAsync(result);
                    break;
                case "Country":
                    success = await UpdateProfileInfoAsync(result);
                    break;
                case "City":
                    success = await UpdateProfileInfoAsync(result);
                    break;
                case "Quote":
                    success = await UpdateProfileInfoAsync(result);
                    break;
                case "Category":
                    success = await UpdateProfileInfoAsync("FavouriteBookCategory");
                    break;
                case "Book":
                    success = await UpdateProfileInfoAsync("FavouriteBook");
                    break;
                default:
                    break;
            }
            return success;
        }
        private async Task<bool> UpdateProfileInfoAsync(string finalFieldName)
        {
            var profile = await SelectProfileInfoAsync();
            DbSet dbSet = db.Set(profile.GetType());
            dbSet.Attach(profile);
            var b = db.Entry(profile);
            if (finalFieldName.Equals("Age"))
            {
                b.Property(finalFieldName).CurrentValue = Convert.ToInt32(Value);
            }
            else
            {
                b.Property(finalFieldName).CurrentValue = Value;
            }
            b.Property(finalFieldName).IsModified = true;
            var result = await db.SaveChangesAsync() > 0;

            return result;
        }
    }
}