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
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var database = new AdsListingDbContext())
            {
                var ads = database
                    .Ads
                    .Include(a => a.Photos)
                    .Include(a => a.Location)
                    .ToList();

                return View(ads);
            }
            
        }

        public ActionResult ListCategories()
        {
            using (var database = new AdsListingDbContext())
            {
                var categories = database
                    .Categories
                    .Include(c => c.Ads)
                    .OrderBy(c => c.Name)
                    .ToList();

                return View(categories);
            }
        }

        public ActionResult ListAds(int? categoryId)
        {
            if (categoryId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new AdsListingDbContext())
            {
                var ads = database
                    .Ads
                    .Where(a => a.CategoryId == categoryId)
                    .Include(a => a.Author)
                    .ToList();

                return View(ads);
            }
        }
    }
}