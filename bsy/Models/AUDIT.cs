using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class AUDIT
    {
        public long id { get; set; }

        [MaxLength(250)]
        public string TableName { get; set; }

        [MaxLength(100)]
        public string UserID { get; set; }
        public string Actions { get; set; }
        public string OldData { get; set; }
        public string NewData { get; set; }
        public long? TableIdValue { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}