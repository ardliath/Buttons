using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Buttons.Controllers
{
    public class Entity
    {
        public string UserId { get; set; }
        public string Colour { get; set; }
    }

    public class Dependency : IDependency
    {
        public string GetText()
        {
            return "Injected ";
        }
    }

    public interface IDependency
    {
        string GetText();
    }

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

                this.ViewBag.Text = _dependency.GetText() + entity.Colour;
                return View();
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