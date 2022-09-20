using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bsy.Helpers
{
    public static class KullaniciHelper
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

            user.id = kx.id;
            user.AdSoyad = kx.Ad + " " + kx.Soyad;
            user.Eposta = li.userName;
            user.KimlikNo = kx.KimlikNo.ToString();
            user.menuRolleri = "";
            user.UserName = user.Eposta;
            user.Roller = KullaniciRolleri(ctx, kx.id);
            user.gy = gorevYerleriHazirla(ctx, user.id);

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

            return ipDenemeSayisi <= SabitlerHelper.maxIpDenemesi && epostaDenemeSayisi <= SabitlerHelper.maxGirisDenemesi; ;

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

        public static GorevYerleri gorevYerleriHazirla(bsyContext ctx, long userID)
        {
            GorevYerleri gyx = new GorevYerleri();
            List<GorevYeri> gy = gorevYerleri(ctx, userID);
            int gyt = gorevYeriTuru(gy);
            if (gyt <= 0)
            {
                return gyx;
            }

            if (gyt == 1)
            {
                gyx.butunTurkiye = true;
                return gyx;
            }

            gyx.mahalleler = gorevMahalleleri(ctx, gy);
            return gyx;

        }
        public static List<GorevYeri> gorevYerleri(bsyContext ctx, long userID)
        {
            List<GorevYeri> gy = (from gs in ctx.tblGorevSahasi
                                  where gs.userID == userID
                                  select new GorevYeri
                                  {
                                      sehirID = gs.SehirID,
                                      ilceID = gs.IlceID,
                                      mahalleID = gs.MahalleID
                                  }).Distinct().ToList();
            return gy;
        }

        public static int gorevYeriTuru(List<GorevYeri> gy)
        {
            var gyt = (from gz in gy
                       where gz.sehirID == 0 && gz.ilceID == 0 && gz.mahalleID == 0
                       select gz).FirstOrDefault();

            var gyd = (from gx in gy
                       where gx.sehirID != 0 || gx.ilceID != 0 || gx.mahalleID != 0
                       select gx).FirstOrDefault();

            if (gyt != null && gyd != null)
            {
                return -1;
            }

            if (gyt == null && gyd == null)
            {
                return 0;
            }

            if (gyt != null && gyd == null)
            {
                return 1;
            }

            return 2;
        }

        public static List<long> gorevMahalleleri(bsyContext ctx, List<GorevYeri> gy)
        {
            List<long> mahalleler = null;
            List<long> gm = new List<long>();
            foreach (GorevYeri gyx in gy)
            {
                if (gyx.sehirID != 0)
                {
                    if (gyx.ilceID == 0)
                    {
                        mahalleler = sehrinMahalleleri(ctx, gyx.sehirID);
                    }

                    if (gyx.sehirID != 0 && gyx.ilceID != 0 && gyx.mahalleID == 0)
                    {
                        mahalleler = ilceninMahalleleri(ctx, gyx.ilceID);
                    }

                    if (gyx.sehirID != 0 && gyx.ilceID != 0 && gyx.mahalleID != 0)
                    {
                        mahalleler.Add(gyx.mahalleID);
                    }

                    foreach (long mx in mahalleler)
                    {
                        gm.Add(mx);
                    }

                }
            }

            return gm;
        }

        public static List<long> sehrinMahalleleri(bsyContext ctx, long sehirID)
        {
            List<long> sm = (from ix in ctx.tblIlceler
                  join mx in ctx.tblMahalleler on ix.id equals mx.ilceID
                  where ix.sehirID == sehirID
                  select mx.id).ToList();

            return sm;
        }

        public static List<long> ilceninMahalleleri(bsyContext ctx, long ilceID)
        {
            List<long> im = (from mx in ctx.tblMahalleler
                             where mx.ilceID == ilceID
                             select mx.id).ToList();

            return im;
        }

    }
}