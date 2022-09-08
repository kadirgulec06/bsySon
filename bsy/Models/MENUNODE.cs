using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class MENUNODE
    {
        public BSYMENUSU bsyMenusu { get; set; }
        public bool leaf { get; set; }
        public byte level { get; set; }
        public List<MENUNODE> children { get; set; } // childrens if exists
        public MENUNODE()
        {
            leaf = true;
            children = new List<MENUNODE>();
        }
    }
}