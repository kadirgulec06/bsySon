using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class SehirIlceMahalle
    {
        public List<long> sehirler { get; set; }
        public List<long> ilceler { get; set; }
        public List<long> mahalleler { get; set; }
    }
}