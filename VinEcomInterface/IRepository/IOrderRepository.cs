﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcomDomain.Enum;
using VinEcomDomain.Model;

namespace VinEcomInterface.IRepository
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<IEnumerable<Order>?> GetOrderAtStateWithDetailsAsync(OrderStatus status, int? customerId);
    }
}
