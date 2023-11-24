using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Project.DAL;
using MVC_Project.Models;
using NuGet.ProjectModel;

namespace MVC_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ColorController : Controller
    {
        private readonly AppDbContext _context;

        public ColorController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Color> colors = await _context.Colors.Include(c => c.ProductColors).ToListAsync();
            return View(colors);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Color color)
        {
            if (!ModelState.IsValid)
            {
                return View();

            }
            bool result = _context.Colors.Any(c => c.Name.Trim() == color.Name.Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda color movcuddur");
                return View();
            }
            await _context.Colors.AddAsync(color);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //UPDATE 
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Color color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
            if (color == null) return NotFound();
            return View(color);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, Color color)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Color exsisted = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
            if (exsisted == null) return NotFound();
            bool result = await _context.Colors.AnyAsync(c => c.Name == color.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda color atriq movcuddur");

                return View();
            }
            exsisted.Name = color.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //DELETE

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Color existed = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
            
            if (existed is null) return NotFound();
            _context.Colors.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id < 0) return BadRequest();

            Color color = await _context.Colors.FirstOrDefaultAsync(x => x.Id == id);
                      
               


            if (color == null) return NotFound();


            return View(color);
        }
    }
}
