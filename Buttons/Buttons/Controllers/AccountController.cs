using Buttons.Data;
using Buttons.Filters;
using Buttons.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Buttons.Controllers
{
    public class AccountController : ControllerBase
    {
        public IDependency _dependency;

        public AccountController(IDependency dependency)
        {
            _dependency = dependency;
        }

        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login", "Account", new {  });
        }


        [HttpGet]
        public async Task<ActionResult> Get(string id)
        {
            if(id == null && this.IsLoggedIn)
            {
                id = this.CurrentUsername;
            }

            if (id != null)
            {
                var user = await _dependency.GetUserAsync(id);

                if (user != null)
                {
                    var model = new Models.Account.Get
                    {
                        Username = user.UserId,
                        QuestionsIHaveAnswered = user.QuestionsAttempted
                            .Where(a => a.Correct)
                            .OrderBy(q => q.AttemptedDate)
                            .Select(q => new Models.Shared.QuestionSummary
                            {
                                Answered = q.Correct,
                                Id = q.ID,
                                Title = q.Title
                            })
                    };
                    return View(model);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(Login model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            if(model.Username == "Adam" && model.Password == "password")
            {
                var user = await _dependency.GetUserAsync(model.Username);

                Session["UserID"] = model.Username;
                Session["AvatarUrl"] = string.Concat(user.AvatarUrl, "?s=25&d=identicon&r=PG");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid Login Attempt");
                return View(model);
            }
        }


        [HttpGet]
        [UserAuthenticationFilter]
        public ActionResult Logout()
        {
            Session.Remove("UserID");
            return RedirectToAction("Index", "Home");
        }
    }
}