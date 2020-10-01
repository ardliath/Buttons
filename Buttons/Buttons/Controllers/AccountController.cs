﻿using Buttons.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Buttons.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login", "Account", new {  });
        }



        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Login model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            if(model.Username == "Adam" && model.Password == "password")
            {
                Session["UserID"] = Guid.NewGuid();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid Login Attempt");
                return View(model);
            }
        }


        [HttpGet]
        public ActionResult Logout()
        {
            Session.Remove("UserID");
            return RedirectToAction("Index", "Home");
        }
    }
}