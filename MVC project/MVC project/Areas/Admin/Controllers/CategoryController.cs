using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Project.DAL;
using MVC_Project.Models;

namespace MVC_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task <IActionResult> Index()
        {
           List<Category> categories =await _context.Categorys.Include(c=>c.Products).ToListAsync();          
                return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
           if (!ModelState.IsValid )
           {
                return View();

           }
           bool result= _context.Categorys.Any(c=>c.Name.Trim() == category.Name.Trim());  
            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda category movcuddur");
                return View();
            }
            await _context.Categorys.AddAsync(category);    
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));   
        }
    }
}
