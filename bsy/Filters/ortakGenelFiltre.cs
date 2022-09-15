using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using bsy.Helpers;
using bsy.Models;
namespace bsy.Filters
{
    public class ortakGenelFiltre : ActionFilterAttribute
    {
        public bsyContext context = new bsyContext();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            User user = (User)filterContext.HttpContext.Session["USER"];

            //user = GenelHelper.getSabitUser();

            //filterContext.HttpContext.Session["USER"] = user;
            string controller = filterContext.RouteData.Values["controller"].ToString();
            string action = filterContext.RouteData.Values["action"].ToString();


            if (user != null)
            {
                int sifreDegistir = (int)filterContext.HttpContext.Session["SifreDegistir"];
                bool degistir = false;
                if (sifreDegistir == 1 && controller != "Sifre" && action != "SifreDegistir")
                {
                    degistir = SifreHelper.SifreDegisecekMI();
                }
                if (degistir)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        action = "SifreDegistir",
                        controller = "Sifre",
                        area = ""
                    }));
                }
                else
                {
                    menuHazirla(user, filterContext);

                    oturumDegerleri(filterContext);

                    filterContext.HttpContext.Session["USER"] = user;
                }
            }

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        private void oturumDegerleri(ActionExecutingContext filterContext)
        {
            DateTime buGun = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            DateTime buAy = DateTime.Parse("01." + buGun.Month.ToString() + "." + buGun.Year.ToString());
            filterContext.HttpContext.Session["buGun"] = buGun;
            filterContext.HttpContext.Session["buAy"] = buAy;
        }
        private void menuHazirla(User user, ActionExecutingContext filterContext)
        {
            user = KullaniciHelper.getSabitUser();
            if (filterContext.HttpContext.Session["bsyMenusu"] == null)
            {
                filterContext.HttpContext.Session["bsyMenusu"] = MenuBuilder.BuildMenu((User)filterContext.HttpContext.Session["USER"], 0);
            }
        }
    }
}