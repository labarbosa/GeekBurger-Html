using System;
using System.Collections.Generic;

namespace GeekBurger_HTML.Models
{
    public class OrderToPost
    {
        public DateTime RequestDate { get; set; }
        public List<ProductToGetFormat> Products { get; set; }
    }
}