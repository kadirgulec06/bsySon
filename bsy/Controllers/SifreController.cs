using bsy.Filters;
using bsy.Helpers;
using bsy.Models;
using bsy.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.Controllers
{
    [OturumAcikMI]
    [Yetkili(Roles = "YONETICI,SAHAGOREVLISI")]
    public class SifreController : Controller
    {
        private bsyContext context = new bsyContext();

        // GET: Sifre
        public ActionResult Index()
        {
            return View();
        }

        [Yetkili(Roles = "YONETICI,SAHAGOREVLISI")]
        public ActionResult SifreDegistir()
        {
            SifreDegisVM sdVM = sifreDegistirmeHazirla();

            return View(sdVM);
        }

        private SifreDegisVM sifreDegistirmeHazirla()
        {
            User user = (User)Session["USER"];
            SifreDegisVM sdVM = new SifreDegisVM();

            sdVM.UserID = user.id;
            sdVM.AdSoyad = user.AdSoyad;
            sdVM.eposta = user.eposta;

            return sdVM;
        }

        [ValidateAntiForgeryToken]
        [Yetkili(Roles = "YONETICI,SAHAGOREVLISI")]
        [HttpPost]
        public ActionResult SifreDegistir(SifreDegisVM sdVM)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            if (!ModelState.IsValid)
            {
                return View(sdVM);
            }

            bool hataVar = false;
            if (sdVM.yeniSifre.Length < SabitlerHelper.sifreBoyuMin)
            {
                m = new Mesaj("hata", "Şifre uzunluğu " + SabitlerHelper.sifreBoyuMin + " karakterden küçük olamaz");
                mesajlar.Add(m);
                hataVar = true;
            }

            User user = (User)Session["USER"];
            bool sifreDogru = SifreHelper.SifreDogruMU(context, user.eposta, sdVM.eskiSifre);
            if (!sifreDogru)
            {
                m = new Mesaj("hata", "Şifre hatalı");
                mesajlar.Add(m);
                hataVar = true;
            }

            if (sdVM.yeniSifre != sdVM.yeniSifreTekrar)
            {
                m = new Mesaj("hata", "Yeni şifreler uyuşmuyor");
                mesajlar.Add(m);
                hataVar = true;
            }

            if (hataVar)
            {
                Session["MESAJLAR"] = mesajlar;
                return View(sdVM);
            }

            string sifreliSifre = SifreHelper.CreateSHA512(sdVM.yeniSifre);
            KULLANICI kx = context.tblKullanicilar.Find(sdVM.UserID);
            kx.Sifre = sifreliSifre;
            context.Entry(kx).State = EntityState.Modified;
            bool sifreDegismeYaratildi = SifreHelper.SifreDegismeYarat(context, kx.id, user.AdSoyad + " tarafından şifre değiştirildi");
            context.SaveChanges();

            m = new Mesaj("bilgi", "Şifreniz değiştirilmiştir");
            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;
            Session["SifreDegistir"] = 0;

            return View(sdVM);
        }

        [Yetkili(Roles = "YONETICI,SAHAGOREVLISI")]
        public ActionResult SifreSifirla()
        {

            return View();
        }

        [HttpPost]
        [Yetkili(Roles = "YONETICI,SAHAGOREVLISI")]
        public ActionResult SifreSifirla(string dummy="")
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            User user = (User)Session["USER"];

            string yeniSifre = SifreHelper.SifreSifirla(context, user.eposta);

            m = new Mesaj("bilgi", "Yeni Sifreniz " + yeniSifre);
            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;
            bool gonder = SifreHelper.sifrepostasiGonder(context, user.eposta, yeniSifre);

            return View();
        }


        [Yetkili(Roles = "YONETICI")]
        public ActionResult SifreSifirlaYonetici()
        {
            return View();
        }

        [Yetkili(Roles = "YONETICI")]
        [HttpPost]
        public ActionResult SifreSifirlaYonetici(long UserID)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            User user = (User)Session["USER"];

            KULLANICI kx = context.tblKullanicilar.Find(UserID);

            string yeniSifre = SifreHelper.SifreSifirla(context, kx.eposta);

            m = new Mesaj("bilgi", kx.eposta + " kullanıcısının yeni şifresi " + yeniSifre);
            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;
            bool gonder = SifreHelper.sifrepostasiGonder(context, kx.eposta, yeniSifre);

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Kullanicilar/Index", false);
            return Content("OK");
        }
    }
}