using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class KunyeID
    {
        public long BolgeID { get; set; }
        public long SehirID { get; set; }
        public long IlceID { get; set; }
        public long MahalleID { get; set; }
        public long HaneID { get; set; }
        public long KisiID { get; set; }

        public string MahalleKodu { get; set; }
        public string HaneKODU { get; set; }
        public string TCNo { get; set; }

    }
}