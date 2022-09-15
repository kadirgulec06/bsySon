using bsy.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace bsy.Helpers
{
    public static class SifreHelper
    {
        public static string CreateSHA512(string strData)
        {
            var message = Encoding.UTF8.GetBytes(strData);
            using (var alg = SHA512.Create())
            {
                string hex = "";

                var hashValue = alg.ComputeHash(message);
                foreach (byte x in hashValue)
                {
                    hex += String.Format("{0:x2}", x);
                }

                return hex;
            }
        }

        public static string sifreUret()
        {
            Random rg = new Random();
            string sifre = rg.Next(1000000, 9999999).ToString();

            return sifre;
        }

        public static bool GirisSifresiDogruMU(bsyContext ctx, LogIn li)
        {

            bool sifreDogru = SifreDogruMU(ctx, li.userName, li.sifre);

            bool denemeKaydedildi = KullaniciHelper.DenemeKaydet(ctx, li, sifreDogru);

            return sifreDogru;
        }

        public static bool SifreDogruMU(bsyContext ctx, string eposta, string sifre)
        {
            string sifreliSifre = SifreHelper.CreateSHA512(sifre);
            var user = (from ux in ctx.tblKullanicilar
                        where ux.eposta == eposta
                        select ux).FirstOrDefault();

            if (user == null || sifreliSifre != user.Sifre)
            {
                return false;
            }

            return true;
        }

        public static string SifreSifirla(bsyContext ctx, string eposta)
        {
            User user = (User)HttpContext.Current.Session["USER"];

            string yeniSifre = sifreUret();
            string sifreliSifre = CreateSHA512(yeniSifre);
            KULLANICI kx = ctx.tblKullanicilar.Where(k => k.eposta == eposta).FirstOrDefault();
            kx.Sifre = sifreliSifre;
            ctx.Entry(kx).State = EntityState.Modified;
            bool sifirlamaYaratildi = SifreSifirlamaYarat(ctx, eposta, user.AdSoyad + " tarafından sıfırlama");

            ctx.SaveChanges();

            return yeniSifre;
        }

        public static bool sifrePostasiGonder(bsyContext ctx, string eposta, string sifre)
        {
            KULLANICI kx = ctx.tblKullanicilar.Where(ky => ky.eposta == eposta).FirstOrDefault();

            string mesaj = "BYS Portalına kaydınız yapılmıştır, şifreniz " + sifre;
            string konu = "BYS Portalı Üyeliği";
            bool gonderildi = GenelHelper.sendMail(eposta, kx.Ad + " " + kx.Soyad, konu, mesaj);

            return gonderildi;
        }

        public static bool SifreDegisecekMI()
        {
            bsyContext ctx = new bsyContext();
            User user = (User)HttpContext.Current.Session["User"];

            var sifreDegisme = (from sdx in ctx.tblSifreDegisme
                                where sdx.userID == user.id
                                orderby sdx.Tarih descending
                                select sdx).FirstOrDefault();

            if (sifreDegisme == null)
            {
                return true;
            }

            var sifreSifirla = (from sdx in ctx.tblSifreSifirla
                                where sdx.userID == user.id
                                orderby sdx.Tarih descending
                                select sdx).FirstOrDefault();

            if (sifreSifirla == null)
            {
                return false;
            }

            if (sifreSifirla.Tarih > sifreDegisme.Tarih)
            {
                return true;
            }

            return false;
        }

        public static bool SifreDegismeYarat(bsyContext ctx, long userID, string aciklama)
        {
            SIFREDEGISME sd = new SIFREDEGISME();

            sd.Aciklama = aciklama;
            sd.id = 0;
            sd.Tarih = DateTime.Now;
            sd.userID = userID;

            ctx.tblSifreDegisme.Add(sd);
            return true;
        }

        public static bool SifreSifirlamaYarat(bsyContext ctx, string eposta, string aciklama)
        {
            KULLANICI kx = ctx.tblKullanicilar.Where(ky => ky.eposta == eposta).FirstOrDefault();

            SIFRESIFIRLA sd = new SIFRESIFIRLA();

            sd.Aciklama = aciklama;
            sd.id = 0;
            sd.Tarih = DateTime.Now;
            sd.userID = kx.id;
            ctx.tblSifreSifirla.Add(sd);

            return true;
        }

    }
}