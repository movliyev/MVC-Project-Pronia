using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Project.DAL;
using MVC_Project.Models;
using MVC_Project.ViewModels;

namespace MVC_Project.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
        public IActionResult Detail(int id)
        {
            if (id == 0) return BadRequest();

            List<Product> products = _context.Products.Include(p => p.ProductImages).ToList();
            Product product=_context.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductImages)
                .Include(p=>p.ProductTags).ThenInclude(x=>x.Tag)
                .Include(p => p.ProductColors).ThenInclude(x => x.Color)
                .Include(p => p.ProductSizes).ThenInclude(x => x.Size)
                .FirstOrDefault(x => x.Id == id);
            if (product == null) return NotFound();
            List<Product> releatedProducts = _context.Products.Include(p => p.ProductImages).Include(p => p.Category).Where(p => p.CategoryId == product.CategoryId && p.Id != id).ToList();

            List<ProductImage> pi = _context.ProductImages.ToList();
            List<Color> colors = _context.Colors.ToList();
            List<Size> sizes = _context.Sizes.ToList();

            ProductVM productVM = new ProductVM
            {
                Sizes = sizes,  
                Colors = colors,
                ProductImages = pi,
                Products =products,
                Product=product,
                ReleatedProducts = releatedProducts

            };

            return View(productVM);
        }

    }
}
