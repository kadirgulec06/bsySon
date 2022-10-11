using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.ViewModels
{
    public class SifreDegisVM
    {
        public long UserID { get; set; }
        public string eposta { get; set; }
        public string AdSoyad { get; set; }

        [MaxLength(40)]
        public string eskiSifre { get; set; }

        [MaxLength(40)]
        public string yeniSifre { get; set; }

        [MaxLength(40)]
        public string yeniSifreTekrar { get; set; }

    }
}