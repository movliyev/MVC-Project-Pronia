
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MVC_Project.Areas.Admin.ViewModels;


using MVC_Project.DAL;
using MVC_Project.Models;

namespace MVC_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
       
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
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

          ViewBag.Categorys = await _context.Categorys.ToListAsync();
            ViewBag.Sizes = await _context.Sizes.ToListAsync();
            ViewBag.Colors = await _context.Colors.ToListAsync();

            ViewBag.Tagids=await _context.Tags.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult>Create(CreateProductVM vm)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.Categorys = await _context.Categorys.ToListAsync();
                ViewBag.Sizes = await _context.Sizes.ToListAsync();
                ViewBag.Colors = await _context.Colors.ToListAsync();
                ViewBag.Tagids = await _context.Tags.ToListAsync();
                return View();
            }
            bool result = await _context.Categorys.AnyAsync(c => c.Id == vm.CategoryId);

            if(!result)
            {
                ViewBag.Categorys = await _context.Categorys.ToListAsync();
                ViewBag.Sizes = await _context.Sizes.ToListAsync();
                ViewBag.Colors = await _context.Colors.ToListAsync();
                ViewBag.Tagids = await _context.Tags.ToListAsync();
                ModelState.AddModelError("CategoryId", "Bu id li category movcud deyil");
                return View();
            }
            foreach (int item in vm.Tagids)
            {
                bool tagresult=await _context.Tags.AnyAsync(t=>t.Id == item);
                if (!tagresult)
                {
                    ViewBag.Categorys = await _context.Categorys.ToListAsync();
                    ViewBag.Sizes = await _context.Sizes.ToListAsync();
                    ViewBag.Colors = await _context.Colors.ToListAsync();
                    ViewBag.Tagids = await _context.Tags.ToListAsync();
                    ModelState.AddModelError("TagIds", "Bu id li tag movcud deyil");
                    return View();
                }
            }
            foreach (int item in vm.Colorids)
            {
                bool cresult = await _context.Colors.AnyAsync(t => t.Id == item);
                if (!cresult)
                {
                    ViewBag.Categorys = await _context.Categorys.ToListAsync();
                    ViewBag.Sizes = await _context.Sizes.ToListAsync();
                    ViewBag.Colors = await _context.Colors.ToListAsync();
                    ViewBag.Tagids = await _context.Tags.ToListAsync();
                    ModelState.AddModelError("Colorids", "Bu id li color movcud deyil");
                    return View();
                }
            }
            foreach (int item in vm.Sizeids)
            {
                bool sresult = await _context.Sizes.AnyAsync(t => t.Id == item);
                if (!sresult)
                {
                    ViewBag.Categorys = await _context.Categorys.ToListAsync();
                    ViewBag.Sizes = await _context.Sizes.ToListAsync();
                    ViewBag.Colors = await _context.Colors.ToListAsync();
                    ViewBag.Tagids = await _context.Tags.ToListAsync();
                    ModelState.AddModelError("TagIds", "Bu id li size movcud deyil");
                    return View();
                }
            }

            Product product = new Product
            {
                SKU = vm.SKU,
                Description = vm.Description,   
                Name = vm.Name, 
                Price = vm.Price,
                CategoryId =(int) vm.CategoryId,
                ProductTags=new List<ProductTag>(),
                ProductSizes = new List<ProductSize>(),
                ProductColors = new List<ProductColor>()


            };

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

            foreach (var item in exsist.ProductTags)
            {
                if (!pvm.Tagids.Exists(t => t == item.TagId))
                {
                    _context.ProductTags.Remove(item);
                }
            }

            foreach (int item in pvm.Tagids)
            {
                if (!exsist.ProductTags.Any(p => p.TagId == item))
                {
                    exsist.ProductTags.Add(new ProductTag
                    { TagId = item });

                }
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
