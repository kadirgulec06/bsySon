using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class ANASOZLUK
    {
        public ANASOZLUK()
        {
            id = 0;
            Turu = "";
            Aciklama = "";
            EkBilgi = "";
        }
        public short id { get; set; }

        [MaxLength(50)]
        public string Turu { get; set; }

        [MaxLength(400)]
        public string Aciklama { get; set; }

        [MaxLength(50)]
        public string EkBilgi { get; set; }

    }
}