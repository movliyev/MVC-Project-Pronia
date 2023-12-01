using Microsoft.EntityFrameworkCore;
using MVC_Project.DAL;

namespace MVC_Project.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;

        public LayoutService( AppDbContext context)
        {
            _context = context;
        }
        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            Dictionary<string,string> settings = await _context.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);
            return settings;
        }
    }
}

