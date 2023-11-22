﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Project.DAL;
using MVC_Project.Models;
using MVC_Project.ViewModels;
using System.Linq;

namespace MVC_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async  Task <IActionResult> Index()
        {
            //_context.Slides.AddRange(slides);  
            //_context.SaveChanges(); 
            List<Slide> slides= await _context.Slides.OrderBy(s => s.Order).Take(3).ToListAsync();
            List<Product> products = _context.Products.Include(p=>p.ProductImages.Where(p=>p.IsPrimary!=null) ).ToList();
          

            HomeVM home = new HomeVM
            {
                Slides = slides,
                Products = products,
                LatestsSliders= _context.Slides.OrderByDescending(x => x.Order).Take(4).ToList()

            };

            return View(home);
        }
        public IActionResult About()
        {
            return View();
        }
    }
}
