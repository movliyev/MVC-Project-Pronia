using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MVC_Project.DAL;
using MVC_Project.Models;
using System.Drawing.Text;

namespace MVC_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController : Controller
    {
       
        private readonly AppDbContext _context;
        public SlideController(AppDbContext context)
        {
            _context = context;
        }
       
        public async Task<IActionResult >Index()
        {
            
            List<Slide> slides=await  _context.Slides.ToListAsync();    
            return View(slides);
        }
        public IActionResult Create()
        {
            return View();  
        }
        [HttpPost]
        public async Task<IActionResult>Create(Slide slide)
        {

            if (slide.Photo is null)
            {
                ModelState.AddModelError("Photo", "Shekil mutleq secilmelidir");
                return View();
            }

            if(!slide.Photo.ContentType.Contains("image/")) 
            {
                ModelState.AddModelError("Photo", "File tipi uyqun deyil");
                return View();
            }
            if (slide.Photo.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("Photo","File olcusu 2-mb den boyuk olmamalidir");
                return View();

            }

            FileStream fileStream = new FileStream(@"C:\Users\helpdesk_connect\Desktop\MVC project Pronia\MVC project\MVC Project\wwwroot\assets\images\slider\" + slide.Photo.FileName,FileMode.Create);
           await slide.Photo.CopyToAsync(fileStream);
            slide.Image = slide.Photo.FileName;

            //return Content(slide.Photo.FileName + " " + slide.Photo.ContentType + " " + slide.Photo.Length);
            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Detail()
        {
            List<Slide> slides = await _context.Slides.ToListAsync();
            return View(slides);
        }
    }
}
