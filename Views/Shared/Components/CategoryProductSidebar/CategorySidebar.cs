using Microsoft.AspNetCore.Mvc;
using mvcblog.Models;

namespace   App.Components{
    [ViewComponent]
public class CategoryProductSidebar : ViewComponent {
    public class CategorysidebarData {
        public List<CategoryProduct> categories {get;set;}
        public int level {get;set;}
        public string categoryslug {get;set;}
    }
    public IViewComponentResult Invoke(CategorysidebarData data ){
        return View(data);
    }
}
}