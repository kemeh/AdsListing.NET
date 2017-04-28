using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public Ad(string authorId, string title, string description, int categoryId, int locationId, double price, int contactNumber)
        {
            this.AuthorId = authorId;
            this.Title = title;
            this.Description = description;
            this.CategoryId = categoryId;
            this.LocationId = locationId;
            this.Price = price;
            this.ContactNumber = contactNumber;
            this.photos = new List<Photo>();
            this.Status = AdStatus.WaitingApproval;
            this.DateCreated = DateTime.UtcNow;
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
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

        public virtual Location Location { get; set; }

        [Required(ErrorMessage = "Price must contain only digits!")]
        public double Price { get; set; }

        public virtual ICollection<Photo> Photos { get; set; }

        public AdStatus Status { get; set; }

        public DateTime DateCreated { get; set; }

        [Required(ErrorMessage = "Contact Number must contain only digits!")]
        [DisplayName("Contact Number")]
        public int ContactNumber { get; set; }

        public bool IsAuthor(string name)
        {
            return this.Author.UserName.Equals(name);
        }
    }
}