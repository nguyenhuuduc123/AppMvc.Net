using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.mvc.net.Models;
using razorweb.models;
using Microsoft.AspNetCore.Authorization;
using App.Data;
using App.Models;
using Microsoft.AspNetCore.Identity;
using  app.areas.models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using app.models.Product.photo;

namespace app.areas.Blogxys
{
    [Area("Product")]
    [Route("admin/blog/productmanage/[action]/{id?}")]
    [Authorize(Roles =RoleName.Administrator + "," + RoleName.Editor)]
    public class ProductManageController : Controller
    {
        private readonly AppDbContext _context;
       private readonly UserManager<AppUser> _userManager;
        public ProductManageController(AppDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager   = userManager;
        }

        // GET: Post
        public async Task<IActionResult> Index([FromQuery(Name ="p")]int currentPage,int pagesize)
        {
           // var appDbContext = _context.Products.Include(p => p.Author);
           // return View(await appDbContext.ToListAsync());
           var Products = _context.Products.Include(p => p.Author).OrderByDescending(p => p.DateUpdated);
           int totalPost = await Products.CountAsync();
           if (pagesize <=0 ) pagesize = 10;
           int countPages = (int)Math.Ceiling((double)totalPost /pagesize);
            if (currentPage > countPages)
                currentPage = countPages;
            if (currentPage < 1)
                currentPage = 1;
            var pagingmodel = new PagingModel(){
                countpages = countPages,
                currentpage = currentPage,
                generateUrl = (pagenumber) => Url.Action("Index",new {
                    p = pagenumber,
                    pagesize = pagesize
                })
            };
            ViewBag.pagingmodel = pagingmodel;
            ViewBag.totalPost = totalPost;
            ViewBag.postIndex = (currentPage -1)*pagesize;
             var potsinPage = await Products.Skip((currentPage - 1) * pagesize)
                        .Take(pagesize).Include(p => p.ProductcategoryProducts).ThenInclude(pc => pc.Category)
                        .ToListAsync();
                        
           return View(potsinPage);
        }
/*

  var model = new UserListModel();
            model.currentPage = currentPage;

            var qr = _userManager.Users.OrderBy(u => u.UserName);

            model.totalUsers = await qr.CountAsync();
            model.countPages = (int)Math.Ceiling((double)model.totalUsers / model.ITEMS_PER_PAGE);

            if (model.currentPage < 1)
                model.currentPage = 1;
            if (model.currentPage > model.countPages)
                model.currentPage = model.countPages;

            var qr1 = qr.Skip((model.currentPage - 1) * model.ITEMS_PER_PAGE)
                        .Take(model.ITEMS_PER_PAGE)
                        .Select(u => new UserAndRole() {
                            Id = u.Id,
                            UserName = u.UserName,
                        });

            model.users = await qr1.ToListAsync();
*/
        // GET: Post/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var post = await _context.Products
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Post/Create
        public async Task<IActionResult> CreateAsync()
        {
            var Category = await _context.CategoryProducts.ToListAsync();
           ViewData["Category"] =  new MultiSelectList(Category,"Id","Title");

           // ViewData["AuthorId"] = new SelectList(_context.Set<AppUser>(), "Id", "Id");
            return View();  
        }

        // POST: Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,Content,Published,CategoryIds,Price")] CreateProductModel  post)
        {
             var Category = await _context.CategoryProducts.ToListAsync();
           ViewData["Category"] =  new MultiSelectList(Category,"Id","Title");
             if(post.Slug == null){
            post.Slug = App.Utilities.AppUtilities.GenerateSlug(post.Title);
           }
           if ( await _context.Products.AnyAsync(p => p.Slug == post.Slug)){
            ModelState.AddModelError("Slug","nhập chuỗi url khác");
            return View(post);
           }
          
            if (ModelState.IsValid)
            {
                var user =await  _userManager.GetUserAsync(this.User);
                post.DateCreated  = post.DateUpdated = DateTime.Now;
                post.AuthorId =  user.Id;
                _context.Add(post);    
              if(post.CategoryIds != null){
                foreach (var cateid in post.CategoryIds)
                {
                    _context.Add(new Product_categoryProduct(){
                        CategoryID = cateid,
                        Product= post
                    });
                }
              }
                await _context.SaveChangesAsync();
                statusmessage = "vừa tạo bài viết mới";
                return RedirectToAction(nameof(Index));
            }
          //  ViewData["AuthorId"] = new SelectList(_context.Set<AppUser>(), "Id", "Id", post.AuthorId);
            return View(post);
        }

        // GET: Post/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

