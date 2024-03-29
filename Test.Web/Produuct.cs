﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Test.Web
{
    public partial class Produuct
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal? Price { get; set; }
        [Required]
        public int? Stock { get; set; }
        [Required]
        public string Color { get; set; }
    }
}
