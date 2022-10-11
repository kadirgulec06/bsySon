using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.Helpers
{
    public static class KullaniciHelper
    {
        public static User getSabitUser()
        {
            User user = new User();

            user.AdSoyad = "Kadir Güleç";
            user.eposta = "kgulec@spk.gov.tr";
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
            user.eposta = li.userName;
            user.KimlikNo = kx.KimlikNo.ToString();
            user.menuRolleri = "";
            user.UserName = user.eposta;
            user.Roller = KullaniciRolleri(ctx, kx.id);
            user.gy = gorevYerleriHazirla(ctx, user.id);
            user.bilet = Guid.NewGuid().ToString();
            bool biletSaklandi = BiletSakla(ctx, user);

            return user;
        }

        public static bool BiletSakla(bsyContext ctx, User user)
        {
            BILET eskiBilet = ctx.tblBiletler.Where(bx => bx.UserID == user.id).FirstOrDefault();
            if (eskiBilet == null)
            {
                eskiBilet = new BILET();
            }

            eskiBilet.UserID = user.id;
            eskiBilet.Bilet = user.bilet;
            if (eskiBilet.id == 0)
            {
                ctx.tblBiletler.Add(eskiBilet);
            }
            else
            {
                ctx.Entry(eskiBilet).State = System.Data.Entity.EntityState.Modified;
            }

            ctx.SaveChanges();

            return true;
        }
        public static string KullaniciRolleri(bsyContext ctx, long UserID)
        {
            KULLANICIROL rolleri = (from kr in ctx.tblKullaniciRolleri
                                    where kr.UserID == UserID
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

            DateTime epostaSonAcmaTarihi = SonAcmaTarihieposta(ctx, li.userName);
            DateTime epostaSonGirisTarihi = SonGirisTarihieposta(ctx, li.userName);

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

        public static DateTime SonAcmaTarihieposta(bsyContext ctx, string eposta)
        {
            epostaACMA sonAcma = (from gax in ctx.tblepostaAcma
                                  where gax.eposta == eposta
                                  select gax).OrderByDescending(i => i.Tarih).FirstOrDefault();

            DateTime sonAcmaTarihi = new DateTime(2001, 01, 01);
            if (sonAcma != null)
            {
                sonAcmaTarihi = sonAcma.Tarih;
            }

            return sonAcmaTarihi;
        }

        public static DateTime SonGirisTarihieposta(bsyContext ctx, string eposta)
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

        public static GorevYerleri gorevYerleriHazirla(bsyContext ctx, long UserID)
        {
            GorevYerleri gyx = new GorevYerleri();
            gyx.gy = gorevYerleri(ctx, UserID);
            int gyt = gorevYeriTuru(gyx.gy);
            if (gyt <= 0)
            {
                return gyx;
            }

            if (gyt == 1)
            {
                gyx.butunTurkiye = true;
            }

            gyx.mahalleler = gorevMahalleleri(ctx, gyx);
            return gyx;

        }

        public static SehirIlceMahalle kullaniciGorevSahalari(bsyContext ctx)
        {
            User user = (User)HttpContext.Current.Session["USER"];

            SehirIlceMahalle sim = new SehirIlceMahalle();

            sim.sehirler = gorevSehirleri(ctx, user.gy);
            sim.ilceler = gorevIlceleri(ctx, user.gy);
            sim.mahalleler = user.gy.mahalleler;

            return sim;
        }
        public static List<GorevYeri> gorevYerleri(bsyContext ctx, long UserID)
        {
            List<GorevYeri> gy = (from gs in ctx.tblGorevSahasi
                                  where gs.UserID == UserID
                                  select new GorevYeri
                                  {
                                      SehirID = gs.SehirID,
                                      IlceID = gs.IlceID,
                                      mahalleID = gs.MahalleID
                                  }).Distinct().ToList();
            return gy;
        }

        public static int gorevYeriTuru(List<GorevYeri> gy)
        {
            var gyt = (from gz in gy
                       where gz.SehirID == 0 && gz.IlceID == 0 && gz.mahalleID == 0
                       select gz).FirstOrDefault();

            var gyd = (from gx in gy
                       where gx.SehirID != 0 || gx.IlceID != 0 || gx.mahalleID != 0
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

        public static List<long> gorevSehirleri(bsyContext ctx, GorevYerleri gy)
        {
            List<long> sehirler = new List<long>();
            if (gy.butunTurkiye)
            {
                sehirler = (from sh in ctx.tblSozluk
                            where sh.Turu == SozlukHelper.sehirKodu
                            select sh.id).ToList();

                return sehirler;                                   
            }

            sehirler = (from sh in gy.gy
                           where sh.SehirID != 0
                           select sh.SehirID).
                           Distinct().ToList();

            return sehirler;
        }
        public static List<long> gorevIlceleri(bsyContext ctx, GorevYerleri gy)
        {
            List<long> ilceler = new List<long>();

            if (gy.butunTurkiye)
            {
                ilceler = (from sh in ctx.tblSozluk
                            where sh.Turu == SozlukHelper.ilceKodu
                            select sh.id).ToList();

                return ilceler;
            }

            var sehirIlceler = (from ic in ctx.tblSozluk
                                  where ic.Turu == SozlukHelper.ilceKodu
                                  select new
                                  {
                                      IlceID = ic.id,
                                      SehirID = ic.BabaID
                                  }).Distinct().ToList();

            ilceler = (from ix in sehirIlceler
                          join gx in gy.gy on ix.SehirID equals gx.SehirID
                          where gx.IlceID == 0 || gx.IlceID == ix.IlceID
                          select ix.IlceID).ToList();

            return ilceler;
        }

        public static List<long> gorevMahalleleri(bsyContext ctx, GorevYerleri gy)
        {
            List<long> mahalleler = null;
            List<long> gm = new List<long>();

            if (gy.butunTurkiye)
            {
                return gm;
            }

            foreach (GorevYeri gyx in gy.gy)
            {
                if (gyx.SehirID != 0)
                {
                    if (gyx.IlceID == 0)
                    {
                        mahalleler = sehrinMahalleleri(ctx, gyx.SehirID);
                    }

                    if (gyx.SehirID != 0 && gyx.IlceID != 0 && gyx.mahalleID == 0)
                    {
                        mahalleler = ilceninMahalleleri(ctx, gyx.IlceID);
                    }

                    if (gyx.SehirID != 0 && gyx.IlceID != 0 && gyx.mahalleID != 0)
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

        public static IEnumerable<SelectListItem> kullaniciSehirleriDD(bsyContext ctx, User user, long secilen = 0, int bosHepsiHicbiri = 0)
        {
            List<long> sehirler = gorevSehirleri(ctx, user.gy);
            var tumSehirler = (from sx in ctx.tblSozluk
                               where sx.Turu == SozlukHelper.sehirKodu
                               select new
                               {
                                   SehirID = sx.id,
                                   sehirADI = sx.Aciklama
                               }).ToList();

            IEnumerable<SelectListItem> sehirlerDD = (from sx in tumSehirler
                                                      join sy in sehirler on sx.SehirID equals sy
                                                      select new SelectListItem
                                                      {
                                                          Text = sx.sehirADI,
                                                          Value = sx.SehirID.ToString(),
                                                          Selected = sx.SehirID == secilen
                                                      }).ToList();
            return sehirlerDD;
        }

        public static IEnumerable<SelectListItem> kullaniciIlceleriDD(bsyContext ctx, User user, long secilen = 0, int bosHepsiHicbiri = 0)
        {
            List<long> ilceleri = gorevIlceleri(ctx, user.gy);
            var tumIlceler = (from ix in ctx.tblSozluk
                              where ix.Turu == SozlukHelper.ilceKodu
                              select new
                              {
                                  IlceID = ix.id,
                                  ilceADI = ix.Aciklama
                              }).ToList();

            IEnumerable <SelectListItem> sozlukListesi = (from ti in tumIlceler
                                                           join gi in ilceleri on ti.IlceID equals gi
                                                           select new SelectListItem
                                                           {
                                                               Value = ti.IlceID.ToString(),
                                                               Text = ti.ilceADI,
                                                               Selected = ti.IlceID.Equals(secilen)
                                                           }).ToList();

            SelectListItem hepsiHicbiri = new SelectListItem { Value = "0", Text = "_Bütün Hepsi", Selected = false };
            if (bosHepsiHicbiri == 2)
            {
                new SelectListItem { Value = "0", Text = "_Hiçbiri", Selected = false };
            }

            if (bosHepsiHicbiri > 0)
            {
                sozlukListesi = sozlukListesi.Prepend(hepsiHicbiri);
                sozlukListesi.Append(hepsiHicbiri);
            }

            return sozlukListesi;
        }

        public static IEnumerable<SelectListItem> kullaniciMahalleleriDD(bsyContext ctx, User user, long secilen = 0, int bosHepsiHicbiri = 0)
        {
            List<long> mahalleleri = gorevMahalleleri(ctx, user.gy);
            var tumMahalleler = (from ix in ctx.tblSozluk
                              where ix.Turu == SozlukHelper.mahalleKodu
                              select new
                              {
                                  mahalleID = ix.id,
                                  mahalleADI = ix.Aciklama
                              }).ToList();

            IEnumerable<SelectListItem> sozlukListesi = (from ti in tumMahalleler
                                                         join gi in mahalleleri on ti.mahalleID equals gi
                                                         select new SelectListItem
                                                         {
                                                             Value = ti.mahalleID.ToString(),
                                                             Text = ti.mahalleADI,
                                                             Selected = ti.mahalleID.Equals(secilen)
                                                         }).ToList();

            SelectListItem hepsiHicbiri = new SelectListItem { Value = "0", Text = "_Bütün Hepsi", Selected = false };
            if (bosHepsiHicbiri == 2)
            {
                new SelectListItem { Value = "0", Text = "_Hiçbiri", Selected = false };
            }

            if (bosHepsiHicbiri > 0)
            {
                sozlukListesi = sozlukListesi.Prepend(hepsiHicbiri);
                sozlukListesi.Append(hepsiHicbiri);
            }

            return sozlukListesi;
        }

        public static List<long> sehrinMahalleleri(bsyContext ctx, long SehirID)
        {
            List<long> sm = (from ix in ctx.tblIlceler
                  join mx in ctx.tblMahalleler on ix.id equals mx.IlceID
                  where ix.SehirID == SehirID
                  select mx.id).ToList();

            return sm;
        }

        public static List<long> ilceninMahalleleri(bsyContext ctx, long IlceID)
        {
            List<long> im = (from mx in ctx.tblMahalleler
                             where mx.IlceID == IlceID
                             select mx.id).ToList();

            return im;
        }

    }
}