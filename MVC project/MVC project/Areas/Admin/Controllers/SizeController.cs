using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Project.Areas.Admin.ViewModels;
using MVC_Project.DAL;
using MVC_Project.Models;

namespace MVC_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SizeController : Controller
    {
        private readonly AppDbContext _context;

        public SizeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Size> sizes = await _context.Sizes.Include(c => c.ProductSizes).ToListAsync();
            return View(sizes);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSizeVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View();

            }
            bool result = _context.Sizes.Any(c => c.Name.Trim() == vm.Name.Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda size movcuddur");
                return View();
            }
            Size size = new Size
            {
                Name = vm.Name,   
            };
            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //UPDATE 
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Size size = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
            if (size == null) return NotFound();
            UpdateSizeVM vm = new UpdateSizeVM
            {
                Name = size.Name,
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSizeVM svm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Size exsisted = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
            if (exsisted == null) return NotFound();
            bool result = await _context.Sizes.AnyAsync(s => s.Name == svm.Name && s.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda size atriq movcuddur");

                return View();
            }
            exsisted.Name = svm.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //DELETE

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Size existed = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
            
            if (existed is null) return NotFound();
            _context.Sizes.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
