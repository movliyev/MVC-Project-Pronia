using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Project.DAL;
using MVC_Project.Models;
using MVC_Project.ViewModels;

namespace MVC_Project.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ShopController(AppDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task <IActionResult> Index(string? search,int? order,int? categoryId)
        {
            IQueryable<Product> query = _context.Products.Include(p => p.ProductImages).AsQueryable();
            switch (order)
            {
                    case 1:
                    query=query.OrderBy(p=>p.Name);
                    break;  
                    case 2: 
                    query=query.OrderBy(p=>p.Price);
                    break; 
                    case 3:
                    query=query.OrderByDescending(p=>p.Id); 
                    break;
              
            }
            if (!String.IsNullOrWhiteSpace(search))
            {
                query=query.Where(p=>p.Name.ToLower().Contains(search.ToLower()));  
            }
            if(categoryId!= null)
            {
                query=query.Where(p=>p.CategoryId==categoryId); 
            }
            ShopVM shopVM = new ShopVM
            {
                Categorys = await _context.Categorys.Include(c=>c.Products).ToListAsync(),
                Products =await query.ToListAsync(),
                CategoryId = categoryId,
                Order = order,
                Search = search,

            };
            return View(shopVM);
        }
    }
}
