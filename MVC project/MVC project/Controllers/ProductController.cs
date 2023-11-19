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
            Product product=_context.Products.Include(p=>p.Category).Include(p=>p.ProductImages).FirstOrDefault(x => x.Id == id);
            if (product == null) return NotFound();
            List<Product> releatedProducts = _context.Products.Include(p => p.ProductImages).Include(p => p.Category).Where(p => p.CategoryId == product.CategoryId && p.Id != id).ToList();

            List<ProductImage> pi = _context.ProductImages.ToList();
            ProductVM productVM = new ProductVM
            {
                ProductImages = pi,
                Products =products,
                Product=product,
                ReleatedProducts = releatedProducts
        };

            return View(productVM);
        }

    }
}
