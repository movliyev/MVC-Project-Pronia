using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Project.Areas.Admin.ViewModels;
using MVC_Project.DAL;
using MVC_Project.Models;
using MVC_Project.ViewModels;

namespace MVC_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }
        

        public async Task<IActionResult> Index(int page = 1)
        {
            if (page < 1) return BadRequest();

            int count = await _context.Tags.CountAsync();
            List<Tag> tags = await _context.Tags.Skip((page - 1) * 3).Take(3)
                .Include(t => t.ProductTags).ToListAsync();
            PaginateVM<Tag> pagvm = new PaginateVM<Tag>
            {
                Items = tags,
                TotalPage = Math.Ceiling((double)count / 3),
                CurrentPage = page,
            };
            return View(pagvm);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateTagVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View();

            }
            bool result = _context.Tags.Any(c => c.Name.Trim() == vm.Name.Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda tag movcuddur");
                return View();
            }

            Tag tag = new Tag
            {
                Name= vm.Name 
            };


            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //UPDATE 
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Tag tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (tag == null) return NotFound();
            UpdateTagVM vm = new UpdateTagVM
            {
                Name = tag.Name,
            };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateTagVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Tag exsisted = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);
            if (exsisted == null) return NotFound();
            bool result = await _context.Tags.AnyAsync(c => c.Name == vm.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda tag atriq movcuddur");

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

            Tag existed = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();
            _context.Tags.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

   
