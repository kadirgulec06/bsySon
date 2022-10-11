using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class GIRISDENEME
    {
        public int id { get; set; }

        [MaxLength(200)]
        public string eposta { get; set; }
        public DateTime Tarih { get; set; }

        [MaxLength(50)]
        public string ip { get; set; }
        public bool Durum { get; set; }

    }
}