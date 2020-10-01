using Buttons.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Buttons.Controllers
{
    [SessionFilter]
    public abstract class ControllerBase : Controller
    {        
        protected bool IsLoggedIn
        {
            get
            {
                return Session["UserID"] != null;
            }
        }      
    }
}