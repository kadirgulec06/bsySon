using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class SIFRESIFIRLA
    {
        public int id { get; set; }
        public int UserID { get; set; }
        public DateTime Tarih { get; set; }

        [MaxLength(400)]
        public string Aciklama { get; set; }
    }
}