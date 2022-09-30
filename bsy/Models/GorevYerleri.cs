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
            mahalleler = new List<long>();
        }
        public bool butunTurkiye { get; set; }
        public List<long> mahalleler { get; set; }
    }
}