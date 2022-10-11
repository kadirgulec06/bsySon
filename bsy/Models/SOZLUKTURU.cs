using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class SOZLUKTURU
    {
        public SOZLUKTURU()
        {
            id = 0;
            Tur = "";
            Aciklama = "";
        }
        public short id { get; set; }

        [MaxLength(50)]
        public string Tur { get; set; }

        [MaxLength(400)]
        public string Aciklama { get; set; }

    }
}