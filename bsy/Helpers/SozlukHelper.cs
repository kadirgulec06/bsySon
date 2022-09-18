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
        public static string sehirKodu = "SEHIR";
        public static string ilceKodu = "ILCE";
        public static string mahalleKodu = "MAHALLE";

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

        public static IEnumerable<SelectListItem> sozlukKalemleriListesi(bsyContext ctx, string turu, long secilen = 0 )
        {
            IEnumerable<SelectListItem> sozlukListesi = ctx.tblSozluk.Where(item => item.Turu == turu).OrderBy(item => item.Aciklama)
              .Select(s => new SelectListItem
              {
                  Value = s.id.ToString(),
                  Text = s.Aciklama,
                  Selected = s.id.Equals(secilen)
              });

            return sozlukListesi;
        }

        public static IEnumerable<SelectListItem> sehrinIlceleri(bsyContext ctx, long sehirID, long secilen = 0)
        {
            var ilceler = from sx in ctx.tblSozluk
                          join icx in ctx.tblIlceler on sx.id equals icx.id
                          where icx.sehirID == sehirID
                          select new { sx.id, sx.Aciklama };
                          
            IEnumerable <SelectListItem> sozlukListesi = ilceler.OrderBy(item => item.Aciklama)
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

        public static long sozlukID(bsyContext ctx, SOZLUK sozluk)
        {
            SOZLUK soz = ctx.tblSozluk.Where(sx => sx.Kodu == sozluk.Kodu && sx.Aciklama == sozluk.Aciklama).FirstOrDefault();
            if (soz == null)
            {
                return 0;
            }

            return soz.id;
        }

    }


}