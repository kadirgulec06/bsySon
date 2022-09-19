using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.ViewModels.GorevSahasi
{
    public class GorevSahasiVM
    {
        public GorevSahasiVM()
        {
            id = 0;
            userID = 0;
            eposta = "";
            Ad = "";
            Soyad = "";
            BasTar = DateTime.Now;
            BitTar = new DateTime(3000, 1, 1);
            SehirID = 0;
            IlceID = 0;
            MahalleID = 0;

            Sehir = "";
            Ilce = "";
            Mahalle = "";                        
        }
        public long id { get; set; }   // Roller id
        public long userID { get; set; }
        public string eposta { get; set; }
        public string Ad { get; set; }
        public String Soyad { get; set; }

        public DateTime BasTar { get; set; }
        public DateTime BitTar { get; set; }
        public long SehirID { get; set; }
        public long IlceID { get; set; }
        public long MahalleID { get; set; }

        public string Sehir { get; set; }
        public string Ilce { get; set; }
        public string Mahalle { get; set; }

        public IEnumerable<SelectListItem> sehirler { get; set; }
        public IEnumerable<SelectListItem> ilceler { get; set; }
        public IEnumerable<SelectListItem> mahalleler { get; set; }
    }
}