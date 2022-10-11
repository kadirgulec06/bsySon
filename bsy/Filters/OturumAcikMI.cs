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
            User user = (User)filterContext.HttpContext.Session["USER"];
            bool girisYapmis = false;
            if (user != null)
            {
                bsyContext ctx = new bsyContext();
                BILET bilet = ctx.tblBiletler.Where(bx => bx.UserID == user.id).FirstOrDefault();
                if (bilet != null && bilet.Bilet == user.bilet)
                {
                    girisYapmis = true;
                }
            }

            if (!girisYapmis) //Session da USER kalmamışsa ya da giriş yapmamışsa
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
