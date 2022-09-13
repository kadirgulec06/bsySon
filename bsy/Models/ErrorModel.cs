using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class ErrorModel
    {
        [MaxLength(36)]
        public string id { get; set; }
        public DateTime time { get; set; }

        [MaxLength(50)]
        public string tckno { get; set; }

        [MaxLength(200)]
        public string message { get; set; }

        [MaxLength(50)]
        public string trace { get; set; }

        [MaxLength(200)]
        public string result { get; set; }

    }
}