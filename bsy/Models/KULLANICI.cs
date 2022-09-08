using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class KULLANICI
    {
        public long id { get; set; }

        [MaxLength(200)]
        public string eposta { get; set; }

        [MaxLength(80)]
        public string Ad { get; set; }

        [MaxLength(80)]
        public String Soyad { get; set; }

        [MaxLength(20)]
        public string Telefon { get; set; }

        public long KimlikNo { get; set; }

        [MaxLength(100)]
        public string Sifre { get; set; }
        public DateTime KayitTarihi { get; set; }

        [MaxLength(1)]
        public string Durum { get; set; }
        public DateTime DurumTarihi { get; set; }
    }
}