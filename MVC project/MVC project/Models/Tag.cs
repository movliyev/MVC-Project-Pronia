using System.ComponentModel.DataAnnotations;

namespace MVC_Project.Models
{
    public class Tag
    {
        internal object Products;

        public  int Id { get; set; }
        [Required(ErrorMessage = "Add daxil edilmelidir")]
        [MaxLength(25, ErrorMessage = "25 den uzun simvol olmaz")]
        public string Name { get; set; }
        public List<ProductTag>? ProductTags { get; set; }

    }
}
