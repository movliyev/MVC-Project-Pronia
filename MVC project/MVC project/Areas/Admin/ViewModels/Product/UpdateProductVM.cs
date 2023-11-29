

using MVC_Project.Models;

namespace MVC_Project.Areas.Admin.ViewModels
{
    public class UpdateProductVM
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        public int CategoryId { get; set; }
        public List<Category>? Categorys { get; set; }
        public List<int> Tagids  { get; set; }
        public List<int> Colorids { get; set; }
        public List<int> Sizeids { get; set; }

        public List<int> Imageids { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
        public IFormFile? MainPhoto { get; set; }
        public IFormFile? HoverPhoto { get; set; }
        public List<IFormFile>? Photos { get; set; }

        public List<Tag>? Tags { get; set; }
        public List<Size>? Sizes { get; set; }
        public List<Color>? Colors { get; set; }




    }
}
