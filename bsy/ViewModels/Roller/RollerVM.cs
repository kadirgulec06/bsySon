using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.ViewModels.Roller
{
    public class RolSatiriVM
    {
        public string Rol { get; set; }
        public byte Secili { get; set; }
    }
    public class RollerVM
    {
        public int UserID { get; set; }
        public string eposta { get; set; }
        public string Ad { get; set; }
        public String Soyad { get; set; }

        public List<RolSatiriVM> Roller { get; set; }
        public long id { get; set; }   // Roller id

    }
}