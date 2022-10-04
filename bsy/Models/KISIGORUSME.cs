using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class KISIGORUSME
    {
        public long id { get; set; }
        public long KisiID { get; set; }
        public DateTime GorusmeTarihi { get; set; }
        public string Aciklama { get; set; }
        public long MedeniDurumu { get; set; }
        public long EgitimDurumu { get; set; }
        public long SosyalGuvencesi { get; set; }
        public long SaglikSorunu { get; set; }
        public long CalismaDurumu { get; set; }
        public long Meslegi { get; set; }
        public long OkulDurumu { get; set; }
        public long SosyalDestek { get; set; }
        public long AsiDurumu { get; set; }
        public long OzelDurum { get; set; }
        public long KronikRahatsizlik { get; set; }
        public short KronikYuzdesi { get; set; }
        public long YonlendirmeDurumu { get; set; }        
        public long BorcBakkal { get; set; }
        public long BorcElektrik { get; set; }
        public long BorcGaz { get; set; }
        public long BorcInternet { get; set; }
        public long BorcTelefon { get; set; }
        public long BorcDiger { get; set; }
        public long OkurYazar { get; set; }
        public string EkBilgi { get; set; }

    }
}