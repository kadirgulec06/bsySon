using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

            user = GenelHelper.getSabitUser();

            filterContext.HttpContext.Session["USER"] = user;

            menuHazirla(user, filterContext);

            oturumDegerleri(filterContext);

            filterContext.HttpContext.Session["USER"] = user;

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
            user = GenelHelper.getSabitUser();
            if (filterContext.HttpContext.Session["bsyMenusu"] == null)
            {
                filterContext.HttpContext.Session["bsyMenusu"] = MenuBuilder.BuildMenu((User)filterContext.HttpContext.Session["USER"], 0);
            }
        }
    }
}