using Microsoft.EntityFrameworkCore;
using MVC_Project.DAL;
using MVC_Project.Models;
using MVC_Project.ViewModels;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;

namespace MVC_Project.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _http;

        public LayoutService(AppDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _http = http;
        }
        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            Dictionary<string,string> settings = await _context.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);
            return settings;
        }
        public async Task<List<BasketItemVM>> GetBasketAsync()
        {
            List<BasketItemVM> items = new List<BasketItemVM>();
            if (_http.HttpContext.Request.Cookies["Basket"] is not null)
            {
                List<BasketCookieItemVM> cookies = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(_http.HttpContext.Request.Cookies["Basket"]);

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

            return items;
        }
    }
}

