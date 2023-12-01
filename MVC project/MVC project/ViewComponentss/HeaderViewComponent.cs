using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Project.DAL;

namespace MVC_Project.ViewComponentss
{
    public class HeaderViewComponent:ViewComponent
    {
        private readonly AppDbContext _context;

        public HeaderViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string,string> headers=await _context.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);
            return View(headers);
        }
    }
}
