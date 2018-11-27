﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Models
{
    public class PagingInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPage => (int) Math.Ceiling(Convert.ToDecimal(TotalItems) / ItemsPerPage);

        public string UrlParam { get; set; }
    }
}