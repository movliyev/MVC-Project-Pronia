
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MVC_Project.Areas.Admin.ViewModels;


using MVC_Project.DAL;
using MVC_Project.Models;
using MVC_Project.Utilities.Extensions;
using System.Drawing;

namespace MVC_Project.Areas.Admin.Controllers
{

    [Area("Admin")]

    public class ProductController : Controller
    {
       
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [Authorize(Roles = "Admin,Moderator")]

        public async Task <IActionResult> Index()
        {
            List<Product> products = await _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary==true))
                .ToListAsync(); 

            return View(products);
        }
        [Authorize(Roles = "Admin,Moderator")]

        public async Task<IActionResult> Create()
        {
            CreateProductVM cvm = new CreateProductVM();

          cvm.Categorys = await _context.Categorys.ToListAsync();
            cvm.Sizes = await _context.Sizes.ToListAsync();
            cvm.Colors = await _context.Colors.ToListAsync();
            cvm.Tags=await _context.Tags.ToListAsync();
            return View(cvm);
        }

        [HttpPost]
        public async Task<IActionResult>Create(CreateProductVM vm)
        {
            if(!ModelState.IsValid)
            {
                vm.Categorys = await _context.Categorys.ToListAsync();
                vm.Sizes = await _context.Sizes.ToListAsync();
                vm.Colors = await _context.Colors.ToListAsync();
                vm.Tags = await _context.Tags.ToListAsync();
                return View(vm);
            }
            bool result = await _context.Categorys.AnyAsync(c => c.Id == vm.CategoryId);

            if(!result)
            {
                vm.Categorys = await _context.Categorys.ToListAsync();
                vm.Sizes = await _context.Sizes.ToListAsync();
                vm.Colors = await _context.Colors.ToListAsync();
                vm.Tags = await _context.Tags.ToListAsync();
                ModelState.AddModelError("CategoryId", "Bu id li category movcud deyil");
                return View(vm);
            }
            foreach (int item in vm.Tagids)
            {
                bool tagresult=await _context.Tags.AnyAsync(t=>t.Id == item);
                if (!tagresult)
                {
                    vm.Categorys = await _context.Categorys.ToListAsync();
                    vm.Sizes = await _context.Sizes.ToListAsync();
                    vm.Colors = await _context.Colors.ToListAsync();
                    vm.Tags = await _context.Tags.ToListAsync();
                    ModelState.AddModelError("TagIds", "Bu id li tag movcud deyil");
                    return View(vm);
                }
            }
            foreach (int item in vm.Colorids)
            {
                bool cresult = await _context.Colors.AnyAsync(t => t.Id == item);
                if (!cresult)
                {
                    vm.Categorys = await _context.Categorys.ToListAsync();
                    vm.Sizes = await _context.Sizes.ToListAsync();
                    vm.Colors = await _context.Colors.ToListAsync();
                    vm.Tags = await _context.Tags.ToListAsync();
                    ModelState.AddModelError("Colorids", "Bu id li color movcud deyil");
                    return View(vm);
                }
            }
            foreach (int item in vm.Sizeids)
            {
                bool sresult = await _context.Sizes.AnyAsync(t => t.Id == item);
                if (!sresult)
                {
                    vm.Categorys = await _context.Categorys.ToListAsync();
                    vm.Sizes = await _context.Sizes.ToListAsync();
                    vm.Colors = await _context.Colors.ToListAsync();
                    vm.Tags = await _context.Tags.ToListAsync();
                    ModelState.AddModelError("TagIds", "Bu id li size movcud deyil");
                    return View(vm);
                }
            }

            if (!vm.MainPhoto.ValidateType("image/"))
            {
                vm.Categorys = await _context.Categorys.ToListAsync();
                vm.Sizes = await _context.Sizes.ToListAsync();
                vm.Colors = await _context.Colors.ToListAsync();
                vm.Tags = await _context.Tags.ToListAsync();
                ModelState.AddModelError("MainPhoto", "Fayl tipi uyqun deyil");
                return View(vm);
            }

            if (!vm.MainPhoto.ValidateSize(600))
            {
                vm.Categorys = await _context.Categorys.ToListAsync();
                vm.Sizes = await _context.Sizes.ToListAsync();
                vm.Colors = await _context.Colors.ToListAsync();
                vm.Tags = await _context.Tags.ToListAsync();
                ModelState.AddModelError("MainPhoto", "Fayl olcusu uyqun deyil");
                return View(vm);

            }
            if (!vm.HoverPhoto.ValidateType("image/"))
            {
                vm.Categorys = await _context.Categorys.ToListAsync();
                vm.Sizes = await _context.Sizes.ToListAsync();
                vm.Colors = await _context.Colors.ToListAsync();
                vm.Tags = await _context.Tags.ToListAsync();
                ModelState.AddModelError("HoverPhoto", "Fayl tipi uyqun deyil");
                return View(vm);
            }

            if (!vm.HoverPhoto.ValidateSize(600))
            {
                vm.Categorys = await _context.Categorys.ToListAsync();
                vm.Sizes = await _context.Sizes.ToListAsync();
                vm.Colors = await _context.Colors.ToListAsync();
                vm.Tags = await _context.Tags.ToListAsync();
                ModelState.AddModelError("HoverPhoto", "Fayl olcusu uyqun deyil");
                return View(vm);

            }

            ProductImage main = new ProductImage
            {
                IsPrimary = true,
                Url=await vm.MainPhoto.CreateFileAsync(_env.WebRootPath,"assets","images","website-images")
                
            };
            ProductImage hover = new ProductImage
            {
                IsPrimary = false,
                Url = await vm.HoverPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images")

            };
            Product product = new Product
            {
                SKU = vm.SKU,
                Description = vm.Description,   
                Name = vm.Name, 
                Price = vm.Price,
                CategoryId =(int) vm.CategoryId,
                ProductTags=new List<ProductTag>(),
                ProductSizes = new List<ProductSize>(),
                ProductColors = new List<ProductColor>(),
                ProductImages = new List<ProductImage> { main,hover}
                


            };

            TempData["Message"] = "";

            foreach (IFormFile photo in vm.Photos ?? new List<IFormFile>())
            {
                if (!photo.ValidateType("image/"))
                {
                    TempData["Message"] += $" <p class=\"text-danger\">{photo.FileName} ali file tipi uyqun deyil</p>";
                    continue;
                }
                if (!photo.ValidateSize(600))
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} ali file olcusu uyqun deyil</p>";

                    continue;
                }

                product.ProductImages.Add(new ProductImage
                {
                    IsPrimary = null,
                    Url = await photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images")

                });

            }

            foreach (var item in vm.Tagids)
            {
                ProductTag ptag = new ProductTag
                {
                    TagId = item,
                };
                product.ProductTags.Add(ptag);
            }
            foreach (var item in vm.Colorids)
            {
                ProductColor ctag = new ProductColor
                {
                    ColorId = item,
                };
                product.ProductColors.Add(ctag);
            }
            foreach (var item in vm.Sizeids)
            {
                ProductSize stag = new ProductSize
                {
                    SizeId = item,
                };
                product.ProductSizes.Add(stag);
            }
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));  
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult>Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products
                .Include(p => p.ProductTags)
                .Include(p=>p.ProductImages)
                .Include(p=>p.ProductColors)
                .Include(p=>p.ProductSizes)
                .FirstOrDefaultAsync(p=> p.Id == id);  
            if(product == null) return NotFound();

            UpdateProductVM vm = new UpdateProductVM
            {
                Name = product.Name,
                Price=product.Price,
                Description = product.Description,
                SKU = product.SKU,
                CategoryId = product.CategoryId,    
                ProductImages=product.ProductImages,
                Tagids=product.ProductTags.Select(p => p.TagId).ToList(), 
                Colorids = product.ProductColors.Select(p=>p.ColorId).ToList(),
                Sizeids = product.ProductSizes.Select(p=>p.SizeId).ToList(),    
                Categorys= await _context.Categorys.ToListAsync()  ,
                Tags=await _context.Tags.ToListAsync(),
                Colors=await _context.Colors.ToListAsync(),
                Sizes=await _context.Sizes.ToListAsync()


            };


            await _context.SaveChangesAsync();


            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,UpdateProductVM pvm)
        {
            Product exsist = await _context.Products
              .Include(p => p.ProductTags)
              .Include(p => p.ProductSizes)
              .Include(p => p.ProductColors)
              .Include(p => p.ProductImages)
              .FirstOrDefaultAsync(p => p.Id == id);
            if (!ModelState.IsValid)
            {
                pvm.Categorys=await _context.Categorys.ToListAsync();
                pvm.Tags=await _context.Tags.ToListAsync();
                pvm.Colors=await _context.Colors.ToListAsync(); 
                pvm.Sizes=await _context.Sizes.ToListAsync();
                pvm.ProductImages = exsist.ProductImages;

                return View(pvm);
            }
          
                if(exsist == null)return NotFound();
            if(pvm.MainPhoto is not null)
            {
                if (!pvm.MainPhoto.ValidateType("image/"))
                {
                    pvm.Categorys = await _context.Categorys.ToListAsync();
                    pvm.Sizes = await _context.Sizes.ToListAsync();
                    pvm.Colors = await _context.Colors.ToListAsync();
                    pvm.Tags = await _context.Tags.ToListAsync();
                    pvm.ProductImages=exsist.ProductImages;
                    ModelState.AddModelError("MainPhoto", "Fayl tipi uyqun deyil");
                    return View(pvm);
                }

                if (!pvm.MainPhoto.ValidateSize(600))
                {
                    pvm.Categorys = await _context.Categorys.ToListAsync();
                    pvm.Sizes = await _context.Sizes.ToListAsync();
                    pvm.Colors = await _context.Colors.ToListAsync();
                    pvm.Tags = await _context.Tags.ToListAsync();
                    pvm.ProductImages = exsist.ProductImages;

                    ModelState.AddModelError("MainPhoto", "Fayl olcusu uyqun deyil");
                    return View(pvm);

                }

            }
            if (pvm.HoverPhoto is not null)
            {
                if (!pvm.HoverPhoto.ValidateType("image/"))
                {
                    pvm.Categorys = await _context.Categorys.ToListAsync();
                    pvm.Sizes = await _context.Sizes.ToListAsync();
                    pvm.Colors = await _context.Colors.ToListAsync();
                    pvm.Tags = await _context.Tags.ToListAsync();
                    pvm.ProductImages = exsist.ProductImages;
                    ModelState.AddModelError("HoverPhoto", "Fayl tipi uyqun deyil");
                    return View(pvm);
                }

                if (!pvm.HoverPhoto.ValidateSize(600))
                {
                    pvm.Categorys = await _context.Categorys.ToListAsync();
                    pvm.Sizes = await _context.Sizes.ToListAsync();
                    pvm.Colors = await _context.Colors.ToListAsync();
                    pvm.Tags = await _context.Tags.ToListAsync();
                    pvm.ProductImages = exsist.ProductImages;

                    ModelState.AddModelError("HoverPhoto", "Fayl olcusu uyqun deyil");
                    return View(pvm);

                }

            }



            bool result=await _context.Categorys.AnyAsync(c=>c.Id==pvm.CategoryId);
            if(!result)
            {
                pvm.Categorys = await _context.Categorys.ToListAsync();
                pvm.Tags = await _context.Tags.ToListAsync();
                pvm.Colors = await _context.Colors.ToListAsync();
                pvm.Sizes = await _context.Sizes.ToListAsync();
                pvm.ProductImages = exsist.ProductImages;

                ModelState.AddModelError("CategoryId", "Bele bir category movcud deyil");
                return View(pvm);
            }

           exsist.ProductTags.RemoveAll(pt=>!pvm.Tagids.Exists(tid=>tid==pt.TagId));


            List<int>create=pvm.Tagids.Where(tid=>!exsist.ProductTags.Exists(pt=>pt.TagId==tid)).ToList();
            foreach (var tid in create)
            {
                bool tresult = await _context.Tags.AnyAsync(t => t.Id == tid);
                if (!tresult)
                {
                    pvm.Categorys = await _context.Categorys.ToListAsync();
                    pvm.Tags = await _context.Tags.ToListAsync();
                    pvm.Colors = await _context.Colors.ToListAsync();
                    pvm.Sizes = await _context.Sizes.ToListAsync();
                    pvm.ProductImages = exsist.ProductImages;

                    ModelState.AddModelError("CategoryId", "Bele bir tag movcud deyil");
                    return View(pvm);
                }
                exsist.ProductTags.Add(new ProductTag { TagId = tid });
            }

         
            foreach (var item in exsist.ProductColors)
            {
                if (!pvm.Colorids.Exists(t => t == item.ColorId))
                {
                    _context.ProductColors.Remove(item);
                }
            }

            foreach (int item in pvm.Colorids)
            {
                if (!exsist.ProductColors.Any(p => p.ColorId == item))
                {
                    exsist.ProductColors.Add(new ProductColor
                    { ColorId = item });

                }
            }
            foreach (var item in exsist.ProductSizes)
            {
                if (!pvm.Sizeids.Exists(t => t == item.SizeId))
                {
                    _context.ProductSizes.Remove(item);
                }
            }

            foreach (int item in pvm.Sizeids)
            {
                if (!exsist.ProductSizes.Any(p => p.SizeId == item))
                {
                    exsist.ProductSizes.Add(new ProductSize
                    { SizeId = item });

                }
            }

            if(pvm.MainPhoto != null)
            {
                string fileNAme = await pvm.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");

                ProductImage eximg=exsist.ProductImages.FirstOrDefault(p=>p.IsPrimary==true);
                eximg.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                exsist.ProductImages.Remove(eximg);

                exsist.ProductImages.Add(new ProductImage
                {
                    IsPrimary = true,   
                    Url=pvm.Name
                });
            }
            if (pvm.HoverPhoto != null)
            {
                string fileNAme = await pvm.HoverPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");

                ProductImage eximg = exsist.ProductImages.FirstOrDefault(p => p.IsPrimary == false);
                eximg.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                exsist.ProductImages.Remove(eximg);

                exsist.ProductImages.Add(new ProductImage
                {
                    IsPrimary = true,
                    Url = pvm.Name
                });
            }

            if(pvm.Imageids is null)
            {
                pvm.Imageids = new List<int>();
            }
            List<ProductImage> remov = exsist.ProductImages.Where(p => !pvm.Imageids.Exists(imgId => imgId == p.Id)&&p.IsPrimary==null).ToList();
            foreach (ProductImage item in remov)
            {
                item.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                exsist.ProductImages.Remove(item);
            }

            TempData["Message"] = "";

            foreach (IFormFile photo in pvm.Photos ?? new List<IFormFile>())
            {
                if (!photo.ValidateType("image/"))
                {
                    TempData["Message"] += $" <p class=\"text-danger\">{photo.FileName} ali file tipi uyqun deyil</p>";
                    continue;
                }
                if (!photo.ValidateSize(600))
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} ali file olcusu uyqun deyil</p>";

                    continue;
                }

                exsist.ProductImages.Add(new ProductImage
                {
                    IsPrimary = null,
                    Url = await photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images")

                });

            }

            exsist.Name = pvm.Name;
            exsist.Description = pvm.Description;   
            exsist.Price = pvm.Price;   
            exsist.SKU = pvm.SKU;   
            exsist.CategoryId = pvm.CategoryId;



            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete (int id)
        {
            if (id <= 0) return BadRequest();

            Product product=await _context.Products.Include(p=>p.ProductImages).FirstOrDefaultAsync(p=>p.Id==id);
            if(product == null) return NotFound();
            foreach (var item in product.ProductImages ?? new List<ProductImage>())
            {
                item.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
            }
            _context.Products.Remove(product);  
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }






        public async Task<IActionResult> Detail(int id)
        {

            Product product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductTags).ThenInclude(x => x.Tag)
                .Include(p => p.ProductColors).ThenInclude(x => x.Color)
                .Include(p => p.ProductSizes).ThenInclude(x => x.Size)
                .FirstOrDefaultAsync(x => x.Id == id);
           
            if (id == 0) return BadRequest();


            if (product == null) return NotFound();
            ViewBag.ProductTags = await _context.ProductTags.ToListAsync();
            ViewBag.ProductColors = await _context.ProductTags.ToListAsync();
            ViewBag.ProductSizes = await _context.ProductTags.ToListAsync();



            return View(product);
        }


    }
}
