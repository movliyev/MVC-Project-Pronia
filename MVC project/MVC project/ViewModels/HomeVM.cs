using MVC_Project.Models;

namespace MVC_Project.ViewModels
{
    public class HomeVM
    {
        public List<Slide> Slides { get; set; }
        public List<Product> Products { get; set; }
        public List<Slide> LatestsSliders { get; set; }
         public List<ProductImage> ProductImages { get; set; }
        
       

    }
}
