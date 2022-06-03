﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using App.mvc.net.Models;

namespace App.mvc.net.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
     private readonly IWebHostEnvironment _env;

    public HomeController(ILogger<HomeController> logger,IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public IActionResult Index()
    {
       
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public IActionResult Readme(){
        string context = @"toi ten la
        nguyen huu duc
        
        
        ";
        
     return    this.Content(context,"text/plan");
    }
}
