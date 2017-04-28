using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AdsListing.Models;

namespace AdsListing.Controllers.Admin
{
    public class LocationController : Controller
    {
        // GET: Location
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: Location/List
        public ActionResult List()
        {
            using (var database = new AdsListingDbContext())
            {
                var locations = database
                    .Locations
                    .ToList();

                return View(locations);
            }
        }

        // GET: Location/Create
        public ActionResult Create()
        {
            return View();
        }

        //POST: Location/Create
        [HttpPost]
        public ActionResult Create(Location location)
        {
            if (ModelState.IsValid)
            {
                using (var database = new AdsListingDbContext())
                {
                    database.Locations.Add(location);
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(location);
        }

        //GET: Location/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new AdsListingDbContext())
            {
                var location = database
                    .Locations
                    .FirstOrDefault(l => l.Id == id);

                if (location == null)
                {
                    return HttpNotFound();
                }

                return View(location);
            }
        }

        // POST: Location/Edit
        [HttpPost]
        public ActionResult Edit(Location location)
        {
            if (ModelState.IsValid)
            {
                using (var database = new AdsListingDbContext())
                {
                    database.Entry(location).State = EntityState.Modified;
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(location);
        }

        //GET: Location/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new AdsListingDbContext())
            {
                var location = database
                    .Locations
                    .FirstOrDefault(c => c.Id == id);

                if (location == null)
                {
                    return HttpNotFound();
                }

                return View(location);
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            using (var database = new AdsListingDbContext())
            {
                var location = database
                    .Locations
                    .FirstOrDefault(c => c.Id == id);

                var locationAds = location
                    .Ads
                    .ToList();

                foreach (var ad in locationAds)
                {
                    database.Ads.Remove(ad);
                }

                database.Locations.Remove(location);
                database.SaveChanges();

                return RedirectToAction("Index");
            }
        }
    }
}