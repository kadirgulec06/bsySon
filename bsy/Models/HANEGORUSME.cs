using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public short IkametYeri { get; set; }
        public long GocSehri { get; set; }
        public long GocIlcesi { get; set; }
        public short GocSebebi { get; set; }
        public short GocYili { get; set; }
        public string EvdeYasayanlar { get; set; }
        public short KonusulanDil { get; set; }
        public short KronikEngelliSayisi { get; set; }
        public int OrtalamaGelir { get; set; }
        public int EmekliMaasi { get; set; }
        public int CalisanGeliri { get; set; }
        public short CalisanSayisi { get; set; }
        public short CalisanCocukSayisi { get; set; }
        public short Calisanlar { get; set; }
        public short SosyalDestekTuru { get; set; }
        public short KonutMulkiyetTuru { get; set; }
        public int KiraTutari { get; set; }
        public short ElektrikErisimi { get; set; }
        public short TemizSu { get; set; }
        public short SehirSuyu { get; set; }
        public short Kanalizasyon { get; set; }
        public short Buzdolabi { get; set; }
        public short CamasirMakinesi { get; set; }
        public short BulasikMakinesi { get; set; }
        public short Televizyon { get; set; }
        public short Internet { get; set; }
        public short BilgisayarTablet { get; set; }
        public short Mobilya { get; set; }
        public short Firin { get; set; }
        public int GidaMasraflari { get; set; }
        public int Faturalar { get; set; }
        public int Taksitler { get; set; }
        public int BebekGideri { get; set; }
        public int IsinmaGideri { get; set; }
        public int DigerGiderler { get; set; }
        public short IsinmaTuru { get; set; }
        public short BeslenmeDurumu { get; set; }
        public short BeslenmeIstekleri { get; set; }
        public short CocukSutu { get; set; }
        public short OgunAtlama { get; set; }
        public short OkulYemegi { get; set; }
        public short OkulBeslenmesi { get; set; }
        public short OkulBeslenmeIstekleri { get; set; }
        public short AlisVerisYeri { get; set; }
        public short VeresiyeAlisVeris { get; set; }
        public short BezMama { get; set; }
        public short OzelIhtiyaclar { get; set; }
        public short KadininMalVarligi { get; set; }
        public short CocuklarEgitimBitirme { get; set; }
        public short EgitimEngelleri { get; set; }
        public short EgitimEngeliCozumleri { get; set; }
        public short SorunDestegi { get; set; }
        public short CevredeUniversiteli { get; set; }
        public short CocukVakitGecirme { get; set; }
        public short AileVakitGecirme { get; set; }
        public short DestekAlmaPaylasma { get; set; }
        public short CevreGuvenlimi { get; set; }
        public short EvdeYasanilanSure { get; set; }
        public short FaturaDurumu { get; set; }
        public short KisiDestegi { get; set; }
        public short KurumDestegi { get; set; }
        public short YonlendirmeDurumu { get; set; }
        public int BorcBakkal { get; set; }
        public int BorcElektrik { get; set; }
        public int BorcSu { get; set; }
        public int BorcGaz { get; set; }
        public int BorcInternet { get; set; }
        public int BorcTelefon { get; set; }
        public int BorcKira { get; set; }
        public int BorcKredi { get; set; }
        public int BorcDiger { get; set; }
        public short OzelDurum { get; set; }
        public short CocukSayisi { get; set; }
        public short YetiskinSayisi { get; set; }
        public bool EngelliVar { get; set; }
        public bool AltmisBesUstuVar { get; set; }
        public bool KronikVar { get; set; }
        public bool ElliAltiOlumVar { get; set; }
        public bool CocukOlumuVar { get; set; }
        public bool BosBit6 { get; set; }
        public bool BosBit7 { get; set; }
        public bool BosBit8 { get; set; }

        [MaxLength(2000)]
        public string EkBilgi { get; set; }

        [MaxLength(400)]
        public string Aciklama { get; set; }

    }
}