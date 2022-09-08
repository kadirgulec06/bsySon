using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class BSYMENUSU
    {
        [Key]
        public int menuNo { get; set; }
        public int babaNo { get; set; }
        public int siraNo { get; set; }
        public short menuTuru { get; set; }

        [MaxLength(200)]
        public string aciklama { get; set; }

        [MaxLength(100)]
        public string controller { get; set; }

        [MaxLength(100)]
        public string metod { get; set; }

        [MaxLength(400)]
        public string queryParam { get; set; }

        [MaxLength(600)]
        public string link { get; set; }

        [MaxLength(400)]
        public string roller { get; set; }

    }
}