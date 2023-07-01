﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcomDomain.Model;
using VinEcomUtility.Pagination;
using VinEcomViewModel.Order;
using VinEcomViewModel.OrderDetail;

namespace VinEcomInterface.IService
{
    public interface IOrderService : IBaseService
    {
        Task<bool> AddToCartAsync(AddToCartViewModel vm);
        Task<bool> RemoveFromCartAsync(int productId);
        Task<Pagination<Order>> GetOrdersAsync(int pageIndex, int pageSize);
        Task<bool> IsProductSameStoreAsync(int productId);
        Task<bool> EmptyCartAsync(int cartId);
        Task<Pagination<Order>?> GetStoreOrderPagesByStatus(int status, int pageIndex, int pageSize);
        Task<Pagination<Order>?> GetCustomerOrderPagesByStatus(int status, int pageIndex, int pageSize);
        Task<Order?> GetCustomerOrdersAsync(int orderId);
        Task<Order?> GetStoreOrderAsync(int orderId);
        Task<bool> CheckoutAsync();
        Task<OrderWithDetailsViewModel?> GetOrderByIdAsync(int id);
    }
}
