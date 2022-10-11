using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.Models
{
    public class KisiGorusmeListeleri
    {
        public IEnumerable<SelectListItem> MedeniDurum { get; set; }
        public IEnumerable<SelectListItem> OkurYazar { get; set; }
        public IEnumerable<SelectListItem> EgitimDurumu { get; set; }
        public IEnumerable<SelectListItem> EgitimSon { get; set; }
        public IEnumerable<SelectListItem> OkulDurumu { get; set; }
        public IEnumerable<SelectListItem> OkulaDevam { get; set; }
        public IEnumerable<SelectListItem> SinifTekrari { get; set; }
        public IEnumerable<SelectListItem> CocukOdasi { get; set; }
        public IEnumerable<SelectListItem> OdevYardimi { get; set; }
        public IEnumerable<SelectListItem> OkulIstegi { get; set; }
        public IEnumerable<SelectListItem> OkulServisi { get; set; }
        public IEnumerable<SelectListItem> DersDestegiIhtiyaci { get; set; }
        public IEnumerable<SelectListItem> CalismaDurumu { get; set; }
        public IEnumerable<SelectListItem> Meslek { get; set; }
        public IEnumerable<SelectListItem> Is { get; set; }
        public IEnumerable<SelectListItem> CalismaIstegi { get; set; }
        public IEnumerable<SelectListItem> SosyalGuvence { get; set; }
        public IEnumerable<SelectListItem> SaglikSigortasi { get; set; }
        public IEnumerable<SelectListItem> SaglikDurumu { get; set; }
        public IEnumerable<SelectListItem> KronikDurumu { get; set; }
        public IEnumerable<SelectListItem> DuzenliIlacIhtiyaci { get; set; }
        public IEnumerable<SelectListItem> IlacTeminDurumu { get; set; }
        public IEnumerable<SelectListItem> PsikolojikDestekIhtiyaci { get; set; }
        public IEnumerable<SelectListItem> PsikolojikDestekCozumleri { get; set; }
        public IEnumerable<SelectListItem> SagliktaGittigiYer { get; set; }
        public IEnumerable<SelectListItem> SaglikYeriSorunlari { get; set; }
        public IEnumerable<SelectListItem> DogumKontroluIhtiyaci { get; set; }
        public IEnumerable<SelectListItem> DogumKontroluIstegi { get; set; }
        public IEnumerable<SelectListItem> HamilelikKontrolleri { get; set; }
        public IEnumerable<SelectListItem> AsiDurumu { get; set; }
    }
}