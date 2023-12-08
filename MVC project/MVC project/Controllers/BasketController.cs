using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Project.DAL;
using MVC_Project.Models;
using MVC_Project.Utilities.Extensions;
using MVC_Project.ViewModels;
using Newtonsoft.Json;
using NuGet.ContentModel;
using System.Security.Claims;

namespace MVC_Project.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BasketController(AppDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> items = new List<BasketItemVM>();
            if(User.Identity.IsAuthenticated)
            {
                AppUser user=await _userManager.Users
                    .Include(u=>u.BasketItems)
                    .ThenInclude(b=>b.Product)
                    .ThenInclude(b=>b.ProductImages.Where(p=>p.IsPrimary==true))
                    .FirstOrDefaultAsync(u=>u.Id==User.FindFirstValue(ClaimTypes.NameIdentifier));
                foreach (BasketItem item in user.BasketItems)
                {
                    items.Add(new BasketItemVM
                    {
                        Id= item.Id,
                        Price=item.Product.Price,
                        Count=item.Count,
                        Name=item.Product.Name,
                        SubTotal=item.Count*item.Product.Price,
                        Image=item.Product.ProductImages.FirstOrDefault()?.Url

                    });
                }

            }
            else
            {
                if (Request.Cookies["Basket"] is not null)
                {
                    List<BasketCookieItemVM> cookies = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);

                    foreach (var cookie in cookies)
                    {
                        Product product = await _context.Products
                            .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                            .FirstOrDefaultAsync(p => p.Id == cookie.Id);

                        if (product is not null)
                        {
                            BasketItemVM item = new BasketItemVM
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Image = product.ProductImages.FirstOrDefault().Url,
                                Price = product.Price,
                                Count = cookie.Count,
                                SubTotal = product.Price * cookie.Count
                            };
                            items.Add(item);
                        }
                    }
                }
            }
          
            return View(items);
        }

        public async Task<IActionResult> AddBasket(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();



            if(User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.Users
                    .Include(u=>u.BasketItems)
                    .FirstOrDefaultAsync(u=>u.Id==User.FindFirstValue(ClaimTypes.NameIdentifier));
                    
                if(user==null) return NotFound();   
                BasketItem item=user.BasketItems.FirstOrDefault(b=>b.ProductId==product.Id);
                if(item is null)
                {
                    item = new BasketItem
                    {
                        AppUserId = user.Id,
                        ProductId = product.Id,
                        Count = 1,
                        Price=product.Price,

                    };
                    user.BasketItems.Add(item);
                }
                else
                {
                    item.Count++;
                }
                await _context.SaveChangesAsync();
               
            }

            else
            {
                List<BasketCookieItemVM> basket;

                if (Request.Cookies["Basket"] is null)
                {
                    basket = new List<BasketCookieItemVM>();
                    BasketCookieItemVM item = new BasketCookieItemVM
                    {
                        Id = id,
                        Count = 1
                    };
                    basket.Add(item);
                }
                else
                {
                    basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);

                    BasketCookieItemVM existed = basket.FirstOrDefault(b => b.Id == id);

                    if (existed is null)
                    {
                        BasketCookieItemVM item = new BasketCookieItemVM
                        {
                            Id = id,
                            Count = 1
                        };
                        basket.Add(item);
                    }
                    else
                    {
                        existed.Count++;
                    }
                }



                string json = JsonConvert.SerializeObject(basket);

                Response.Cookies.Append("Basket", json);

            }


           
            return RedirectToAction(nameof(Index), "Home");
        }


        public async Task<IActionResult>RemoveBasket(int id)
        {
                if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();


            if (User.Identity.IsAuthenticated)
            {

                AppUser user = await _userManager.Users
                   .Include(u => u.BasketItems)
                   .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (user == null) return NotFound();

                BasketItem item = user.BasketItems.FirstOrDefault(b => b.ProductId == product.Id);
                if (item is not null)
                {
                   
                    user.BasketItems.Remove(item);
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                string json = Request.Cookies["Basket"];
                if (json != null)
                {
                    List<BasketCookieItemVM> basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(json);

                    BasketCookieItemVM exist = basket.FirstOrDefault(b => b.Id == id);

                    if (exist != null)
                    {
                        basket.Remove(exist);
                    }

                    Response.Cookies.Append("Basket", JsonConvert.SerializeObject(basket));
                }
            }
                
            return RedirectToAction(nameof(Index),"Basket");
        }


        public IActionResult GetBasket()
        {
            return Content(Request.Cookies["Basket"]);
        }
    }
}

