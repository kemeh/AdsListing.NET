﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Security;

namespace AdsListing.Models
{
    public class Role
    {
        public string Name { get; set; }

        public bool IsSelected { get; set; }
    }
}