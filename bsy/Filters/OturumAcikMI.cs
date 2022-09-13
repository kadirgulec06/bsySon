using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace bsy.Filters
{
    public class OturumAcikMI : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (filterContext.HttpContext.Session["USER"] == null) //Session da USER kalmamışsa
            {
                List<Mesaj> mesajlar = new List<Mesaj>();
                Mesaj m = new Mesaj("hata", "Oturumun Açık Kalma Süresini Aştığınız için Tekrar Giriş Yapmanız Gerekmektedir!!!");
                mesajlar.Add(m);

                filterContext.HttpContext.Session["MESAJLAR"] = mesajlar;

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    action = "girisIndex",
                    controller = "Giris",
                    area = ""
                }));
            }

            base.OnActionExecuting(filterContext);

        }

    }

}
