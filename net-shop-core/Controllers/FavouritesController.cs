﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ModestLiving.Controllers
{
    public class FavouritesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}