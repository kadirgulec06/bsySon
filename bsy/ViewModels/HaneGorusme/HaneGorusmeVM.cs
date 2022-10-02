using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.ViewModels.HaneGorusme
{
    public class HaneGorusmeVM
    {
        public int yeniGorusme { get; set; }
        public int tabIndex { get; set; }
        public Kunye kunye { get; set; }
        public HANEGORUSME haneGorusme { get; set; }
        public IEnumerable<SelectListItem> Ihtiyaclar { get; set; }
        public IEnumerable<SelectListItem> BelediyeYardimi { get; set; }
        public IEnumerable<SelectListItem> EvTuru { get; set; }
        public IEnumerable<SelectListItem> EvMulkiyeti { get; set; }
        public IEnumerable<SelectListItem> HaneGelirDilimi { get; set; }
    }
}