using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.ViewModels.Sehir
{
    public class SehirVM
    {
        public SOZLUK sozluk { get; set; }
        public SEHIR sehir { get; set; }
        public IEnumerable<SelectListItem> bolgeler { get; set; }
        public string bolgeADI { get; set; }

    }
}