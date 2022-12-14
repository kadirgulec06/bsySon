using bsy.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.Controllers
{
    [OturumAcikMI]
    [Yetkili(Roles = "YONETICI,SAHAGOREVLISI")]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
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

        public ActionResult Cikis()
        {
            Session["USER"] = null;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Giris/girisIndex", false);
            return Content("OK");
        }

    }
}