using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.mvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.mvc.net.Controllers
{
    [Route("he-mat-troi/[action]")]
    public class PlanetController : Controller
    {
        private readonly PlanetServices _planetService;
        private readonly ILogger<PlanetController> _ilogger;
        public PlanetController(PlanetServices planetService, ILogger<PlanetController> ilogger){
                _planetService = planetService;
                _ilogger =  ilogger;
        }
        [Route("danh-sach-cac-hanh-tinh.html")]
        public IActionResult Index()
        {
            return View();
        }
        [BindProperty(SupportsGet = true, Name = "action")]
        public string Name {get;set;}
        // planet/ action = Mecury
        public IActionResult Mercury(){
          var planet =  _planetService.Where(p => p.Name == Name).FirstOrDefault();
            return View("Detail",planet);
        }
         public IActionResult Venus(){
          var planet =  _planetService.Where(p => p.Name == Name).FirstOrDefault();
            return View("Detail",planet);
        }
         public IActionResult Earth(){
          var planet =  _planetService.Where(p => p.Name == Name).FirstOrDefault();
            return View("Detail",planet);
        }
         public IActionResult Mars(){
          var planet =  _planetService.Where(p => p.Name == Name).FirstOrDefault();
            return View("Detail",planet);
        }
         public IActionResult Jupiter(){
          var planet =  _planetService.Where(p => p.Name == Name).FirstOrDefault();
            return View("Detail",planet);
        }
         public IActionResult Saturn(){
          var planet =  _planetService.Where(p => p.Name == Name).FirstOrDefault();
            return View("Detail",planet);
        }
         public IActionResult Uranus(){
          var planet =  _planetService.Where(p => p.Name == Name).FirstOrDefault();
            return View("Detail",planet);
        }
        [Route("sao",Order = 1, Name = "Neptune1")] // Order : độ ưu tiên phát sinh url 

         public IActionResult Neptune(){
          var planet =  _planetService.Where(p => p.Name == Name).FirstOrDefault();
            return View("Detail",planet);
         }
        [Route("hanh-tinh/{id:int}")]
         public IActionResult PlanetInfo(int id){
          var planet =  _planetService.Where(p => p.Id == id).FirstOrDefault();
            return View("Detail",planet);
        }

}
}