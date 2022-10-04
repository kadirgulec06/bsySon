using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.Models
{
    public class HaneListeleri
    {
        public IEnumerable<SelectListItem> Ihtiyaclar { get; set; }
        public IEnumerable<SelectListItem> BelediyeYardimi { get; set; }
        public IEnumerable<SelectListItem> EvTuru { get; set; }
        public IEnumerable<SelectListItem> EvMulkiyeti { get; set; }
        public IEnumerable<SelectListItem> HaneGelirDilimi { get; set; }
    }
}