using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}