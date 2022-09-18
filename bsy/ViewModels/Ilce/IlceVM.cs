using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.ViewModels.Ilce
{
    public class IlceVM
    {
        public SOZLUK sozluk { get; set; }
        public ILCE ilce { get; set; }
        public IEnumerable<SelectListItem> sehirler { get; set; }
        public string sehirADI { get; set; }
    }
}