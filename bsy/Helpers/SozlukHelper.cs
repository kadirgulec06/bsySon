using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.Helpers
{
    public static class SozlukHelper
    {
        public static string rolKodu = "ROL";
        public static string bolgeKodu = "BOLGE";
        public static string sehirKodu = "SEHIR";
        public static string ilceKodu = "ILCE";
        public static string mahalleKodu = "MAHALLE";
        public static string haneKodu = "HANE";
        public static string kisiKodu = "KISI";

        public static string RolKoduBul(long id, string rolKodu)
        {
            if (id == 0)
            {
                return sayiUret().ToString();
            }

            return rolKodu;
        }

        public static string RolKoduHazirla(long id)
        {
            string idStr = id.ToString();
            string sifirlar = GenelHelper.repeat("0", SabitlerHelper.SozlukKodBoyu - rolKodu.Length - idStr.Length);
            return rolKodu + sifirlar + idStr;
        }

        public static long sayiUret()
        {
            Random rg = new Random();
            long sayi = rg.Next(100000000, 999999999);

            return sayi;

        }

        public static IEnumerable<SelectListItem> sozlukTurleriListesi(bsyContext ctx, string secilen="", bool hepsiEkle = false)
        {
            IEnumerable<SelectListItem> turListesi = (ctx.tblSozlukTurleri.OrderBy(item => item.Tur)
              .Select(s => new SelectListItem
              {
                  Value = s.Tur.ToString(),
                  Text = s.Tur,
                  Selected = s.Tur.Equals(secilen)
              })).ToList();

            if (hepsiEkle)
            {
                turListesi = turListesi.Prepend(new SelectListItem { Value = "Hepsi", Text = "_Bütün Hepsi", Selected = false });
            }

            turListesi.Append(new SelectListItem { Value = "Hepsi", Text = "_Bütün Hepsi", Selected = false });

            return turListesi;
        }


        public static IEnumerable<SelectListItem> sozlukKalemleriListesi(bsyContext ctx, string turu, long secilen = 0, bool hepsiEkle = false )
        {
            IEnumerable<SelectListItem> sozlukListesi = (ctx.tblSozluk.Where(item => item.Turu == turu).OrderBy(item => item.Aciklama)
              .Select(s => new SelectListItem
              {
                  Value = s.id.ToString(),
                  Text = s.Aciklama,
                  Selected = s.id.Equals(secilen)
              })).ToList();

            if (hepsiEkle)
            {
                sozlukListesi = sozlukListesi.Prepend(new SelectListItem{ Value =  "0", Text = "_Bütün Hepsi", Selected=false });
            }
            sozlukListesi.Append(new SelectListItem { Value = "0", Text = "_Bütün Hepsi", Selected = false });

            return sozlukListesi;
        }

        public static IEnumerable<SelectListItem> sehrinIlceleri(bsyContext ctx, long sehirID, long secilen = 0)
        {
            var ilceler = (from sx in ctx.tblSozluk
                          join icx in ctx.tblIlceler on sx.id equals icx.id
                          where icx.sehirID == sehirID
                          select new { sx.id, sx.Aciklama }).ToList();

            ilceler.Add(new { id = (long)0, Aciklama = "_Bütün İlçeler" });

            IEnumerable<SelectListItem> sozlukListesi = ilceler.OrderBy(item => item.Aciklama)
              .Select(s => new SelectListItem
              {
                  Value = s.id.ToString(),
                  Text = s.Aciklama,
                  Selected = s.id.Equals(secilen)
              }).ToList();

            return sozlukListesi;
        }

        public static IEnumerable<SelectListItem> IlceninMahalleleri(bsyContext ctx, long ilceID, long secilen = 0)
        {
            var mahalleler = (from sx in ctx.tblSozluk
                          join icx in ctx.tblMahalleler on sx.id equals icx.id
                          where icx.ilceID == ilceID
                          select new { sx.id, sx.Aciklama }).ToList();

            mahalleler.Add(new { id = (long)0, Aciklama = "_Bütün Mahalleler" });

            IEnumerable<SelectListItem> sozlukListesi = mahalleler.OrderBy(item => item.Aciklama)
              .Select(s => new SelectListItem
              {
                  Value = s.id.ToString(),
                  Text = s.Aciklama,
                  Selected = s.id.Equals(secilen)
              }).ToList();

            return sozlukListesi;
        }

        public static string sozlukAciklama(bsyContext ctx, long id)
        {

            string aciklama = "";
            SOZLUK soz = ctx.tblSozluk.Find(id);
            if (soz != null)
            {
                aciklama = soz.Aciklama;
            }

            return aciklama;
        }

        /*
        public static long sozlukID(bsyContext ctx, SOZLUK sozluk)
        {
            SOZLUK soz = ctx.tblSozluk.Where(sx => sx.Kodu == sozluk.Kodu && sx.Aciklama == sozluk.Aciklama).FirstOrDefault();
            if (soz == null)
            {
                return 0;
            }

            return soz.id;
        }
        */

        public static string sozlukKalemi(bsyContext ctx, long id)
        {
            if (id == 0)
            {
                return "Bütün Hepsi";
            }

            SOZLUK sozluk = ctx.tblSozluk.Find(id);
            if (sozluk == null)
            {
                return "Tanımlanmamış";
            }

            return sozluk.Aciklama;
        }

        public static Kunye KunyeHazirla(bsyContext ctx, long id)
        {
            Kunye kunye = new Kunye();
            long birimID = id;
            long yeniBirim = 0;

            SOZLUK soz = ctx.tblSozluk.Find(birimID);
            while (soz != null)
            {
                yeniBirim = soz.BabaID;
                switch (soz.Turu.Trim())
                {
                    case "KISI":
                        kunye.KisiID = birimID;
                        kunye.AdSoyad = soz.Aciklama;
                        kunye.TCNo = soz.Parametre;
                        kunye.HaneID = KisiHelper.kisiHanesi(ctx, kunye.KisiID);
                        yeniBirim = kunye.HaneID;
                        break;
                    case "HANE":
                        kunye.HaneID = birimID;
                        kunye.Adres = haneAdresi(ctx, birimID);
                        kunye.HaneKODU = soz.Parametre;
                        kunye.haneBilgileri = soz.Aciklama;
                        break;
                    case "MAHALLE":
                        kunye.MahalleID = birimID;
                        kunye.Mahalle = soz.Aciklama;
                        kunye.MahalleKodu = soz.Parametre;
                        break;
                    case "ILCE":
                        kunye.IlceID = birimID;
                        kunye.Ilce = soz.Aciklama;
                        break;
                    case "SEHIR":
                        kunye.SehirID = birimID;
                        kunye.Sehir = soz.Aciklama;
                        break;
                    default:
                        break;
                }

                birimID = yeniBirim;
                soz = ctx.tblSozluk.Find(birimID);
            }

            return kunye;
        }

        public static string haneAdresi(bsyContext ctx, long id)
        {
            HANE hane = ctx.tblHaneler.Find(id);
            string adres = hane.Cadde + ";" + hane.Sokak + ";" + hane.Apartman + "-" + hane.Daire;
            return adres;
        }
        public static string haneAdresi(HANE hane)
        {
            string adres = hane.Cadde + ";" + hane.Sokak + ";" + hane.Apartman + "-" + hane.Daire;
            return adres;
        }

        public static SOZLUK haneSozlugu(bsyContext ctx, HANE hane)
        {
            SOZLUK sozluk = new SOZLUK();
            if (hane.id != 0)
            {
                sozluk = ctx.tblSozluk.Find(hane.id);
            }

            sozluk.Turu = SozlukHelper.haneKodu;
            sozluk.Aciklama = SozlukHelper.haneAdresi(hane);
            sozluk.Parametre = hane.HaneKodu;
            sozluk.BabaID = hane.MahalleID;

            return sozluk;

        }

        public static SOZLUK kisiSozlugu(bsyContext ctx, KISI kisi)
        {
            SOZLUK sozluk = new SOZLUK();
            if (kisi.id != 0)
            {
                sozluk = ctx.tblSozluk.Find(kisi.id);
            }

            sozluk.Turu = SozlukHelper.kisiKodu;
            sozluk.Aciklama = kisi.Ad + " " + kisi.Soyad;
            sozluk.Parametre = kisi.TCNo;
            sozluk.BabaID = 0;   // KisiHane Kaydı Babası 

            return sozluk;

        }

    }


}