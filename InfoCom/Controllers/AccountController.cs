﻿using DataAccess;
using DataAccess.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using NHibernate.Linq;
using InfoCom.ViewModels;
using System.Web;

namespace InfoCom.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        [AllowAnonymous]
        public ActionResult Login()
        {

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                using (var session = DbConnect.SessionFactory.OpenSession())
                {
                    var usernameCheck = session.Query<User>().FirstOrDefault(x => x.Username == model.Username);
                    if (usernameCheck == null)
                    {
                        TempData["Message"] = "Invalid username or password";
                        TempData["Type"] = "alert-danger";
                        return View(model);
                    }
                    var passwordCheck =
                        session.Query<User>()
                            .Where(x => x.Username.Equals(model.Username))
                            .Select(x => x.Password)
                            .Single();

                    if (BCrypt.Net.BCrypt.Verify(model.Password, passwordCheck))
                    {
                        var identity = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier,
                                session.Query<User>()
                                    .Where(x => x.Username.Equals(model.Username))
                                    .Select(x => x.Id)
                                    .Single()
                                    .ToString()),
                            new Claim(ClaimTypes.Name, model.Username)
                        }, "InfoComCookie");

                        if(model.Username == "Admin")
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Authentication, "Admin"));
                        }

                        var authManager = Request.GetOwinContext().Authentication;
                        authManager.SignIn(identity);

                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToAction("Index", "Error");
            }

            TempData["Message"] = "Invalid username or password";
            TempData["Type"] = "alert-danger";
            return View(model);
        }
    }
}