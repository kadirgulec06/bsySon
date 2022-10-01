using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class SOZLUK
    {
        public SOZLUK()
        {
            id = 0;
            Turu = "";
            Aciklama = "";
            Parametre = "";
            BabaID = 0;
        }
        public long id { get; set; }

        [MaxLength(20)]
        public string Turu { get; set; }

        [MaxLength(400)]
        public string Aciklama { get; set; }

        [MaxLength(200)]
        public string Parametre{ get; set; }
        public long BabaID { get; set; }
    }
}