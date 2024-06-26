﻿using System.ComponentModel.DataAnnotations;

namespace MVC_Project.Areas.Admin.ViewModels
{
    public class CreateColorVM
    {
        [Required(ErrorMessage = "Add daxil edilmelidir")]
        [MaxLength(25, ErrorMessage = "25 den uzun simvol olmaz")]
        public string Name { get; set; }
    }
}
