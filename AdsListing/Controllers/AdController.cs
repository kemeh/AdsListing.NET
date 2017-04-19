using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AdsListing.Models;

namespace AdsListing.Controllers
{
    public class AdController : Controller
    {
        // GET: Ad
        public ActionResult Index()
        {
            return RedirectToAction("List");
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
                var ad = database.Ads.Where(a => a.Id == id).Include(a => a.Author).First();

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
                var model = new AdViewModel();
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

        //POST: Ad/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(AdViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var database = new AdsListingDbContext())
                {
                    //Get Author Id
                    var authorId = database
                        .Users
                        .Where(u => u.UserName == this.User.Identity.Name)
                        .First()
                        .Id;

                    var ad = new Ad(authorId, model.Title, model.Description, model.CategoryId, model.LocationId, model.Price);
                    //Set Ads Author
                    ad.AuthorId = authorId;

                    //Save Ad in the DB
                    database.Ads.Add(ad);
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(model);
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

                var model = new AdViewModel();
                model.Id = ad.Id;
                model.Title = ad.Title;
                model.Description = ad.Description;
                model.Price = ad.Price;
                model.CategoryId = ad.CategoryId;
                model.Categories = database
                    .Categories
                    .OrderBy(c => c.Name)
                    .ToList();
                model.LocationId = ad.LocationId;
                model.Locations = database
                    .Locations
                    .OrderBy(l => l.Name)
                    .ToList();

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Edit(AdViewModel model)
        {
            //Check if model state is valid
            if (ModelState.IsValid)
            {
                using (var database = new AdsListingDbContext())
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

                    //Set article state in DB
                    database.Entry(ad).State = EntityState.Modified;
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        private bool IsUserAuthorizedToEdit(Ad ad)
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isAuthor = ad.IsAuthor(this.User.Identity.Name);

            return isAdmin || isAuthor;
        }
    }
}