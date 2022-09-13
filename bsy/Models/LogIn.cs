using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class LogIn
    {
        [Display(Name="EPosta Adresi")]
        public string userName { get; set; }

        [Display(Name = "Şifre")]
        public string sifre { get; set; }
        public bool girisAktif { get; set; }
        public string ip { get; set; }
    }
}