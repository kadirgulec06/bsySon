using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class ExcelParams
    {
    }

    public class HucreParam
    {
        public HucreParam(string p, int r, int c, byte i)
        {
            param = p;
            satir = r;
            sutun = c;
            islem = i;
        }

        public string param { get; set; }
        public int satir { get; set; }
        public int sutun { get; set; }
        public byte islem { get; set; }  // 1: Replace 2:Append Before 3: Append After
    }
    public class ExcelParam
    {
        public HucreParam[] prm { get; set; }
    }
}