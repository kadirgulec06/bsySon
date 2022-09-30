using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class KISIHANE
    {
        public KISIHANE()
        {
            id = 0;
            KisiID = 0;
            HaneID = 0;
            BasTar = DateTime.Now.Date;
            BitTar = new DateTime(3000, 1, 1);
            Aciklama = "";
        }
        public long id { get; set; }
        public long KisiID { get; set; }
        public long HaneID { get; set; }
        public DateTime BasTar { get; set; }
        public DateTime BitTar { get; set; }

        [MaxLength(400)]
        public string Aciklama { get; set; }
    }
}