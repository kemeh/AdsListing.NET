using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace AdsListing.Models
{
    public class Photo
    {
        [Key]
        public int Id { get; set; }

        public string ImagePath { get; set; }

        public string ThumbPath { get; set; }

        [ForeignKey("Ad")]        
        public int AdId { get; set; }

        public virtual Ad Ad { get; set; }

        public bool IsSelected { get; set; }
    }
}