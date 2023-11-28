using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Project.Areas.Admin.ViewModels;
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
        public async Task<IActionResult> Create(CreateColorVM cvm)
        {
            if (!ModelState.IsValid)
            {
                return View();

            }
            bool result = _context.Colors.Any(c => c.Name.Trim() == cvm.Name.Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda color movcuddur");
                return View();
            }
            Color color = new Color
            {
                Name= cvm.Name, 
            };

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
            UpdateColorVM vm = new UpdateColorVM
            {
                Name = color.Name,  
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateColorVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Color exsisted = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
            if (exsisted == null) return NotFound();
            bool result = await _context.Colors.AnyAsync(c => c.Name == vm.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda color atriq movcuddur");

                return View();
            }
            exsisted.Name = vm.Name;
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
