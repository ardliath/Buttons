using Microsoft.Azure.Documents.Client;
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
        public ActionResult Index()
        {
            //using (var client = CreateDocumentClient())
            {
                //var uri = UriFactory.CreateDocumentCollectionUri("Buttons", "Entities");
                this.ViewBag.Text = ConfigurationManager.AppSettings["DatabaseEndpoint"];
                return View();
            }
        }

        //private DocumentClient CreateDocumentClient()
        //{
        //    var url = ConfigurationManager.AppSettings["DatabaseEndpoint"];
        //    var key = ConfigurationManager.AppSettings["DatabaseKey"];
        //    return new DocumentClient(new Uri(url), key);
        //}

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