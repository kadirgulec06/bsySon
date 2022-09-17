using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class LogIn
    {
        public LogIn()
        {
            userName = "kgulec@spk.gov.tr";
            sifre = "123";
        }

        [Display(Name="EPosta Adresi")]
        public string userName { get; set; }

        [Display(Name = "Şifre")]
        public string sifre { get; set; }
        public bool girisAktif { get; set; }
        public string ip { get; set; }
        public string mesaj { get; set; }
    }
}