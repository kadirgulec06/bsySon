using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class Kunye
    {
        public long BolgeID { get; set; }
        public long SehirID { get; set; }
        public long IlceID { get; set; }
        public long MahalleID { get; set; }
        public long HaneID { get; set; }
        public long KisiID { get; set; }

        public string Bolge { get; set; }
        public string Sehir { get; set; }
        public string Ilce { get; set; }
        public string MahalleKodu { get; set; }
        public string Mahalle { get; set; }
        public string haneBilgileri { get; set; }
        public string HaneKODU { get; set; }
        public string Adres { get; set; }
        public string AdSoyad { get; set; }
        public string TCNo { get; set; }
    }
}