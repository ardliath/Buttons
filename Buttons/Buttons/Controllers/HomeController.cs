using Buttons.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Buttons.Controllers
{

    public class HomeController : Controller
    {
        public IDependency _dependency;

        public HomeController(IDependency dependency)
        {
            _dependency = dependency;
        }

        public ActionResult Index()
        {
            var testEntity = _dependency.GetTestEntity();

            var model = new Models.Home.Index
            {
                TheText = $"The colour from the upserted test entity is {testEntity.Colour}"
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