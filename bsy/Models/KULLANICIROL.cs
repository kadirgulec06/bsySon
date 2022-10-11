using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class KULLANICIROL
    {
        public KULLANICIROL()
        {
            id = 0;
            UserID = 0;
            Rolleri = "";
            Tarih = DateTime.Now;
        }
        public int id { get; set; }
        public int UserID { get; set; }

        [MaxLength(400)]
        public string Rolleri { get; set; }
        public DateTime Tarih { get; set; }

    }
}