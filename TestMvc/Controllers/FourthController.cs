using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ruanmou.Framework.Extension;
using Ruanmou.Framework.ImageHelper;
using TestMvc.Utility;

namespace TestMvc.Controllers
{
    public class FourthController : Controller
    {
        // GET: Fourth
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string name, string verify, string password)
        {
            LoginResult result = this.HttpContext.UserLogin(name, password, verify);
            if (result == LoginResult.Success)
            {
                if (this.HttpContext.Session["CurrentUrl"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    string url = this.HttpContext.Session["CurrentUrl"].ToString();
                    this.HttpContext.Session["CurrentUrl"] = null;
                    return Redirect(url);
                }
            }
            else
            {
                ModelState.AddModelError("failed", result.GetRemark());
                return View();
            }
        }

        public FileContentResult VerifyCode()
        {
            string code = "";
            var bitMap = VerifyCodeHelper.CreateVerifyCode(out code);
            HttpContext.Session["CheckCode"] = code;
            var stream=new MemoryStream();
            bitMap.Save(stream,ImageFormat.Gif);
            return File(stream.ToArray(), "image/gif");
        }

        public void Verify()
        {
            string code = "";
            var bitmap = VerifyCodeHelper.CreateVerifyCode(out code);
            HttpContext.Session["CheckCode"] = code;
            bitmap.Save(Response.OutputStream,ImageFormat.Gif);
            Response.ContentType = "image/gif";
        }


}
}