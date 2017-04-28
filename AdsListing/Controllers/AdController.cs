using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AdsListing.Models;
using System.IO;

namespace AdsListing.Controllers
{
    public class AdController : Controller
    {
        // GET: Ad
        public ActionResult Index()
        {
            return RedirectToAction("ListAds", "Home", new { area = "" });
        }
        // GET: Ad/List
        public ActionResult List()
        {
            using (var database = new AdsListingDbContext())
            {
                // Get Ads from the database
                var ads = database
                    .Ads
                    .Include(a => a.Author)
                    .Include(a => a.Photos)
                    .ToList();

                return View(ads);
            }
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new AdsListingDbContext())
            {
                //Get the ad from the database
                var ad = database
                    .Ads
                    .Where(a => a.Id == id)
                    .Include(a => a.Photos)
                    .Include(a => a.Author)
                    .Include(c => c.Category)
                    .Include(l => l.Location)
                    .First();

                if (ad == null)
                {
                    return HttpNotFound();
                }

                return View(ad);
            }
        }

        //GET: Ad/Create
        [Authorize]
        public ActionResult Create()
        {
            using (var database = new AdsListingDbContext())
            {
                var model = new AdViewModel
                {
                    Categories = database
                        .Categories
                        .OrderBy(c => c.Name)
                        .ToList(),
                    Locations = database
                        .Locations
                        .OrderBy(l => l.Name)
                        .ToList()
                };

                return View(model);
            }
        }

        //POST: Ad/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(AdViewModel model, IEnumerable<HttpPostedFileBase> images)
        {
            using (var database = new AdsListingDbContext())
            {
                if (ModelState.IsValid)
                {

                    //Get Author Id
                    var authorId = database
                        .Users
                        .Where(u => u.UserName == this.User.Identity.Name)
                        .First()
                        .Id;

                    var ad = new Ad(authorId, model.Title, model.Description, model.CategoryId, model.LocationId, model.Price, model.ContactNumber);
                    //Set Ads Author
                    ad.AuthorId = authorId;

                    //Save Ad in the DB
                    database.Ads.Add(ad);
                    database.SaveChanges();

                    if (images.Count() != 0 && images.FirstOrDefault() != null)
                    {
                        var photo = new Photo();

                        var allowedImageTypes = new[] { "image/jpeg", "image/jpg", "image/png" };

                        foreach (var image in images)
                        {
                            if (allowedImageTypes.Contains(image.ContentType))
                            {
                                if (image.ContentLength == 0) continue;

                                var fileName = Guid.NewGuid().ToString();
                                var extension = "";

                                if (Path.GetExtension(image.FileName) == null)
                                {
                                    extension = "add";
                                }
                                else
                                {
                                    extension = Path.GetExtension(image.FileName).ToLower();
                                }

                                using (var img = Image.FromStream(image.InputStream))
                                {
                                    photo.ThumbPath = String.Format("/Content/Images/Thumbs/{0}{1}", fileName, extension);
                                    photo.ImagePath = String.Format("/Content/Images/Original/{0}{1}", fileName, extension);

                                    SaveToFolder(img, fileName, extension, new Size(180, 120), photo.ThumbPath);
                                    SaveToFolder(img, fileName, extension, new Size(600, 600), photo.ImagePath);
                                }
                                photo.AdId = ad.Id;
                                database.Photos.Add(photo);
                                database.SaveChanges();
                            }
                        }
                    }

                    ad.Photos = database
                        .Photos
                        .Where(p => p.AdId == ad.Id)
                        .ToList();

                    return RedirectToAction("Details", new { id = ad.Id });
                }


                model.Categories = database
                    .Categories
                    .OrderBy(c => c.Name)
                    .ToList();
                model.Locations = database
                    .Locations
                    .OrderBy(l => l.Name)
                    .ToList();

                return View(model);
            }
        }

