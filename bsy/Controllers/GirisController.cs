using bsy.Helpers;
using bsy.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;

namespace bsy.Controllers
{
    public class GirisController : Controller
    {
        //
        // GET: /Giris/

        private bool sabitUser = false;
        private bsyContext context = new bsyContext();
        [WebMethod(EnableSession = true)]
        public ActionResult girisIndex()
        {
            //cultureName = CultureHelper.GetImplementedCulture(cultureName);

            List<Mesaj> mesajlar = new List<Mesaj>();

            //Session["USER"] = null;
            Session["layout"] = 0;
            Session["MESAJLAR"] = mesajlar;

            if (Session["USER"] != null)//bu SSO olmamış demek filter da
            {
                Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Home/Index", false);
                return null;
            }

            LogIn li = new LogIn();
            li.girisAktif = true;
            li.mesaj = "Lütfen eposta ve şifrenizle giriş yapınız...";

            return View(li);
        }

        [HttpPost]
        public ActionResult girisIndex(LogIn li)
        {
            //cultureName = CultureHelper.GetImplementedCulture(cultureName);

            Mesaj m;
            List<Mesaj> mesajlar = new List<Mesaj>();

            li.ip = HttpContext.Request.UserHostAddress;

            li.girisAktif = KullaniciHelper.GirisHakkiVar(context, li);
            if (!li.girisAktif)
            {
                li.mesaj = "Sisteme Giriş yapma hakkınız kalmamış, sistem yöneticisine bilgi veriniz";
                m = new Mesaj("hata", "Sisteme Giriş yapma hakkınız kalmamış");
                mesajlar.Add(m);
                Session["MESAJLAR"] = mesajlar;
                return View(li);
            }

            bool sifreDogru = SifreHelper.GirisSifresiDogruMU(context, li);
            context.SaveChanges();
            if (sifreDogru)
            {
                User user = KullaniciHelper.KullaniciBilgileri(context, li);
                Session["USER"] = user;
                Session["SifreDegistir"] = 1;
                Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Home/Index", false);
                return Content("OK");
            }

            m = new Mesaj("hata", "Kullanıcı Adı veya Şifresi Hatalı");
            li.mesaj = "Kullanıcı Adı veya Şifresi Hatalı";
            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;
            return View(li);

        }

        private bool BiletSakla(User user)
        {
            return true;
        }
        private User sabitKullanici()
        {
            User usr = new User();
            usr.AdSoyad = "Kadir Güleç";
            usr.eposta = "kgulec@spk.gov.tr";
            usr.KimlikNo = "24461035746";
            usr.menuRolleri = "";
            usr.Roller = "YONETICI";
            usr.UserName = "kgulec";

            //Session["USER"] = usr;
            return usr;
        }
    }
}

