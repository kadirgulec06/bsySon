using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class HANE
    {
        public HANE()
        {
            id = 0;
            HaneKodu = "";
            MahalleID = 0;
            KayitTarihi = DateTime.Now.Date;
            Cadde = "";
            Sokak = "";
            Apartman = "";
            Daire = "";
            OdaSayisi = 0;
            BrutAlan = 0;
            EkBilgi = "";
            eposta = "";
            Telefon = "";
            HaneTipi = 0;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long id { get; set; }

        [MaxLength(50)]
        public string HaneKodu { get; set; }
        public long MahalleID { get; set; }
        public DateTime KayitTarihi { get; set; }

        [MaxLength(20)]
        public string Telefon { get; set; }

        [MaxLength(200)]
        public string eposta { get; set; }

        [MaxLength(200)]
        public string Cadde { get; set; }

        [MaxLength(200)]
        public string Sokak { get; set; }

        [MaxLength(50)]
        public string Apartman { get; set; }

        [MaxLength(20)]
        public string Daire { get; set; }
        public short OdaSayisi { get; set; }
        public short BrutAlan { get; set; }

        [MaxLength(1000)]
        public string EkBilgi { get; set; }
        public short HaneTipi { get; set; }

    }
}