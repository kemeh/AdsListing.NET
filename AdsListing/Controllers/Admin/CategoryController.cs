﻿using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AdsListing.Models;

namespace AdsListing.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: Category/List
        public ActionResult List()
        {
            using (var database = new AdsListingDbContext())
            {
                var categories = database.Categories.ToList();

                return View(categories);
            }
        }

        // GET: Category/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                using (var database = new AdsListingDbContext())
                {
                    database.Categories.Add(category);
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(category);
        }

        //GET Category/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new AdsListingDbContext())
            {
                var category = database
                    .Categories
                    .FirstOrDefault(c => c.Id == id);

                if (category == null)
                {
                    return HttpNotFound();
                }

                return View(category);
            }
        }

        // POST: Category/Edit
        [HttpPost]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                using (var database = new AdsListingDbContext())
                {
                    database.Entry(category).State = EntityState.Modified;
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(category);
        }

        //GET: Category/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new AdsListingDbContext())
            {
                var category = database
                    .Categories
                    .FirstOrDefault(c => c.Id == id);

                if (category == null)
                {
                    return HttpNotFound();
                }

                return View(category);
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            using (var database = new AdsListingDbContext())
            {
                var category = database
                    .Categories
                    .FirstOrDefault(c => c.Id == id);

                var categoryAds = category
                    .Ads
                    .ToList();

                foreach (var ad in categoryAds)
                {
                    database.Ads.Remove(ad);
                }

                database.Categories.Remove(category);
                database.SaveChanges();

                return RedirectToAction("Index");
            }
        }
    }
}