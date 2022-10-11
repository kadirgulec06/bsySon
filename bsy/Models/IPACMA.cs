using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class IPACMA
    {
        public int id { get; set; }

        [MaxLength(50)]
        public string ip { get; set; }
        public DateTime Tarih { get; set; }

        [MaxLength(400)]
        public string Aciklama { get; set; }
    }
}