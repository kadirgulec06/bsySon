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
        public HaneGorusmeVM()
        {
            yeniGorusme = 0;
            tabIndex = 0;
            kunye = new Kunye();
            haneListeleri = new HaneGorusmeListeleri();
            haneGorusme = new Column();
        }
        public int yeniGorusme { get; set; }
        public int tabIndex { get; set; }
        public Kunye kunye { get; set; }
        public HaneGorusmeListeleri haneListeleri { get; set; }
        public Column haneGorusme { get; set; }
    }
}