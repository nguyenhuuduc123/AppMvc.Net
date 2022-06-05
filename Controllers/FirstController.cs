using Microsoft.AspNetCore.Mvc;
using System.IO;
namespace  App.mvc.net.Controllers{


    
    public class FirstController : Controller{
        private readonly ILogger<FirstController> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly ProductServices _productservices;
        public FirstController(ILogger<FirstController> logger,IWebHostEnvironment env,ProductServices productservices){
                _logger = logger;
                _env = env;
                _productservices = productservices;
        }
        
        
        


        public string Index(){
            _logger.LogInformation("index action");
             _logger.LogCritical("index action");
             _logger.LogWarning("index action");
            return "toi la index cua first";
        }
        public void nothing(){
            _logger.LogInformation("thong tin ");
            this.Response.Headers.Add("hi","xin chao cac ban");
        }
        public IActionResult Readme(){
        string context = @"toi ten la
        nguyen huu duc";
          
     return    this.Content(context,"text/plan");
    }
  //  [Route("grfg",Order = 1,Name ="sds")]
    public IActionResult bird(){
        string path = Path.Combine(_env.ContentRootPath,"Files","bird.webp");
        var bytes =  System.IO.File.ReadAllBytes(path);
        return File(bytes,"img/webp");
        
    }
    public IActionResult Iphoneprice(){
        return Json(new {
            Name = "iphonex",
            Price = 9000
        });
    }
    public IActionResult Privacy(){
        return LocalRedirect("~/home/index");
    }
    [TempData]
    public string statusmessage {get;set;}
    [AcceptVerbs("POST","GET")]
    public IActionResult ViewProduct(int? id){
        var product = _productservices.Where(p => p.id == id).FirstOrDefault();
        if(product == null) {
       //  TempData["statusmessage"] = "san pham ban yeu cau khong co"; 
       statusmessage = "sản phẩm bạn yêu cầu không có";
            return  LocalRedirect("~/home/index");
        }
        return View(product);
        
    } 
    }

}