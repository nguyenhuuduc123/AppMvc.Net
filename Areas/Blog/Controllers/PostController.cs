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
namespace app.areas.Blogxys
{
    [Area("Blog")]
    [Route("admin/blog/post/[action]/{id?}")]
    [Authorize(Roles =RoleName.Administrator + "," + RoleName.Editor)]
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
       private readonly UserManager<AppUser> _userManager;
        public PostController(AppDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager   = userManager;
        }

        // GET: Post
        public async Task<IActionResult> Index([FromQuery(Name ="p")]int currentPage,int pagesize)
        {
           // var appDbContext = _context.posts.Include(p => p.Author);
           // return View(await appDbContext.ToListAsync());
           var posts = _context.posts.Include(p => p.Author).OrderByDescending(p => p.DateUpdated);
           int totalPost = await posts.CountAsync();
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
             var potsinPage = await posts.Skip((currentPage - 1) * pagesize)
                        .Take(pagesize).Include(p => p.PostCategories).ThenInclude(pc => pc.Category)
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
            if (id == null || _context.posts == null)
            {
                return NotFound();
            }

            var post = await _context.posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Post/Create
        public async Task<IActionResult> CreateAsync()
        {
            var Category = await _context.Categories.ToListAsync();
           ViewData["Category"] =  new MultiSelectList(Category,"Id","Title");

           // ViewData["AuthorId"] = new SelectList(_context.Set<AppUser>(), "Id", "Id");
            return View();  
        }

        // POST: Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,Content,Published,CategoryIds")] CreatePostModel  post)
        {
             var Category = await _context.Categories.ToListAsync();
           ViewData["Category"] =  new MultiSelectList(Category,"Id","Title");
             if(post.Slug == null){
            post.Slug = App.Utilities.AppUtilities.GenerateSlug(post.Title);
           }
           if ( await _context.posts.AnyAsync(p => p.Slug == post.Slug)){
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
                    _context.Add(new PostCategory(){
                        CategoryID = cateid,
                        Post = post
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
            if (id == null || _context.posts == null)
            {
                return NotFound();
            }

           // var post = await _context.posts.FindAsync(id);
           var post = await _context.posts.Include(p => p.PostCategories).FirstOrDefaultAsync(p => p.PostId == id);
            if (post == null)
            {
                return NotFound();
            }
            var postEdit = new CreatePostModel(){
                PostId = post.PostId,
                Title  = post.Title,
                Content = post.Content,
                Description = post.Description,
                Slug = post.Slug,
                Published = post.Published,
                CategoryIds = post.PostCategories.Select(pc => pc.CategoryID).ToArray()
            };
         var Category = await _context.Categories.ToListAsync();
           ViewData["Category"] =  new MultiSelectList(Category,"Id","Title");
            return View(postEdit);
        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Description,Slug,Content,Published,CategoryIds")] CreatePostModel post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }
            var Category = await _context.Categories.ToListAsync();
           ViewData["Category"] =  new MultiSelectList(Category,"Id","Title");
            
             if(post.Slug == null){
            post.Slug = App.Utilities.AppUtilities.GenerateSlug(post.Title);
           }
           if ( await _context.posts.AnyAsync(p => p.Slug == post.Slug && p.PostId != id)){
            ModelState.AddModelError("Slug","nhập chuỗi url khác");
            return View(post);
           }
            
            if (ModelState.IsValid)
            {
                try
                {
                    var postUpdate   = await _context.posts.Include(p => p.PostCategories).FirstOrDefaultAsync(p => p.PostId == id);
                    if(postUpdate == null){
                        return NotFound();
                    }
                   postUpdate.Title = post.Title;
                   postUpdate.Description = post.Description;
                   postUpdate.Content = post.Content;
                   postUpdate.Published = post.Published;
                   postUpdate.Slug = post.Slug;
                   postUpdate.DateUpdated = DateTime.Now;
                    if(post.CategoryIds == null ){
                        post.CategoryIds = new int[] {};
                    }
                    var oldCateids = postUpdate.PostCategories.Select(c => c.CategoryID).ToArray();
                    var newCateIds = post.CategoryIds;
                    var removeCatePosts = from postcate in postUpdate.PostCategories
                                            where ( !newCateIds.Contains(postcate.CategoryID))
                                            select postcate;
                    _context.PostCategories.RemoveRange(removeCatePosts);
                    var addCateIds = from cateId in newCateIds
                                    where !oldCateids.Contains(cateId)
                                    select cateId;
                    foreach (var cateid in addCateIds)
                    {
                        _context.PostCategories.Add(new PostCategory(){
                                PostID = id,
                                CategoryID = cateid
                        });
                    }
                   _context.Update(postUpdate);
                   await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
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
            if (id == null || _context.posts == null)
            {
                return NotFound();
            }

            var post = await _context.posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.PostId == id);
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
            if (_context.posts == null)
            {
                return Problem("Entity set 'AppDbContext.Posts'  is null.");
            }
            var post = await _context.posts.FindAsync(id);
            if (post != null)
            {
                _context.posts.Remove(post);
            }
            
            await _context.SaveChangesAsync();
            statusmessage = "bạn vừa xóa bài viết" + post.Title;
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
          return _context.posts.Any(e => e.PostId == id);
        }
    }
}
