using System;
using System.Collections.Generic;
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

        public static AdsListingDbContext Create()
        {
            return new AdsListingDbContext();
        }
    }
}