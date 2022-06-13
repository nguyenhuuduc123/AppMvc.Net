using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Models;
using App.mvc.net.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvcblog.Models;

namespace App.mvc.net.Areas_Blog_Controllers_
{
    [Area("Blog")]
    public class ViewPostController : Controller
    {
        private readonly ILogger<ViewPostController> _logger;
        private readonly AppDbContext _context;

        public ViewPostController(ILogger<ViewPostController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        //post 
        // post/{categoryslug?}
        [Route("/post/{categoryslug?}")]
        public IActionResult Index(string categoryslug, [FromQuery(Name = "p")]int currentPage,int pagesize)
        {
            var categories = Getcategories();
            ViewBag.categories = categories;
            ViewBag.categoryslug = categoryslug;
            Category category = null;
            if(!string.IsNullOrEmpty(categoryslug)){
                category = _context.Categories.Where(c => c.Slug == categoryslug).Include(c => c.CategoryChildren).FirstOrDefault();
                if(category == null){
                    return NotFound("không thấy category");
                }
                
            }
           // return Content(categorysly);
           var posts = _context.posts.Include(p => p.Author).Include( p => p.PostCategories).ThenInclude( p => p.Category).AsQueryable();
           posts.OrderByDescending(p => p.DateUpdated);

            if(category != null){
                var ids = new List<int>();
                category.ChildCategoryIds(null,ids);
                ids.Add(category.Id);
               posts =  posts.Where(p => p.PostCategories.Where(pc => ids.Contains(pc.CategoryID)).Any());
            }

              int totalPost = posts.Count();
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
              var potsinPage = posts.Skip((currentPage - 1) * pagesize)
                        .Take(pagesize);


           ViewBag.category = category;
            return View(potsinPage.ToList());

        }
        [Route("/post/{postslug}.html")]
        public IActionResult Detail(string postslug){
                var categories = Getcategories();
            ViewBag.categories = categories;
            var post = _context.posts.Where(p => p.Slug == postslug).Include(p => p.Author).Include ( p => p.PostCategories).ThenInclude( p => p.Category).FirstOrDefault();
            if(post == null){
                return NotFound("không thấy bài viết");
            }
            Category category = post.PostCategories.FirstOrDefault()?.Category;
            ViewBag.category = category;
            var otherpost1 = _context.posts.Where(p => p.PostCategories.Any(c => c.CategoryID == category.Id)).Where(p => p.PostId != post.PostId).
                                OrderByDescending(p => p.DateUpdated).Take(5);
                
            ViewBag.otherpost1 = otherpost1.ToList();
            
               return View(post);
        }
        private List<Category> Getcategories(){
            var categories = _context.Categories.Include(c => c.CategoryChildren).AsEnumerable().Where(c => c.ParentCategory == null).ToList();
            return categories;
        }
    }
}