using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using MVC_Project.Areas.Admin.ViewModels;
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
        public async Task<IActionResult>Create(CreateSlideVM slidevm)
        {


            //if (slidevm.Photo is null)
            //{
            //    ModelState.AddModelError("Photo", "Shekil mutleq secilmelidir");
            //    return View();
            //}

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!slidevm.Photo.ValidateType("image/"))
            {
                ModelState.AddModelError("Photo", "File tipi uyqun deyil");
                return View();
            }

            if (!slidevm.Photo.ValidateSize(2*1024))
            {
                ModelState.AddModelError("Photo", "File olcusu 2-mb den boyuk olmamalidir");
                return View();

            }

            string filename = await slidevm.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "slider");

            Slide slide = new Slide
            {
                Image = filename,
                Title = slidevm.Title,
                Subtitle = slidevm.Subtitle,
                Description = slidevm.Description,
                Order = slidevm.Order

            };
            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult>Update(int id)
        {
            if (id <= 0) return BadRequest();
            Slide exsist = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (exsist == null) return NotFound();

            UpdateSlideVM slidevm = new UpdateSlideVM
            {
                Description=exsist.Description,
                Subtitle=exsist.Subtitle,
                Order = exsist.Order,
                Image=exsist.Image,
                Title= exsist.Title
            };

            return View(slidevm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSlideVM slidevm)
        {
           
            if (!ModelState.IsValid)
            {
                return View(slidevm);
            }

            Slide exsist = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (exsist == null) return NotFound();

            if (slidevm.Photo is not null)
            {
                if (!slidevm.Photo.ValidateType("image/"))
                {
                    ModelState.AddModelError("Photo", "File tipi uyqun deyil");
                    return View(slidevm);
                }

                if (!slidevm.Photo.ValidateSize(2 * 1024))
                {
                    ModelState.AddModelError("Photo", "File olcusu 2-mb den boyuk olmamalidir");
                    return View(slidevm);

                }
                string filename = await slidevm.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "slider");
                exsist.Image.DeleteFile(_env.WebRootPath, "assets", "images", "slider");
                exsist.Image = filename;
            }
            exsist.Title= slidevm.Title;  
            exsist.Subtitle= slidevm.Subtitle;    
            exsist.Order= slidevm.Order;
            exsist.Description= slidevm.Description;  
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
