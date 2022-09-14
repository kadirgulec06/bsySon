using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class EPOSTAACMA
    {
        public long id { get; set; }

        [MaxLength(200)]
        public string eposta { get; set; }
        public DateTime Tarih { get; set; }

        [MaxLength(400)]
        public string Aciklama { get; set; }
    }
}