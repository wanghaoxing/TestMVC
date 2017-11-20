using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestMvc.Models;

namespace TestMvc.Controllers
{
    public class FirstController : Controller
    {
        // GET: First
        public ActionResult Index()
        {
            return View();
        }
        public string String()
        {
            return "This first string";
        }

        public string Time(int year, int month, int day)
        {
            return $"{year}年{month}月{day}";
        }

        public ActionResult Data(int? id)
        {
            ViewData["User1"] = new CurrentUser()
            {
                Id = 7,
                Name = "Y",
                Account = " ╰つ Ｈ ♥. 花心胡萝卜",
                Email = "莲花未开时",
                Password = "落单的候鸟",
                LoginTime = DateTime.Now
            };

            base.ViewData["Something"] = 12345;

            base.ViewBag.Name = "Eleven";
            base.ViewBag.Description = "Teacher";//js
            base.ViewBag.User = new CurrentUser()
            {
                Id = 7,
                Name = "IOC",
                Account = "限量版",
                Email = "莲花未开时",
                Password = "落单的候鸟",
                LoginTime = DateTime.Now
            };

            base.TempData["User"] = new CurrentUser()
            {
                Id = 7,
                Name = "CSS",
                Account = "季雨林",
                Email = "KOKE",
                Password = "落单的候鸟",
                LoginTime = DateTime.Now
            };//后台可以跨action  基于session
            //return View();
            if (id == null)
            {
                return RedirectToAction("TempDataPage");//302
            }
            else
            {
                return View("~/Views/First/Data2.cshtml", new CurrentUser()
                {
                    Id = 7,
                    Name = "一点半",
                    Account = "季雨林",
                    Email = "KOKE",
                    Password = "落单的候鸟",
                    LoginTime = DateTime.Now
                });
            }
        }

        public ActionResult TempDataPage()
        {
            ViewBag.User = TempData["User"];
            return View();
        }
    }
}