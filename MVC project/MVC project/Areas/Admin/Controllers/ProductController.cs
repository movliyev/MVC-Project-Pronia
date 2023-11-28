
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MVC_Project.Areas.Admin.ViewModels;


using MVC_Project.DAL;
using MVC_Project.Models;
using MVC_Project.Utilities.Extensions;

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
        public async Task <IActionResult> Index()
        {
            List<Product> products = await _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary==true))
                .ToListAsync(); 

            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            CreateProductVM cvm = new CreateProductVM();

          cvm.Categorys = await _context.Categorys.ToListAsync();
            cvm.Sizes = await _context.Sizes.ToListAsync();
            cvm.Colors = await _context.Colors.ToListAsync();
            cvm.Tags=await _context.Tags.ToListAsync();
            return View();
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

            return View(nameof(Index));  
        }


        public async Task<IActionResult>Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products
                .Include(p => p.ProductTags)
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
                Tagids=product.ProductTags.Select(p => p.TagId).ToList(), 
                Colorids = product.ProductColors.Select(p=>p.ColorId).ToList(),
                Sizeids = product.ProductSizes.Select(p=>p.SizeId).ToList(),    
                Categorys= await _context.Categorys.ToListAsync()  ,
                Tags=await _context.Tags.ToListAsync(),
                Colors=await _context.Colors.ToListAsync(),
                Sizes=await _context.Sizes.ToListAsync(),


            };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,UpdateProductVM pvm)
        {
            if (!ModelState.IsValid)
            {
                pvm.Categorys=await _context.Categorys.ToListAsync();
                pvm.Tags=await _context.Tags.ToListAsync();
                pvm.Colors=await _context.Colors.ToListAsync(); 
                pvm.Sizes=await _context.Sizes.ToListAsync();
                return View(pvm);
            }
            Product exsist = await _context.Products
                .Include(p=>p.ProductTags)
                .Include(p => p.ProductSizes)
                .Include(p => p.ProductColors)

                .FirstOrDefaultAsync(p => p.Id == id);
                if(exsist == null)return NotFound();

                bool result=await _context.Categorys.AnyAsync(c=>c.Id==pvm.CategoryId);
            if(!result)
            {
                pvm.Categorys = await _context.Categorys.ToListAsync();
                pvm.Tags = await _context.Tags.ToListAsync();
                pvm.Colors = await _context.Colors.ToListAsync();
                pvm.Sizes = await _context.Sizes.ToListAsync();
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
            exsist.Name = pvm.Name;
            exsist.Description = pvm.Description;   
            exsist.Price = pvm.Price;   
            exsist.SKU = pvm.SKU;   
            exsist.CategoryId = pvm.CategoryId;
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
