﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AdsListing.Models
{
    public class AdViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(50)]
        public string Title { get; set; }

        public string Description { get; set; }

        public string AuthorId { get; set; }

        public int CategoryId { get; set; }

        public ICollection<Category> Categories { get; set; }

        public int LocationId { get; set; }

        public ICollection<Location> Locations { get; set; }

        public double Price { get; set; }

        public List<Photo> Photos { get; set; }

        public AdStatus Status { get; set; }

        public int ContactNumber { get; set; }
    }
}