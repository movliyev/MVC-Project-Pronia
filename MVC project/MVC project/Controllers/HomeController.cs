using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            //_context.Slides.AddRange(slides);  
            //_context.SaveChanges(); 
            List<Slide> slides= _context.Slides.OrderBy(s=>s.Order).Take(2).ToList();
            List<Product> products = _context.Products.ToList();
            //List<Product> latest = _context.Products.OrderByDescending(x => x.Order).Take(8).ToList();

            HomeVM home = new HomeVM
            {
                Slides = slides,
                Products = products,
                LatestsSliders= _context.Products.OrderByDescending(x => x.Order).Take(8).ToList()
        };

            return View(home);
        }
        public IActionResult About()
        {
            return View();
        }
    }
}
