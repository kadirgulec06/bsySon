using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class KISI
    {
        public KISI()
        {
            id = 0;
            TCNo = "";
            KayitTarihi = DateTime.Now.Date;
            Ad = "";
            Soyad = "";
            DogumTarihi = DateTime.Now.Date;
            Cinsiyet = 0;
            Telefon = "";
            eposta = "";
            EkBilgi = "";
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long id { get; set; }

        [StringLength(11)]
        public string TCNo { get; set; }
        public DateTime KayitTarihi { get; set; }

        [MaxLength(80)]
        public string Ad { get; set; }

        [MaxLength(80)]
        public string Soyad { get; set; }
        public DateTime DogumTarihi { get; set; }
        public short Cinsiyet { get; set; }

        [MaxLength(20)]
        public string Telefon { get; set; }

        [MaxLength(200)]
        public string eposta { get; set; }

        [MaxLength(1000)]
        public string EkBilgi { get; set; }

    }
}