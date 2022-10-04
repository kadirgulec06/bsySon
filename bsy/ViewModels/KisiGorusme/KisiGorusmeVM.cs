using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bsy.ViewModels.KisiGorusme
{
    public class KisiGorusmeVM
    {
        public int yeniGorusme { get; set; }
        public int tabIndex { get; set; }
        public Kunye kunye { get; set; }
        public KisiListeleri kisiListeleri { get; set; }
        public KISIGORUSME kisiGorusme { get; set; }

    }
}