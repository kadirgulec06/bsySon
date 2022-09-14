using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace bsy.Helpers
{
    public static class GenelHelper
    {
        public static User getSabitUser()
        {
            User user = new User();

            user.AdSoyad = "Kadir Güleç";
            user.Eposta = "kgulec@spk.gov.tr";
            user.KimlikNo = "24461035746";
            user.menuRolleri = "YONETICI";
            user.Roller = "YONETICI";
            user.UserName = "kgulec";

            return user;
        }

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

        /*
        SmtpClient client = new SmtpClient();
        client.Port = 587;
                    client.Host = "smtp.gmail.com";
                    client.EnableSsl = true;
                    client.Timeout = 10000;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential("mail@gmail.com", "password");
                    MailMessage mm = new MailMessage(Text, Text, "Text", Text);
        mm.BodyEncoding = UTF8Encoding.UTF8;
                    mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    client.Send(mm);
                    MessageBox.Show("Mesajiniz gonderildi:)");
            */

        public static bool sendMail(string to, string toName, string konu, string mesaj)
        {
            var fromAddress = new MailAddress("kadirgulec59@gmail.com", "BSY Portalı");
            var toAddress = new MailAddress(to, toName);
            const string fromPassword = "homhrjgdsnimmsyz";
            //const string fromPassword = "Kato36Zato34Nato36.34";
            string subject = konu;
            string body = mesaj;

            var smtp = new SmtpClient
            {                
                Timeout = 10000,
                Host = "smtp.gmail.com",
                Port = 587,  //465
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                BodyEncoding = UTF8Encoding.UTF8,
                DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
            })
            {
                smtp.Send(message);
            }
            return true;
        }

        public static bool dizideVar(string aranan, string[] dizi)
        {
            foreach (string eleman in dizi)
            {
                if (eleman == aranan)
                {
                    return true;
                }
            }

            return false;
        }

        public static string sifreUret()
        {
            Random rg = new Random();
            string sifre = rg.Next(1000000, 9999999).ToString();

            return sifre;
        }

        public static User KullaniciBilgileri(bsyContext ctx, LogIn li)
        {
            User user = new User();
            KULLANICI kx = (from k in ctx.tblKullanicilar
                            where k.eposta == li.userName
                            select k).FirstOrDefault();
            if (kx == null)
            {
                return null;
            }

            user.AdSoyad = kx.Ad + " " + kx.Soyad;
            user.Eposta = li.userName;
            user.KimlikNo = kx.KimlikNo.ToString();
            user.menuRolleri = "";
            user.UserName = user.Eposta;
            user.Roller = KullaniciRolleri(ctx, kx.id);

            return user;
        }

        public static string KullaniciRolleri(bsyContext ctx, long userID)
        {
            KULLANICIROL rolleri = (from kr in ctx.tblKullaniciRolleri
                             where kr.userID == userID
                             select kr).FirstOrDefault();
            if (rolleri == null)
            {
                return "";
            }

            return rolleri.Rolleri;
        }

        public static bool GirisHakkiVar(bsyContext ctx, LogIn li)
        {
            DateTime ipSonAcmaTarihi = SonAcmaTarihiIP(ctx, li.ip);
            DateTime ipSonGirisTarihi = SonGirisTarihiIP(ctx, li.ip);

            DateTime epostaSonAcmaTarihi = SonAcmaTarihiEPosta(ctx, li.userName);
            DateTime epostaSonGirisTarihi = SonGirisTarihiEPosta(ctx, li.userName);

            int ipDenemeSayisi = (from gdx in ctx.tblGirisDenemeleri
                                where gdx.ip == li.ip && gdx.Durum == false &&
                                gdx.Tarih > ipSonAcmaTarihi && gdx.Tarih > ipSonGirisTarihi
                                select gdx).Count();

            int epostaDenemeSayisi = (from gdx in ctx.tblGirisDenemeleri
                                  where gdx.eposta == li.userName && gdx.Durum == false &&
                                  gdx.Tarih > epostaSonAcmaTarihi && gdx.Tarih > epostaSonGirisTarihi
                                  select gdx).Count();

            return ipDenemeSayisi <= SabitlerHelper.maxGirisDenemesi && epostaDenemeSayisi <= SabitlerHelper.maxGirisDenemesi; ;

        }

        public static DateTime SonAcmaTarihiIP(bsyContext ctx, string ip)
        {
            IPACMA sonAcma = (from gax in ctx.tblIPAcma
                                  where gax.ip == ip
                                  select gax).OrderByDescending(i => i.Tarih).FirstOrDefault();

            DateTime sonAcmaTarihi = new DateTime(2001, 01, 01);
            if (sonAcma != null)
            {
                sonAcmaTarihi = sonAcma.Tarih;
            }

            return sonAcmaTarihi;
        }

        public static DateTime SonGirisTarihiIP(bsyContext ctx, string ip)
        {
            GIRISDENEME sonGiris = (from gdx in ctx.tblGirisDenemeleri
                                  where gdx.ip == ip && gdx.Durum == true
                                  select gdx).OrderByDescending(i => i.Tarih).FirstOrDefault();

            DateTime sonGirisTarihi = new DateTime(2001, 01, 01);
            if (sonGiris != null)
            {
                sonGirisTarihi = sonGiris.Tarih;
            }

            return sonGirisTarihi;
        }

        public static DateTime SonAcmaTarihiEPosta(bsyContext ctx, string eposta)
        {
            EPOSTAACMA sonAcma = (from gax in ctx.tblEPostaAcma
                                  where gax.eposta == eposta
                                  select gax).OrderByDescending(i => i.Tarih).FirstOrDefault();

            DateTime sonAcmaTarihi = new DateTime(2001, 01, 01);
            if (sonAcma != null)
            {
                sonAcmaTarihi = sonAcma.Tarih;
            }

            return sonAcmaTarihi;
        }

        public static DateTime SonGirisTarihiEPosta(bsyContext ctx, string eposta)
        {
            GIRISDENEME sonGiris = (from gdx in ctx.tblGirisDenemeleri
                                    where gdx.eposta == eposta && gdx.Durum == true
                                    select gdx).OrderByDescending(i => i.Tarih).FirstOrDefault();

            DateTime sonGirisTarihi = new DateTime(2001, 01, 01);
            if (sonGiris != null)
            {
                sonGirisTarihi = sonGiris.Tarih;
            }

            return sonGirisTarihi;
        }

        public static bool DenemeKaydet(bsyContext ctx, LogIn li, bool durum)
        {
            GIRISDENEME gd = new GIRISDENEME();

            gd.Durum = durum;
            gd.eposta = li.userName;
            gd.id = 0;
            gd.ip = li.ip;
            gd.Tarih = DateTime.Now;

            ctx.tblGirisDenemeleri.Add(gd);
            return true;
        }

        public static string exceptionMesaji(Exception ex)
        {
            string mesaj = ex.Message;
            Exception ie = ex.InnerException;
            while (ie != null)
            {
                mesaj = mesaj + "\\r\\n" + ie.Message;
                ie = ie.InnerException;
            }

            return mesaj;
        }

    }
}