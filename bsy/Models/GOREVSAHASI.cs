using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class GOREVSAHASI
    {
        public GOREVSAHASI()
        {
            id = 0;
            userID = 0;
            BasTar = DateTime.Now;
            BitTar = new DateTime(3000, 1, 1);
            SehirID = 0;
            IlceID = 0;
            MahalleID = 0;
        }
        public long id { get; set; }
        public long userID { get; set; }
        public DateTime BasTar { get; set; }
        public DateTime BitTar { get; set; }
        public long SehirID { get; set; }
        public long IlceID { get; set; }
        public long MahalleID { get; set; }
    }
}