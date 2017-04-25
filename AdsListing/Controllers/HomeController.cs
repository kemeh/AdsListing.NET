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
            //using (var database = new AdsListingDbContext())
            //{
            //    var ads = database
            //        .Ads
            //        .Include(a => a.Photos)
            //        .Include(a => a.Location)
            //        .Take(26)
            //        .ToList();

            //    return View(ads);
            //}

            return RedirectToAction("ListAds");

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

        public ActionResult ListAds(                       
            int page = 1,
            string category = null,
            string locations = null,
            string search = null
            )
        {
            using (var database = new AdsListingDbContext())
            {
                var LocationsList = database
                .Locations
                .OrderBy(l => l.Name)
                .ToList();

                var locationNames = new Dictionary<string, int>();

                foreach (var loc in LocationsList)
                {
                    locationNames[loc.Name] = loc.Id;
                }

                ViewBag.Locations = new SelectList(locationNames.Keys);

                var CategoriesList = database
                .Categories
                .OrderBy(l => l.Name)
                .ToList();

                var categoryNames = new Dictionary<string, int>();

                foreach (var cat in CategoriesList)
                {
                    categoryNames[cat.Name] = cat.Id;
                }

                ViewBag.Category = new SelectList(categoryNames.Keys);

                var pageSize = 5;

                var adsQuery = database.Ads.AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    adsQuery = adsQuery
                        .Where(a => a.Title.ToLower().Contains(search.ToLower()));
                }

                if (!string.IsNullOrEmpty(locations))
                {
                    adsQuery = adsQuery
                        .Where(a => a.Location.Name.Equals(locations));
                }

                if (!string.IsNullOrEmpty(category))
                {
                    adsQuery = adsQuery
                        .Where(a => a.Category.Name.Equals(category));
                }

                ViewBag.CurrentPage = page;

                var ads = adsQuery
                    .OrderByDescending(a => a.DateCreated)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Include(a => a.Location)
                    .Include(a => a.Category)
                    .Include(a => a.Author)
                    .ToList();

                return View(ads);
            }
        }
    }
}