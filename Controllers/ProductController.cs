using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace App.mvc.net.Controllers
{
    [Area("productmanage")]
    public class ProductController : Controller
    {
        private readonly ProductServices _productService;
        private readonly ILogger<ProductController> _ilogger;
        public ProductController(ProductServices productService,ILogger<ProductController> ilogger){
            _productService = productService;
            _ilogger = ilogger;
        }
        public IActionResult Index()
        {
            // Areas/AreaName/Views/ProductName/ActionName.cshtml
            var products = _productService.OrderBy(p => p.Name).ToList();
            return View(products);
        }
    }
}