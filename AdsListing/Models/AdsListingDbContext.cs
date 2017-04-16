using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AdsListing.Models
{
    public class AdsListingDbContext : IdentityDbContext<ApplicationUser>
    {
        public AdsListingDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public virtual IDbSet<Ad> Ads { get; set; }

        public static AdsListingDbContext Create()
        {
            return new AdsListingDbContext();
        }
    }
}