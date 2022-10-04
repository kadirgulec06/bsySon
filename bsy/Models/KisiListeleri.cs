using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.Models
{
    public class KisiListeleri
    {
        public IEnumerable<SelectListItem> MedeniDurum { get; set; }
        public IEnumerable<SelectListItem> EgitimDurumu { get; set; }
        public IEnumerable<SelectListItem> SosyalGuvence { get; set; }
        public IEnumerable<SelectListItem> SaglikSorunu { get; set; }
        public IEnumerable<SelectListItem> CalismaDurumu { get; set; }
        public IEnumerable<SelectListItem> Meslek { get; set; }
        public IEnumerable<SelectListItem> OkulDurumu { get; set; }
        public IEnumerable<SelectListItem> SosyalDestek { get; set; }
        public IEnumerable<SelectListItem> AsiDurumu { get; set; }
        public IEnumerable<SelectListItem> OzelDurum { get; set; }
        public IEnumerable<SelectListItem> KronikRahatsizlik { get; set; }
        public IEnumerable<SelectListItem> YonlendirmeDurumu { get; set; }
        public IEnumerable<SelectListItem> OkurYazar { get; set; }
    }
}