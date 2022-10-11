using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.Models
{
    public class HaneGorusmeListeleri
    {
        public IEnumerable<SelectListItem> IkametYeri { get; set; }
        public IEnumerable<SelectListItem> GocSehri { get; set; }
        public IEnumerable<SelectListItem> GocIlcesi { get; set; }
        public IEnumerable<SelectListItem> GocSebebi { get; set; }
        public IEnumerable<SelectListItem> KonusulanDil { get; set; }
        public IEnumerable<SelectListItem> Calisanlar { get; set; }
        public IEnumerable<SelectListItem> SosyalDestekTuru { get; set; }
        public IEnumerable<SelectListItem> KonutMulkiyetTuru { get; set; }
        public IEnumerable<SelectListItem> ElektrikErisimi { get; set; }
        public IEnumerable<SelectListItem> TemizSu { get; set; }
        public IEnumerable<SelectListItem> SehirSuyu { get; set; }
        public IEnumerable<SelectListItem> Kanalizasyon { get; set; }
        public IEnumerable<SelectListItem> BuzDolabi { get; set; }
        public IEnumerable<SelectListItem> CamasirMakinesi { get; set; }
        public IEnumerable<SelectListItem> BulasikMakinesi { get; set; }
        public IEnumerable<SelectListItem> Televizyon { get; set; }
        public IEnumerable<SelectListItem> Internet { get; set; }
        public IEnumerable<SelectListItem> BilgisayarTablet { get; set; }
        public IEnumerable<SelectListItem> Mobilya { get; set; }
        public IEnumerable<SelectListItem> Firin { get; set; }
        public IEnumerable<SelectListItem> IsinmaTuru { get; set; }
        public IEnumerable<SelectListItem> BeslenmeDurumu { get; set; }
        public IEnumerable<SelectListItem> BeslenmeIstekleri { get; set; }
        public IEnumerable<SelectListItem> CocukSutu { get; set; }
        public IEnumerable<SelectListItem> OgunAtlama { get; set; }
        public IEnumerable<SelectListItem> OkulYemegi { get; set; }
        public IEnumerable<SelectListItem> OkulBeslenmesi { get; set; }
        public IEnumerable<SelectListItem> OkulBeslenmeIstekleri { get; set; }
        public IEnumerable<SelectListItem> AlisVerisYeri { get; set; }
        public IEnumerable<SelectListItem> VeresiyeAlisveris { get; set; }
        public IEnumerable<SelectListItem> BezMama { get; set; }
        public IEnumerable<SelectListItem> OzelIhtiyaclar { get; set; }
        public IEnumerable<SelectListItem> KadininMalVarligi { get; set; }
        public IEnumerable<SelectListItem> CocuklarEgitimBitirme { get; set; }
        public IEnumerable<SelectListItem> EgitimEngelleri { get; set; }
        public IEnumerable<SelectListItem> EgitimEngeliCozumleri { get; set; }
        public IEnumerable<SelectListItem> SorunDestegi { get; set; }
        public IEnumerable<SelectListItem> CevredeUniversiteli { get; set; }
        public IEnumerable<SelectListItem> CocukVakitGecirme { get; set; }
        public IEnumerable<SelectListItem> AileVakitGecirme { get; set; }
        public IEnumerable<SelectListItem> DestekAlmaPaylasma { get; set; }
        public IEnumerable<SelectListItem> CevreGuvenlimi { get; set; }
        public IEnumerable<SelectListItem> FaturaDurumu { get; set; }
        public IEnumerable<SelectListItem> KisiDestegi { get; set; }
        public IEnumerable<SelectListItem> KurumDestegi { get; set; }
        public IEnumerable<SelectListItem> YonlendirmeDurumu { get; set; }
        public IEnumerable<SelectListItem> OzelDurum { get; set; }
    }
}