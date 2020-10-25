using HomeLibrary.DAL;
using HomeLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HomeLibrary.Services.Community
{
    public class ProfileServices
    {
        private HomeLibraryConnection db;
        private HttpPostedFileBase file;
        private string sourceImg;
        private string name;

        public async Task<List<ProfilePicturesModels>> PhotosManager()
        {
            var model = await SelectUserProfile();           

            return model;
        }
        private async Task<List<ProfilePicturesModels>> SelectUserProfile()
        {
            db = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            var profileID = await db.Profile.Where(n => n.Name == name).Select(p => p.ProfileID).SingleAsync();
            var pictures = await db.Pictures.Where(i => i.ProfileID == profileID).ToListAsync();

            return pictures;
        }
        //update photos in db
        public async Task<bool> PicturesManagerAsync(HttpPostedFileBase file)
        {
            this.file = file;
            bool success = false;
            SavePicture();
            success = await UpdatePicturesAsync();

            return success;
        }
        //Update Db
        private async Task<bool> UpdatePicturesAsync()
        {
            db = new HomeLibraryConnection();
            var profileId = await db.Profile.Where(n => n.Name == name).Select(p=>p.ProfileID).SingleAsync();

            ProfilePicturesModels picturesTemp = new ProfilePicturesModels
            {
                PicturePath = sourceImg,
                ProfileID = profileId
            };
            var pictures = db.Pictures.Add(picturesTemp);
            bool result = await db.SaveChangesAsync() > 0;

            return result;
        }
        private void SavePicture()
        {
            name = HttpContext.Current.User.Identity.Name;
            var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Images/" + name + "/pictures/"));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                file.SaveAs(path + fileName);
                sourceImg = name + "/pictures/" + fileName;
            }
            else
            {
                //sourceImg = "default_img.png";
            }
        }
        public async Task<bool> RemovePcituresAsync(string choseToRemove)
        {
            List<string> Pictures = new List<string>();
            Pictures = choseToRemove.Split(',').ToList();
            Pictures.RemoveAt(Pictures.Count-1);
            var PicturesIDs = Pictures.Select(int.Parse).ToList();
            bool result = await RemovePicsDatabase(PicturesIDs);

            return result;
        }
        private async Task<bool> RemovePicsDatabase(List<int> PicturesIDs)
        {
            db = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Images/"));
            int profileId = await db.Profile.Where(n => n.Name == name).Select(i=>i.ProfileID).SingleAsync();
            List<string> PicturesPath = new List<string>();

            foreach (var i in PicturesIDs)
            {
                var picturePath = await db.Pictures.Where(p => p.PictureID == i).Select(v => v.PicturePath).SingleAsync();
                var temp = await db.Pictures.Where(id => id.PictureID == i).SingleAsync();
                db.Pictures.Remove(temp);
                File.Delete(path + picturePath);
            }
            bool result = await db.SaveChangesAsync() > 0;

            return result;
        }
        public async Task<bool> SetPicture(int bckgPic, int profilePic)
        {
            bool result = false;

            if(bckgPic != -1)
            {
                result = await SetBckgImage(bckgPic);
            }
            else if (profilePic != -1)
            {
                result = await SetProfilePic(profilePic);
            }

            return false;
        }
        private async Task<bool> SetBckgImage(int picture)
        {
            db = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            var picturePath = await db.Pictures.Where(i => i.PictureID == picture).Select(p => p.PicturePath).SingleAsync();
            var Profile = await db.Profile.Where(n => n.Name == name).SingleAsync();

            Profile.BackgroundPic = picturePath;

            DbSet dbSet = db.Set(Profile.GetType());
            dbSet.Attach(Profile);
            var b = db.Entry(Profile);

            b.Property("BackgroundPic").IsModified = true;

            bool result = await db.SaveChangesAsync()>0;
            return result;
        }
        private async Task<bool> SetProfilePic(int picture)
        {

            db = new HomeLibraryConnection();
            string name = HttpContext.Current.User.Identity.Name;
            var picturePath = await db.Pictures.Where(i => i.PictureID == picture).Select(p => p.PicturePath).SingleAsync();
            var Profile = await db.Profile.Where(n => n.Name == name).SingleAsync();

            Profile.ProfilePic = picturePath;

            DbSet dbSet = db.Set(Profile.GetType());
            dbSet.Attach(Profile);
            var b = db.Entry(Profile);

            b.Property("ProfilePic").IsModified = true;

            bool result = await db.SaveChangesAsync() > 0;
            return result;
        }
        public async Task<BasicProfileInfoModels> SelectProfile()
        {
            db = new HomeLibraryConnection();
            var name = HttpContext.Current.User.Identity.Name;
            var model = await db.Profile.Where(n => n.Name == name).SingleAsync();

            return model;
        }
    }
}