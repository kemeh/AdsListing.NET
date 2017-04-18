using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AdsListing.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace AdsListing.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        // Get: User
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: User List
        public ActionResult List()
        {
            using (var database = new AdsListingDbContext())
            {
                var users = database.Users.ToList();

                var adminUsers = GetAdminUserNames(users, database);
                ViewBag.Admins = adminUsers;

                return View(users);
            }           
        }

        //GET: User/Edit
        public ActionResult Edit(string id)
        {
            //Validate ID
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new AdsListingDbContext())
            {
                // Get the user from the DB
                var user = database
                    .Users
                    .Where(u => u.Id == id)
                    .First();

                // Check if the user is valid
                if (user == null)
                {
                    return HttpNotFound();
                }

                //Create a view model
                var viewModel = new EditUserViewModel();
                viewModel.User = user;
                viewModel.Roles = GetUserRoles(user, database);

                return View(viewModel);
            }
        }

        //POST: User/Edit
        [HttpPost]
        public ActionResult Edit(string id, EditUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var database = new AdsListingDbContext())
                {
                    // Get user from database
                    var user = database.Users.FirstOrDefault(u => u.Id == id);

                    if (user == null)
                    {
                        return HttpNotFound();
                    }

                    // If password field is not empty, change password
                    if (!string.IsNullOrEmpty(viewModel.Password))
                    {
                        var hasher = new PasswordHasher();
                        var passwordHash = hasher.HashPassword(viewModel.Password);
                        user.PasswordHash = passwordHash;
                    }

                    //Set user properties
                    user.Email = viewModel.User.Email;
                    user.FullName = viewModel.User.FullName;
                    user.UserName = viewModel.User.Email;
                    this.SetUserRoles(viewModel, user, database);

                    database.Entry(user).State = EntityState.Modified;
                    database.SaveChanges();

                    return RedirectToAction("List");
                }               
            }
            return View(viewModel);
        }

        //GET: User/Delete
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new AdsListingDbContext())
            {
                //Get User from the DB
                var user = database
                    .Users
                    .Where(u => u.Id.Equals(id))
                    .First();

                // Check if user exists
                if (user == null)
                {
                    return HttpNotFound();
                }

                return View(user);
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new AdsListingDbContext())
            {
                //Get User from the DB
                var user = database
                    .Users
                    .Where(u => u.Id.Equals(id))
                    .First();

                //Get User Ads from the database
                var userAds = database
                    .Ads
                    .Where(a => a.Author.Id == user.Id);

                //Delete user Ads
                foreach (var ad in userAds)
                {
                    database.Ads.Remove(ad);
                }

                //Delete the user and update the DB
                database.Users.Remove(user);
                database.SaveChanges();
                    

                return RedirectToAction("List");
            }
        }


        private void SetUserRoles(EditUserViewModel model, ApplicationUser user, AdsListingDbContext db)
        {
            var userManager = Request
                .GetOwinContext()
                .GetUserManager<ApplicationUserManager>();

            foreach (var role in model.Roles)
            {
                if (role.IsSelected)
                {
                    userManager.AddToRole(user.Id, role.Name);
                }
                else if (!role.IsSelected)
                {
                    userManager.RemoveFromRole(user.Id, role.Name);
                }
            }
        }

        private IList<Role> GetUserRoles(ApplicationUser user, AdsListingDbContext db)
        {
            //Create user manager
            var userManager = Request
                .GetOwinContext()
                .GetUserManager<ApplicationUserManager>();

            //Get all application roles
            var roles = db.Roles
                .Select(r => r.Name)
                .OrderBy(r => r)
                .ToList();

            // Check if the user has the Role
            var userRoles = new List<Role>();

            foreach (var roleName in roles)
            {
                var role = new Role {Name = roleName};

                if (userManager.IsInRole(user.Id, roleName))
                {
                    role.IsSelected = true;
                }

                userRoles.Add(role);
            }

            return userRoles;
        }

        private HashSet<string> GetAdminUserNames(List<ApplicationUser> users, AdsListingDbContext context)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var admins = new HashSet<string>();

            foreach (var user in users)
            {
                if (userManager.IsInRole(user.Id, "Admin"))
                {
                    admins.Add(user.UserName);
                }
            }

            return admins;
        }
    }
}