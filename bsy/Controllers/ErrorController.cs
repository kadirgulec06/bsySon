using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace bsy.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }

        private ActionResult Index(RouteData r)
        {
            var DN = "";
            if (Session["USER"] != null)
                DN = ((User)Session["USER"]).KimlikNo;
            Exception e = (Exception)r.Values["exception"];
            ErrorModel em = new ErrorModel()
            {
                id = r.Values["guid"].ToString(),
                time = DateTime.Now,
                tckno = DN,
                message = e.Message,
                trace = e.StackTrace + e.Source
            };
            if (e.InnerException != null)
                em.trace += e.InnerException.StackTrace + e.InnerException.Source;
            return View("Error", em);
        }

        public ActionResult AnaSayfa()
        {
            return RedirectToAction("Index", "Home");
        }

        /*
        public ActionResult Details(string id)
        {
            ErrorModel em = db.Errors.Find(id);
            if (em == null)
            {
                throw new HttpException(404, "Erişmek istiğiniz kayda ulaşılamadı. Takip No: " + id);
            }
            return View(em);
        }
        */
        public ActionResult Http404()
        {
            //Response.StatusCode = 404;
            //return Content("404", "text/plain");
            return Index(RouteData);
        }

        public ActionResult Http500()
        {
            //Response.StatusCode = 500;
            //return Content("500", "text/plain");
            return Index(RouteData);
        }

        public ActionResult Http403()
        {
            //Response.StatusCode = 403;
            //return Content("403", "text/plain");
            return Index(RouteData);
        }

        public ActionResult Http401()
        {
            //Response.StatusCode = 401;
            //return Content("401", "text/plain");
            return Index(RouteData);
        }

    }
}