           // var post = await _context.Products.FindAsync(id);
           var product = await _context.Products.Include(p => p.ProductcategoryProducts).FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            var productEdit = new CreateProductModel(){
                ProductId = product.ProductId,
                Title  = product.Title,
                Content = product.Content,
                Description = product.Description,
                Slug = product.Slug,
                Published = product.Published,
                CategoryIds = product.ProductcategoryProducts.Select(pc => pc.CategoryID).ToArray(),
                Price = product.Price
            };
         var Category = await _context.CategoryProducts.ToListAsync();
           ViewData["Category"] =  new MultiSelectList(Category,"Id","Title");
            return View(productEdit);
        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Title,Description,Slug,Content,Published,CategoryIds,Price")] CreateProductModel post)
        {
            if (id != post.ProductId)
            {
                return NotFound();
            }
            var Category = await _context.CategoryProducts.ToListAsync();
           ViewData["Category"] =  new MultiSelectList(Category,"Id","Title");
            
             if(post.Slug == null){
            post.Slug = App.Utilities.AppUtilities.GenerateSlug(post.Title);
           }
           if ( await _context.Products.AnyAsync(p => p.Slug == post.Slug && p.ProductId != id)){
            ModelState.AddModelError("Slug","nhập chuỗi url khác");
            return View(post);
           }
            
            if (ModelState.IsValid)
            {
                try
                {
                    var ProductUpdate   = await _context.Products.Include(p => p.ProductcategoryProducts).FirstOrDefaultAsync(p => p.ProductId == id);
                    if(ProductUpdate == null){
                        return NotFound();
                    }
                   ProductUpdate.Title = post.Title;
                   ProductUpdate.Description = post.Description;
                   ProductUpdate.Content = post.Content;
                   ProductUpdate.Published = post.Published;
                   ProductUpdate.Slug = post.Slug;
                   ProductUpdate.DateUpdated = DateTime.Now;
                   ProductUpdate.Price = post.Price;
                    if(post.CategoryIds == null ){
                        post.CategoryIds = new int[] {};
                    }
                    var oldCateids = ProductUpdate.ProductcategoryProducts.Select(c => c.CategoryID).ToArray();
                    var newCateIds = post.CategoryIds;
                    var removeCateProducts = from postcate in ProductUpdate.ProductcategoryProducts
                                            where ( !newCateIds.Contains(postcate.CategoryID))
                                            select postcate;
                    _context.ProductcategoryProducts.RemoveRange(removeCateProducts);
                    var addCateIds = from cateId in newCateIds
                                    where !oldCateids.Contains(cateId)
                                    select cateId;
                    foreach (var cateid in addCateIds)
                    {
                        _context.ProductcategoryProducts.Add(new Product_categoryProduct(){
                                ProductId = id,
                                CategoryID = cateid
                        });
                    }
                   _context.Update(ProductUpdate);
                   await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                statusmessage = "vừa cập nhập bài viết";
                return RedirectToAction(nameof(Index));
            } ViewData["AuthorId"] = new SelectList(_context.Set<AppUser>(), "Id", "Id", post.AuthorId);
            return  View(post);
        }

        // GET: Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var post = await _context.Products
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }
        [TempData]
        public string statusmessage {get;set;}
        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'AppDbContext.Products'  is null.");
            }
            var post = await _context.Products.FindAsync(id);
            if (post != null)
            {
                _context.Products.Remove(post);
            }
            
            await _context.SaveChangesAsync();
            statusmessage = "bạn vừa xóa bài viết" + post.Title;
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
          return _context.Products.Any(e => e.ProductId == id);
        }


        public class UploadoneFile{
            [Required(ErrorMessage ="phải upload file")]
            [DataType(DataType.Upload)]
            [FileExtensions(Extensions ="png,jpg,jepg,gif")]
            [DisplayName("chọn file upload")]
            public IFormFile Fileuploads {set;get;}

        }   




        [HttpGet]
        public IActionResult UploadPhoto(int id){
            var product = _context.Products.Where(p => p.ProductId == id ).Include(p => p.Photos).FirstOrDefault();
            if(product == null){
                    return NotFound("khong tim thay san pham");

            }
            ViewData["product"] = product;
                return View(new UploadoneFile());
        }
        [HttpPost,ActionName("UploadPhoto")]
        public   async Task<IActionResult> UploadPhotoAsync(int id,[Bind("Fileuploads")] UploadoneFile f){
             var product = _context.Products.Where(p => p.ProductId == id ).Include(p => p.Photos).FirstOrDefault();
            if(product == null){
                    return NotFound("khong tim thay san pham");

            }
            ViewData["product"] = product;

            if(f != null){
                var file1 = System.IO.Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetExtension(f.Fileuploads.FileName);
                var file = Path.Combine("Uploads","Products",file1);
                using (var filestream = new FileStream(file,FileMode.Create)){
                    await  f.Fileuploads.CopyToAsync(filestream);
                }
                    _context.Add(new ProductPhoto(){
                        ProductId = product.ProductId,
                        FileName = file1
                    });
                    await _context.SaveChangesAsync();

            }





                return View(new UploadoneFile());
        }
        [HttpPost]
        public IActionResult ListPhotos(int id){
            var product = _context.Products.Where(p => p.ProductId == id ).Include(p => p.Photos).FirstOrDefault();
            if(product == null){
                    return Json(
                        new {
                            success = 0,
                            message = "không tìm thấy sản phẩm"

                        }
                    );

            }
        var listphoto  = product.Photos.Select(photo => new {
                id = photo.Id,
                path =  "/contents/Products/" + photo.FileName
            });
            return Json(new{
                success = 1,
                photos =  listphoto
            });
        }
        [HttpPost]
        public IActionResult DeletePhoto(int id){
                var photo = _context.ProductPhotos.Where(p => p.Id == id).FirstOrDefault();
                if(photo != null){
                    _context.Remove(photo);
                    _context.SaveChanges();
                    var filename = "Uploads/Products/" + photo.FileName;
                    System.IO.File.Delete(filename);
                }
                return Ok();
        }
        [HttpPost]
        public   async Task<IActionResult> UploadPhotoApi(int id,[Bind("Fileuploads")] UploadoneFile f){
             var product = _context.Products.Where(p => p.ProductId == id ).Include(p => p.Photos).FirstOrDefault();
            if(product == null){
                    return NotFound("khong tim thay san pham");

            }
           

            if(f != null){
                var file1 = System.IO.Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetExtension(f.Fileuploads.FileName);
                var file = Path.Combine("Uploads","Products",file1);
                using (var filestream = new FileStream(file,FileMode.Create)){
                    await  f.Fileuploads.CopyToAsync(filestream);
                }
                    _context.Add(new ProductPhoto(){
                        ProductId = product.ProductId,
                        FileName = file1
                    });
                    await _context.SaveChangesAsync();

            }





                return Ok();
        }
    }
}
