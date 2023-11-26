using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MVC_Project.Areas.Admin.ViewModels;
using MVC_Project.Areas.Admin.ViewModels;
using MVC_Project.DAL;
using MVC_Project.Models;

namespace MVC_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
       
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public async Task <IActionResult> Index()
        {
            List<Product> products = await _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary==true))
                .ToListAsync(); 

            return View(products);
        }

        public async Task<IActionResult> Create()
        {

          ViewBag.Categorys = await _context.Categorys.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult>Create(CreateProductVM vm)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.Categorys = await _context.Categorys.ToListAsync();
                return View();
            }
            bool result = await _context.Categorys.AnyAsync(c => c.Id == vm.CategoryId);

            if(!result)
            {
                ViewBag.Categorys = await _context.Categorys.ToListAsync();
                ModelState.AddModelError("CategoryId", "Bu id li category movcud deyil");
                return View();
            }
            //int result=await _context.Products.Where(p=>p.Price>=0).CountAsync();
            //if(!result)
            //{
            //    ModelState.AddModelError("Price","yalnis deyer");
            //}
            Product product = new Product
            {
                SKU = vm.SKU,
                Description = vm.Description,   
                Name = vm.Name, 
                Price = vm.Price,
                CategoryId =(int) vm.CategoryId,


            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return View(nameof(Index));  
        }

        public async Task<IActionResult> Detail(int id)
        {

            Product product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductTags).ThenInclude(x => x.Tag)
                .Include(p => p.ProductColors).ThenInclude(x => x.Color)
                .Include(p => p.ProductSizes).ThenInclude(x => x.Size)
                .FirstOrDefaultAsync(x => x.Id == id);
           
            if (id == 0) return BadRequest();


            if (product == null) return NotFound();
            ViewBag.ProductTags = await _context.ProductTags.ToListAsync();
            ViewBag.ProductColors = await _context.ProductTags.ToListAsync();
            ViewBag.ProductSizes = await _context.ProductTags.ToListAsync();



            return View(product);
        }


    }
}
