using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class SOZLUK
    {
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