using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class KISIGORUSME
    {
        public long id { get; set; }
        public long KisiID { get; set; }
        public DateTime GorusmeTarihi { get; set; }
        public short Boy { get; set; }
        public short Kilo { get; set; }
        public short MedeniDurumu { get; set; }
        public short OkurYazar { get; set; }
        public short EgitimDurumu { get; set; }
        public short EgitimSon { get; set; }
        public short OkulDurumu { get; set; }
        public short OkulaDevam { get; set; }
        public short SinifTekrari { get; set; }
        public short CocukOdasi { get; set; }
        public short OdevYardimi { get; set; }
        public short Sinif { get; set; }
        public short OkulIstegi { get; set; }
        public short OkulServisi { get; set; }
        public short DersDestegiIhtiyaci { get; set; }
        public short CalismaDurumu { get; set; }
        public short Meslek { get; set; }
        public short Is { get; set; }
        public short CalismaIstegi { get; set; }

        [MaxLength(2000)]
        public string IsGunuHikayesi { get; set; }
        public short SosyalGuvence { get; set; }
        public short SaglikSigortasi { get; set; }
        public short SaglikDurumu { get; set; }
        public short KronikDurumu { get; set; }
        public short KronikYuzdesi { get; set; }
        public short DuzenliIlacIhtiyaci { get; set; }
        public short IlacTeminDurumu { get; set; }
        public short PsikolojikDestekIhtiyaci { get; set; }
        public short PsikolojikDestekCozumleri { get; set; }
        public short SagliktaGittigiYerler { get; set; }
        public short SaglikYeriSorunlari { get; set; }
        public short DogumKontroluIhtiyaci { get; set; }
        public short DogumKontroluIstegi { get; set; }
        public short HamilelikKontrolleri { get; set; }
        public short AsiDurumu { get; set; }

        [MaxLength(2000)]
        public string EkBilgi { get; set; }

        [MaxLength(400)]
        public string Aciklama { get; set; }

    }
}