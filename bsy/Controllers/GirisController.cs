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

            li.girisAktif = GenelHelper.GirisHakkiVar(context, li);
            if (!li.girisAktif)
            {
                li.mesaj = "Sisteme Giriş yapma hakkınız kalmamış, sistem yöneticisine bilgi veriniz";
                m = new Mesaj("hata", "Sisteme Giriş yapma hakkınız kalmamış");
                mesajlar.Add(m);
                Session["MESAJLAR"] = mesajlar;
                return View(li);
            }

            bool sifreDogru = KullaniciSifreDogruMU(li);
            context.SaveChanges();
            if (sifreDogru)
            {
                Session["USER"] = GenelHelper.KullaniciBilgileri(context, li);
                Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Home/Index", false);
                return Content("OK");
            }

            m = new Mesaj("hata", "Kullanıcı Adı veya Şifresi Hatalı");
            li.mesaj = "Kullanıcı Adı veya Şifresi Hatalı";
            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;
            return View(li);

        }

        private bool KullaniciSifreDogruMU(LogIn li)
        {
            string sifreliSifre = GenelHelper.CreateSHA512(li.sifre);
            var user = (from ux in context.tblKullanicilar
                        where ux.eposta == li.userName
                        select ux).FirstOrDefault();

            bool denemeKaydedildi = false;
            if (user == null || sifreliSifre != user.Sifre)
            {
                denemeKaydedildi = GenelHelper.DenemeKaydet(context, li, false);
                return false;
            }

            denemeKaydedildi = GenelHelper.DenemeKaydet(context, li, true);
            return true;
        }
        private User sabitKullanici()
        {
            User usr = new User();
            usr.AdSoyad = "Kadir Güleç";
            usr.Eposta = "kgulec@spk.gov.tr";
            usr.KimlikNo = "24461035746";
            usr.menuRolleri = "";
            usr.Roller = "YONETICI";
            usr.UserName = "kgulec";

            //Session["USER"] = usr;
            return usr;
        }
    }
}

