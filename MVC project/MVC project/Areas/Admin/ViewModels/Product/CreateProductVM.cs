using Microsoft.AspNetCore.Mvc.Routing;
using MVC_Project.Models;
using System.ComponentModel.DataAnnotations;
using System.Windows.Markup;

namespace MVC_Project.Areas.Admin.ViewModels
{
    public class CreateProductVM
    {
        [Required(ErrorMessage = "Add daxil edilmelidir")]
        [MaxLength(25, ErrorMessage = "25 den uzun simvol olmaz")]
        public string Name { get; set; }
        [Required]
        
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        [Required]
        public int? CategoryId { get; set; }
    }
}
