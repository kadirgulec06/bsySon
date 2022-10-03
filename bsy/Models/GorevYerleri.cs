using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class GorevYerleri
    {
        public GorevYerleri()
        {
            butunTurkiye = false;
            gy = new List<GorevYeri>();
            sehirler = new List<long>();
            ilceler = new List<long>();
            mahalleler = new List<long>();
        }
        public bool butunTurkiye { get; set; }
        public List<GorevYeri> gy { get; set; }
        public List<long> sehirler { get; set; }
        public List<long> ilceler { get; set; }
        public List<long> mahalleler { get; set; }
    }
}