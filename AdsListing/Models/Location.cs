using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AdsListing.Models
{
    public class Location
    {
        private ICollection<Ad> ads;

        public Location()
        {
            this.ads = new List<Ad>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [StringLength(25)]
        public string Name { get; set; }

        public virtual ICollection<Ad> Ads { get; set; }
    }
}