using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class HANE
    {
        public long id { get; set; }

        [MaxLength(20)]
        public string HaneKodu { get; set; }
        public long MahalleID { get; set; }
        public DateTime KayitTarihi { get; set; }

        [MaxLength(200)]
        public string Cadde { get; set; }

        [MaxLength(200)]
        public string Sokak { get; set; }

        [MaxLength(40)]
        public string Apartman { get; set; }

        [MaxLength(20)]
        public string Daire { get; set; }
        public short OdaSayisi { get; set; }
        public short BrutAlan { get; set; }

        [MaxLength(1000)]
        public string EkBilgi { get; set; }

        [MaxLength(200)]
        public string eposta { get; set; }

    }
}