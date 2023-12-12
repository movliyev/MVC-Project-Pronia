using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Project.DAL;
using MVC_Project.Models;
using MVC_Project.Utilities.Exceptions;
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
        public async Task <IActionResult> Detail(int id)
        {
            if (id == 0) throw new WrongRequestException("Sorgu yanlisdir");

            List<Product> products = _context.Products.Include(p => p.ProductImages).ToList();
            Product product= await _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductImages)
                .Include(p=>p.ProductTags).ThenInclude(x=>x.Tag)
                .Include(p => p.ProductColors).ThenInclude(x => x.Color)
                .Include(p => p.ProductSizes).ThenInclude(x => x.Size)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) throw new NotFoundException("Mehsul tapilmadi");
            List<Product> releatedProducts = _context.Products.Include(p => p.ProductImages)
                .Include(p => p.Category)
                .Where(p => p.CategoryId == product.CategoryId && p.Id != id).ToList();

            List<ProductImage> pi = await _context.ProductImages.ToListAsync();
            List<Color> colors = await _context.Colors.ToListAsync();
            List<Size> sizes = await _context.Sizes.ToListAsync();

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
