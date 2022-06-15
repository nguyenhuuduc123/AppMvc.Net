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
    [Area("Product")]
    public class ViewProductController : Controller
    {
        private readonly ILogger<ViewProductController> _logger;
        private readonly AppDbContext _context;
        private readonly CartService _cartservice;

        public ViewProductController(ILogger<ViewProductController> logger, AppDbContext context,CartService cartservice)
        {
            _logger = logger;
            _context = context;
            _cartservice = cartservice;
        }
        //post 
        // post/{productslug?}
        [Route("/product/{productslug?}")]
        public IActionResult Index(string productslug, [FromQuery(Name = "p")]int currentPage,int pagesize)
        {   
            var categories = Getcategories();
            ViewBag.categories = categories;
            ViewBag.categoryslug = productslug;
            CategoryProduct category = null;
            if(!string.IsNullOrEmpty(productslug)){
                category = _context.CategoryProducts.Where(c => c.Slug == productslug).Include(c => c.CategoryChildren).FirstOrDefault();
                if(category == null){
                    return NotFound("không thấy category");
                }
                
            }
           // return Content(categorysly);
           var Products = _context.Products.Include(p => p.Author).Include(p => p.Photos).Include( p => p.ProductcategoryProducts).ThenInclude( p => p.Category).AsQueryable();
           Products.OrderByDescending(p => p.DateUpdated);

            if(category != null){
                var ids = new List<int>();
                category.ChildCategoryIds(null,ids);
                ids.Add(category.Id);
               Products =  Products.Where(p => p.ProductcategoryProducts.Where(pc => ids.Contains(pc.CategoryID)).Any());
            }

              int totalPost = Products.Count();
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
              var potsinPage = Products.Skip((currentPage - 1) * pagesize)
                        .Take(pagesize);


           ViewBag.category = category;
            return View(potsinPage.ToList());

        }
        [Route("/product/{productslug}.html")]
        public IActionResult Detail(string productslug){
                var categories = Getcategories();
            ViewBag.categories = categories;
            var post = _context.Products.Where(p => p.Slug == productslug).Include(p => p.Author).Include(p => p.Photos).Include ( p => p.ProductcategoryProducts).ThenInclude( p => p.Category).FirstOrDefault();
            if(post == null){
                return NotFound("không thấy bài viết");
            }
            CategoryProduct category = post.ProductcategoryProducts.FirstOrDefault()?.Category;
            ViewBag.category = category;
            var otherproducts = _context.Products.Where(p => p.ProductcategoryProducts.Any(c => c.CategoryID == category.Id)).Where(p => p.ProductId != post.ProductId).
                                OrderByDescending(p => p.DateUpdated).Take(5);
                
            ViewBag.otherproducts = otherproducts.ToList();
            
               return View(post);
        }
        private List<CategoryProduct> Getcategories(){
            var categories = _context.CategoryProducts.Include(c => c.CategoryChildren).AsEnumerable().Where(c => c.ParentCategory == null).ToList();
            return categories;
        }
[Route ("addcart/{productid:int}", Name = "addcart")]
public IActionResult AddToCart ([FromRoute] int productid) {

    var product = _context.Products
        .Where (p => p.ProductId == productid)
        .FirstOrDefault ();
    if (product == null)
        return NotFound ("Không có sản phẩm");

    // Xử lý đưa vào Cart ...
    var cart = _cartservice.GetCartItems ();
    var cartitem = cart.Find (p => p.product.ProductId == productid);
    if (cartitem != null) {
        // Đã tồn tại, tăng thêm 1
        cartitem.quantity++;
    } else {
        //  Thêm mới
        cart.Add (new CartItem () { quantity = 1, product = product });
    }

    // Lưu cart vào Session
   _cartservice.SaveCartSession (cart);
    // Chuyển đến trang hiện thị Cart
    return RedirectToAction (nameof (Cart));
}
[Route ("/cart", Name = "cart")]
public IActionResult Cart () {
    return View (_cartservice.GetCartItems());
}
/// xóa item trong cart
[Route ("/removecart/{productid:int}", Name = "removecart")]
public IActionResult RemoveCart ([FromRoute] int productid) {
    var cart = _cartservice.GetCartItems();
    var cartitem = cart.Find (p => p.product.ProductId == productid);
    if (cartitem != null) {
        // Đã tồn tại, tăng thêm 1
        cart.Remove(cartitem);
    }

  _cartservice.SaveCartSession (cart);
    return RedirectToAction (nameof (Cart));
}
/// Cập nhật
[Route ("/updatecart", Name = "updatecart")]
[HttpPost]
public IActionResult UpdateCart ([FromForm] int productid, [FromForm] int quantity) {
    // Cập nhật Cart thay đổi số lượng quantity ...
    var cart =_cartservice.GetCartItems ();
    var cartitem = cart.Find (p => p.product.ProductId == productid);
    if (cartitem != null) {
        // Đã tồn tại, tăng thêm 1
        cartitem.quantity = quantity;
    }
  _cartservice.SaveCartSession (cart);
    // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
    return Ok();
}
    }
}