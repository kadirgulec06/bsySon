using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bsy.Helpers
{
    public static class SozlukHelper
    {
        public static string rolKodu = "ROL";
        public static string sehirKodu = "SEHIR";

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

    }


}