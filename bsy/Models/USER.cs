using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class User
    {
        public long id { get; set; }
        public string UserName { get; set; }
        public string KimlikNo { get; set; }
        public string AdSoyad { get; set; }
        public string Eposta { get; set; }
        public string menuRolleri { get; set; }
        public string Roller { get; set; }
    }
}