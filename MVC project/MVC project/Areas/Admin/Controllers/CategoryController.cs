﻿using Microsoft.AspNetCore.Mvc;
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


        //UPDATE 
        public async Task<IActionResult>Update(int id)
        {
            if (id<= 0) return BadRequest();
            Category category=await _context.Categorys.FirstOrDefaultAsync(c=>c.Id==id);
            if (category == null) return NotFound();
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult>Update(int id,Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Category exsisted = await _context.Categorys.FirstOrDefaultAsync(c => c.Id == id);
            if(exsisted == null) return NotFound(); 
            bool result=await _context.Categorys.AnyAsync(c=>c.Name==category.Name&&c.Id!=id);
            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda category atriq movcuddur");

                return View();
            }
            exsisted.Name = category.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));  
        }


        //DELETE

        public async Task<IActionResult> Delete(int id)
        { 
            if(id<=0) return BadRequest(); 

            Category existed= await _context.Categorys.FirstOrDefaultAsync(c=>c.Id==id);

            if (existed is null) return NotFound();
            _context.Categorys.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); 
        }

        //detail
        public async Task<IActionResult> Detail(int id)
        {
            if (id < 0) return BadRequest();

            Category category = await _context.Categorys
                .Include(p => p.Products)
                .ThenInclude(p=>p.ProductImages)
                .Include(x=>x.Products).ThenInclude(x=>x.ProductTags)
                .ThenInclude(x=>x.Tag)
                .Include(p=>p.Products)
                .ThenInclude(p=>p.ProductSizes).ThenInclude(pi=>pi.Size)
                 .Include(p => p.Products)
                .ThenInclude(p => p.ProductColors).ThenInclude(pi => pi.Color)
                .FirstOrDefaultAsync(x => x.Id == id);


            if (category == null) return NotFound();


            return View(category);
        }


    }
}
