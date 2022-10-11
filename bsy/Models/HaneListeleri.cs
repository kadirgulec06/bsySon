using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.Models
{
    public class HaneListeleri
    {
        public IEnumerable<SelectListItem> HaneTipi { get; set; }
    }
}