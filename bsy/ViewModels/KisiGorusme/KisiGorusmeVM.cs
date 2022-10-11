using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bsy.ViewModels.KisiGorusme
{
    public class KisiGorusmeVM
    {
        public KisiGorusmeVM()
        {
            yeniGorusme = 0;
            tabIndex = 0;
            kunye = new Kunye();
            kisiListeleri = new KisiGorusmeListeleri();
            kisiGorusme = new KISIGORUSME();
        }
        public int yeniGorusme { get; set; }
        public int tabIndex { get; set; }
        public Kunye kunye { get; set; }
        public KisiGorusmeListeleri kisiListeleri { get; set; }
        public KISIGORUSME kisiGorusme { get; set; }

    }
}