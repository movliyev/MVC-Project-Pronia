using System.ComponentModel.DataAnnotations;

namespace MVC_Project.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Add daxil edilmelidir")]
        [MaxLength(25,ErrorMessage ="25 den uzun simvol olmaz")]
        public string Name { get; set; }
        public List<Product>? Products  { get; set; }

    }
}
