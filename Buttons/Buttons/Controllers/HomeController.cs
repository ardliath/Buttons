using Buttons.Data;
using Buttons.Data.Entities;
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
        public IDependency _dependency;

        public HomeController(IDependency dependency)
        {
            _dependency = dependency;
        }

        public ActionResult Index()
        {
            using (var client = CreateDocumentClient())
            {
                var uri = UriFactory.CreateDocumentCollectionUri("Buttons", "Entities");

                var entity = client.CreateDocumentQuery<Entity>(uri, new FeedOptions { MaxItemCount = 1 })
                    //.Where(s => s.CommandCode == commandCode)
                    .AsEnumerable<Entity>()
                    .FirstOrDefault();

                var model = new Models.Home.Index
                {
                    TheText = $"The colour from the entity is {entity.Colour}"
                };
                return View(model);
            }

        }

        private DocumentClient CreateDocumentClient()
        {
            var url = ConfigurationManager.AppSettings["DatabaseEndpoint"];
            var key = ConfigurationManager.AppSettings["DatabaseKey"];
            return new DocumentClient(new Uri(url), key);
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