using MVC_Project.Models;

namespace MVC_Project.ViewModels
{
    public class ProductVM
    {
        public List <Product> Products { get; set; }
        public List<Product> ReleatedProducts { get; set; }

        public List<ProductImage> ProductImages { get; set; }
        public Category Category { get; set; }
        public Product Product { get; set; }
        public List<Color> Colors { get; set;}
        public List<Size> Sizes { get; set; }



    }
}
