using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace bsy.Filters
{
    //Yetki Kontrolü
    public class Yetkili : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            //Mesaj m = new Mesaj("hata", "Yetkisiz Erişim Talebi!!!");
            //mesajlar.Add(m);

            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (filterContext.HttpContext.Session["USER"] == null)
            {
                filterContext.HttpContext.Session["MESAJLAR"] = mesajlar;

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    action = "girisIndex",
                    controller = "Giris",
                    //action = "Index",
                    //controller = "Home",
                    area = ""
                }));
            }
            else
            {
                if (!AuthorizeCore(filterContext.HttpContext))
                {
                    Mesaj m = new Mesaj("hata", "Yetkisiz Erişim Talebi!!!");
                    mesajlar.Add(m);
                    filterContext.HttpContext.Session["MESAJLAR"] = mesajlar;

                    throw new Exception("Yetkisiz Erişim");

                }
            }
        }

        //Burada yetki kontrolü yapılıyor sisteme giriş yapan kişinin "User u"
        //kimlikno'su veya rolü eğer erişmek istediği sayfaya girmeye yetkili ise true
        //değilse false dönüyor
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            User u = (User)httpContext.Session["USER"];

            var _rolesSplit = Roles.Split(',');//Burada Roles bizim [Yetkili] de yazdığımız Roles
            var rolleri = u.Roller.Split(',');

            bool cnt = false;

            foreach (string rol in rolleri)//bu foreach kullanıcın menü elemanına yetkisi olup olmadığını kontrol ediyor
            {
                if (_rolesSplit.Contains(rol))
                {
                    cnt = true;
                    break;
                }
            }

            return cnt;
        }
    }
}