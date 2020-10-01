using Buttons.Data;
using Buttons.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Buttons.Controllers
{

    public class HomeController : ControllerBase
    {
        public IDependency _dependency;

        public HomeController(IDependency dependency)
        {
            _dependency = dependency;
        }

        public async Task<ActionResult> Index()
        {
            var model = new Models.Home.Index
            {
            };
            return View(model);
        }
        
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}