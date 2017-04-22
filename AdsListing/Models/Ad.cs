using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AdsListing.Models
{
    public class Ad
    {
        private ICollection<Photo> photos;

        public Ad()
        {
            
        }

        public Ad(string authorId, string title, string description, int categoryId, int locationId, double price)
        {
            this.AuthorId = authorId;
            this.Title = title;
            this.Description = description;
            this.CategoryId = categoryId;
            this.LocationId = locationId;
            this.Price = price;
            this.photos = new List<Photo>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        public string Description { get; set; }

        [ForeignKey("Author")]
        public string AuthorId { get; set; }

        public virtual ApplicationUser Author { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        [ForeignKey("Location")]
        public int LocationId { get; set; }

        public virtual  Location Location { get; set; }

        [Required]
        public double Price { get; set; }

        public virtual ICollection<Photo> Photos { get; set; }

        public bool IsAuthor(string name)
        {
            return this.Author.UserName.Equals(name);
        }
    }
}