        // Get: Ad/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new AdsListingDbContext())
            {
                //Get Ad from the DB
                var ad = database
                    .Ads
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();

                if (!IsUserAuthorizedToEdit(ad))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                //Check if the Ad exists
                if (ad == null)
                {
                    return HttpNotFound();
                }

                //Redirect to the Index Page
                return View(ad);
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new AdsListingDbContext())
            {
                //Get the Ad from the DB
                var ad = database
                    .Ads
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();

                //Check if ad exists
                if (ad == null)
                {
                    return HttpNotFound();
                }

                //Remove ad from the DB
                database.Ads.Remove(ad);
                database.SaveChanges();

                //Redirect to Index page
                return RedirectToAction("Index");
            }
        }

        //GET: Ad/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new AdsListingDbContext())
            {
                var ad = database
                    .Ads
                    .Where(a => a.Id == id)
                    .First();

                if (!IsUserAuthorizedToEdit(ad))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                if (ad == null)
                {
                    return HttpNotFound();
                }

                var photos = database
                    .Photos
                    .Where(p => p.AdId == id)
                    .ToList();

                var model = new AdViewModel
                {
                    Id = ad.Id,
                    Title = ad.Title,
                    Description = ad.Description,
                    Price = ad.Price,
                    CategoryId = ad.CategoryId,
                    Categories = database
                        .Categories
                        .OrderBy(c => c.Name)
                        .ToList(),
                    LocationId = ad.LocationId,
                    Locations = database
                        .Locations
                        .OrderBy(l => l.Name)
                        .ToList(),
                    Status = ad.Status,
                    ContactNumber = ad.ContactNumber,
                    Photos = photos
                };

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Edit(AdViewModel model, IEnumerable<HttpPostedFileBase> images, int[] deleteInputs)
        {
            using (var database = new AdsListingDbContext())
            {
                //Check if model state is valid
                if (ModelState.IsValid)
                {

                    //Get ad from the DB
                    var ad = database
                        .Ads
                        .FirstOrDefault(a => a.Id == model.Id);

                    //Set article properties
                    ad.Title = model.Title;
                    ad.Description = model.Description;
                    ad.Price = model.Price;
                    ad.CategoryId = model.CategoryId;
                    ad.LocationId = model.LocationId;
                    ad.Status = model.Status;

                    //Set article state in DB
                    database.Entry(ad).State = EntityState.Modified;
                    database.SaveChanges();

                    if (images.Count() != 0 && images.FirstOrDefault() != null)
                    {
                        var photo = new Photo();

                        var allowedImageTypes = new[] { "image/jpeg", "image/jpg", "image/png" };

                        foreach (var image in images)
                        {
                            if (allowedImageTypes.Contains(image.ContentType))
                            {
                                if (image.ContentLength == 0) continue;

                                var fileName = Guid.NewGuid().ToString();

                                var extension = Path.GetExtension(image.FileName).ToLower();

                                using (var img = Image.FromStream(image.InputStream))
                                {

                                    photo.ThumbPath = String.Format("/Content/Images/Thumbs/{0}{1}", fileName, extension);
                                    photo.ImagePath = String.Format("/Content/Images/Original/{0}{1}", fileName, extension);


                                    SaveToFolder(img, fileName, extension, new Size(100, 100), photo.ThumbPath);
                                    SaveToFolder(img, fileName, extension, new Size(600, 600), photo.ImagePath);
                                }

                                photo.AdId = ad.Id;
                                database.Photos.Add(photo);
                                database.SaveChanges();
                            }
                        }
                    }

                    if (deleteInputs != null && deleteInputs.Length > 0)
                    {
                        DeletePhotos(deleteInputs, model.Id);
                    }

                    ad.Photos = database
                        .Photos
                        .Where(p => p.AdId == ad.Id)
                        .ToList();

                    return RedirectToAction("Details", new { id = ad.Id });
                }

                model.Categories = database
                        .Categories
                        .OrderBy(c => c.Name)
                        .ToList();
                model.Locations = database
                    .Locations
                    .OrderBy(l => l.Name)
                    .ToList();

                model.Photos = database
                        .Photos
                        .Where(p => p.AdId == model.Id)
                        .ToList();

                return View(model);
            }
        }

        public ActionResult AdminList()
        {
            using (var database = new AdsListingDbContext())
            {
                var ads = database.Ads.ToList();

                return View(ads);
            }
        }

        private bool IsUserAuthorizedToEdit(Ad ad)
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isAuthor = ad.IsAuthor(this.User.Identity.Name);

            return isAdmin || isAuthor;
        }

        public Size NewImageSize(Size imageSize, Size newSize)
        {
            Size finalSize;
            double tempval;
            if (imageSize.Height > newSize.Height || imageSize.Width > newSize.Width)
            {
                if (imageSize.Height > imageSize.Width)
                {
                    tempval = newSize.Height / (imageSize.Height * 1.0);
                }
                else
                {
                    tempval = newSize.Width / (imageSize.Width * 1.0);
                }


                finalSize = new Size((int)(tempval * imageSize.Width), (int)(tempval * imageSize.Height));
            }
            else
                finalSize = imageSize; // image is already small size

            return finalSize;
        }

        private void SaveToFolder(Image img, string fileName, string extension, Size newSize, string pathToSave)
        {
            // Get new resolution
            Size imgSize = NewImageSize(img.Size, newSize);
            using (Image newImg = new Bitmap(img, imgSize.Width, imgSize.Height))
            {
                newImg.Save(Server.MapPath(pathToSave), img.RawFormat);
            }
        }

        private void DeletePhotos(int[] photos, int id)
        {
            using (var database = new AdsListingDbContext())
            {
                var adPhotos = database
                    .Photos
                    .Where(p => p.AdId == id)
                    .ToList();

                foreach (var adPhoto in adPhotos)
                {
                    foreach (var photo in photos)
                    {
                        if (adPhoto.Id == photo)
                        {
                            //string fullPathToRemove = Request.MapPath(adPhoto.ThumbPath);

                            database.Photos.Remove(adPhoto);
                            //if (System.IO.File.Exists(adPhoto.ThumbPath))
                            //{
                            //    System.IO.File.Delete(adPhoto.ThumbPath);
                            //}
                            database.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}