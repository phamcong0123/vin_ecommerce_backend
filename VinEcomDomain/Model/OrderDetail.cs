﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcomDomain.Model
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }
        public string? Comment { get; set; }
        public int? Rate { get; set; }
        //
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
