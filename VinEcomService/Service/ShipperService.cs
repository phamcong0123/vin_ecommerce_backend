﻿using AutoMapper;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcomDomain.Enum;
using VinEcomDomain.Model;
using VinEcomDomain.Resources;
using VinEcomInterface;
using VinEcomInterface.IService;
using VinEcomInterface.IValidator;
using VinEcomUtility.UtilityMethod;
using VinEcomViewModel.Base;
using VinEcomViewModel.Customer;
using VinEcomViewModel.Order;
using VinEcomViewModel.Shipper;

namespace VinEcomService.Service
{
    public class ShipperService : UserService, IShipperService
    {
        public ShipperService(IUnitOfWork unitOfWork,
                              IConfiguration config,
                              ITimeService timeService,
                              ICacheService cacheService,
                              IClaimService claimService,
                              IMapper mapper,
                              IUserValidator validator) : base(unitOfWork, config, timeService, cacheService, claimService, mapper, validator)
        {
        }

        public async Task<AuthorizedViewModel?> AuthorizeAsync(SignInViewModel vm)
        {
            var shipper = await unitOfWork.ShipperRepository.AuthorizeAsync(vm.Phone, vm.Password);
            if (shipper is null) return null;
            string accessToken = shipper.User.GenerateToken(shipper.Id, config, timeService.GetCurrentTime(), 60 * 24 * 30, Role.Shipper);
            return new AuthorizedViewModel
            {
                AccessToken = accessToken,
                Name = shipper.User.Name,
                AvatarUrl = shipper.User.AvatarUrl ?? VinEcom.VINECOM_USER_AVATAR_URL_NOT_FOUND
            };
        }
        public async Task<bool> RegisterAsync(ShipperSignUpViewModel vm)
        {
            var user = mapper.Map<User>(vm);
            user.PasswordHash = vm.Password.BCryptSaltAndHash();
            var shipper = new Shipper
            {
                User = user,
                LicensePlate = vm.LicensePlate,
                VehicleType = vm.VehicleType
            };
            await unitOfWork.ShipperRepository.AddAsync(shipper);
            if (await unitOfWork.SaveChangesAsync()) return true;
            return false;
        }

        public async Task<ValidationResult> ValidateRegistrationAsync(ShipperSignUpViewModel vm)
        {
            return await validator.ShipperCreateValidator.ValidateAsync(vm);
        }

        public async Task<IEnumerable<ShipperViewModel>> GetShippersAvailableAsync()
        {
            var result = await unitOfWork.ShipperRepository.GetAvailableShipperAsync();
            return mapper.Map<IEnumerable<ShipperViewModel>>(result);
        }

        public async Task<bool> ChangeWorkingStatusAsync()
        {
            var shipper = await FindShipperAsync();
            if (shipper == null) return false;
            shipper.Status = shipper.Status == ShipperStatus.Offline ? ShipperStatus.Available : ShipperStatus.Offline;
            unitOfWork.ShipperRepository.Update(shipper);
            return await unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> OrderDeliveredAsync()
        {
            var shipper = await FindShipperAsync();
            if (shipper == null) return false;
            shipper.Status = ShipperStatus.Available;
            unitOfWork.ShipperRepository.Update(shipper);
            // Get current shipper order
            var currentOrder = shipper.Orders.LastOrDefault(x => x.Status == OrderStatus.Shipping);
            // Update order status
            if (currentOrder is not null)
            {
                currentOrder.Status = OrderStatus.Done;
                unitOfWork.OrderRepository.Update(currentOrder);
            }
            //
            return await unitOfWork.SaveChangesAsync();
        }

        private async Task<Shipper?> FindShipperAsync()
        {
            var userId = claimService.GetCurrentUserId();
            return await unitOfWork.ShipperRepository.GetShipperByUserId(userId);
        }

        public async Task<IEnumerable<OrderWithDetailsViewModel>?> GetDeliveredListAsync()
        {
            var shipper = await FindShipperAsync();
            if (shipper is null) return null;
            //
            var orders = await unitOfWork.OrderRepository.GetOrdersByShipperIdAsync(shipper.Id);
            return MapListOrder(orders);
        }

        private IEnumerable<OrderWithDetailsViewModel> MapListOrder(IEnumerable<Order> orders)
        {
            var ordersVM = new List<OrderWithDetailsViewModel>();
            //
            foreach (var item in orders)
            {
                var orderVM = mapper.Map<OrderWithDetailsViewModel>(item);
                var detail = mapper.Map<List<OrderDetailViewModel>>(item.Details);
                orderVM.Details = detail;
                ordersVM.Add(orderVM);
            }
            //
            return ordersVM;
        }

        #region ReceiveOrder
        public async Task<bool> ReceiveOrderAsync(int orderId)
        {
            var shipper = await FindShipperAsync();
            if (shipper is null || shipper.Status == ShipperStatus.Enroute) return false;
            //
            var order = await unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order is null || order.ShipperId.HasValue) return false;
            //
            order.ShipperId = shipper.Id;
            order.Status = OrderStatus.Shipping;
            unitOfWork.OrderRepository.Update(order);
            //
            UpdateShipperStatus(shipper);
            return await unitOfWork.SaveChangesAsync();
        }

        private void UpdateShipperStatus(Shipper shipper)
        {
            shipper.Status = ShipperStatus.Enroute;
            unitOfWork.ShipperRepository.Update(shipper);
        }
        #endregion
    }
}
