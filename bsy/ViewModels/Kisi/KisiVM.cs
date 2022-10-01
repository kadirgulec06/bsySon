using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.ViewModels.Kisi
{
    public class KisiVM
    {
        public Kunye kunye { get; set; }
        public KISI kisi { get; set; }
        public KISIHANE kisiHane { get; set; }
        public IEnumerable<SelectListItem> cinsiyetler { get; set; }

    }
}