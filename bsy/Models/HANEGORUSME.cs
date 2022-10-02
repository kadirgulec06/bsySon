using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class HANEGORUSME
    {
        public HANEGORUSME()
        {
            GorusmeTarihi = DateTime.Now.Date;
        }
        public long id { get; set; }
        public long HaneID { get; set; }
        public DateTime GorusmeTarihi { get; set; }
        public long Ihtiyaclar { get; set; }
        public long BelediyeYardimi { get; set; }
        public long HaneGelirDilimi { get; set; }
        public long EvTuru { get; set; }
        public long EvMulkiyeti { get; set; }
        public long KiraTutari { get; set; }
        public string Aciklama { get;set; }
        public string EkBilgi { get; set; }
    }
}