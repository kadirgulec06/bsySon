using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.ViewModels.Mahalle
{
    public class MahalleVM
    {
        public SOZLUK sozluk { get; set; }
        public MAHALLE mahalle { get; set; }
        public IEnumerable<SelectListItem> sehirler { get; set; }
        public string sehirADI { get; set; }
        public long sehirID { get; set; }
        public IEnumerable<SelectListItem> ilceler { get; set; }
        public string ilceADI { get; set; }

    }
}