using HomeLibrary.DAL;
using HomeLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HomeLibrary.Services
{
    public class AdditionalSignInServices
    {
        private BasicProfileInfoModels _profile;
        public async Task<BasicProfileInfoModels> ProfileCreatorAsync(string name)
        {
            HomeLibraryConnection db = new HomeLibraryConnection();
            List<bool> success = new List<bool>();
            success.Add(await UProfileInfoAsync(db, name));
            //success.Add(await UProfilePicturesAsync(db));

            if (success[0])
                return _profile;
            else
                return null;
        }
        //Update Db
        //private async Task<bool> UProfilePicturesAsync(HomeLibraryConnection db)
        //{
        //    ProfilePicturesModels picturesTemp = new ProfilePicturesModels
        //    {
        //        ProfileID = _profile.ProfileID
        //    };
        //    var pictures = db.Pictures.Add(picturesTemp);
        //    _pictures = pictures;
        //    bool result = await db.SaveChangesAsync() > 0;

        //    return result;
        //}
        //Update Db
        private async Task<bool> UProfileInfoAsync(HomeLibraryConnection db, string name) 
        {
            BasicProfileInfoModels profileTemp = new BasicProfileInfoModels
            {
                Name = name,
                Country = "unknown",
                City = "unknown",
                Quote = "unknown",
                FavouriteBook = "unknown",
                FavouriteBookCategory = "unknown",
                ProfilePic = "profileph.png"
            };

            var Profile = db.Profile.Add(profileTemp);
            _profile = Profile;
            bool result = await db.SaveChangesAsync() > 0;

            return result;
        }
    }
}