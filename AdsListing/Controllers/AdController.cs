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
    }
}