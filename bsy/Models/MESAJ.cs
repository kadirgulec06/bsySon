using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class Mesaj
    {
        public Mesaj(string tur, string mesaj)
        {
            this.Tur = tur.ToLower();
            this.MesajIcerik = mesaj;
        }
        public string Tur { get; set; }
        public string MesajIcerik { get; set; }
    }
}