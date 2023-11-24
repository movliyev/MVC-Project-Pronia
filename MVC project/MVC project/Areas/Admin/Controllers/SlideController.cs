using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MVC_Project.DAL;
using MVC_Project.Models;
using MVC_Project.Utilities.Extensions;
using MVC_Project.ViewModels;
using System.Drawing.Text;

namespace MVC_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController : Controller
    {
       
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SlideController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env=env;   
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

            if (!slide.Photo.ValidateType("image/"))
            {
                ModelState.AddModelError("Photo", "File tipi uyqun deyil");
                return View();
            }

            if (!slide.Photo.ValidateSize(2*1024))
            {
                ModelState.AddModelError("Photo", "File olcusu 2-mb den boyuk olmamalidir");
                return View();

            }

            slide.Image = await slide.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "slider");

          
            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult>Update(int id)
        {
            if (id <= 0) return BadRequest();
            Slide exsist = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (exsist == null) return NotFound();
            return View(exsist);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, Slide slide)
        {
            Slide exsist = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if(exsist == null) return NotFound();
            if (!ModelState.IsValid)
            {
                return View(exsist);
            }
            if(slide.Photo is not null)
            {
                if (!slide.Photo.ValidateType("image/"))
                {
                    ModelState.AddModelError("Photo", "File tipi uyqun deyil");
                    return View(exsist);
                }

                if (!slide.Photo.ValidateSize(2 * 1024))
                {
                    ModelState.AddModelError("Photo", "File olcusu 2-mb den boyuk olmamalidir");
                    return View(exsist);

                }
                string filename = await slide.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "slider");
                exsist.Image.DeleteFile(_env.WebRootPath, "assets", "images", "slider");
                exsist.Image = filename;
            }
            exsist.Title= slide.Title;  
            exsist.Subtitle= slide.Subtitle;    
            exsist.Order= slide.Order;
            exsist.Description= slide.Description;  
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }




        public async Task<IActionResult>Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Slide exsist = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (exsist == null) return NotFound();
            exsist.Image.DeleteFile(_env.WebRootPath, "assets", "image", "slider");
            _context.Slides.Remove(exsist); 
            await _context.SaveChangesAsync();
           

            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Detail(int id)
        {

           Slide slides = await _context.Slides.FirstOrDefaultAsync(x => x.Id == id);
            if (id == 0) return BadRequest();

           
            if (slides == null) return NotFound();
           

            return View(slides);
        }
    }
}
