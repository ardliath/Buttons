using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Buttons.Filters
{
    public class SessionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            Controller controller = filterContext.Controller as Controller;
            if (controller != null)
            {
                controller.ViewData["IsLoggedIn"] = filterContext.HttpContext.Session["UserId"] != null;                
            }            
        }
    }
}