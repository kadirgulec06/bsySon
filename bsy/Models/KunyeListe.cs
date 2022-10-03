using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.Models
{
    public class KunyeListe
    {
        public IEnumerable<SelectListItem> bolgeler { get; set; }
        public IEnumerable<SelectListItem> sehirler { get; set; }
        public IEnumerable<SelectListItem> ilceler { get; set; }
        public IEnumerable<SelectListItem> mahalleler { get; set; }

    }
}