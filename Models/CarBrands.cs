﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TemplateProject.Models
{
    public class CarBrands
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Marka Adı")]
        public string Name { get; set; }
    }
}
