using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class DENEME
    {
        public long id { get; set; }

        [MaxLength(100)]
        public string aciklama { get; set; }
        public int sayiTam { get; set; }
        public decimal sayiNumeric1 { get; set; }
        public DateTime tarih { get; set; }
        public DateTime zaman { get; set; }
        public DateTime tarihTam { get; set; } 
    }
}