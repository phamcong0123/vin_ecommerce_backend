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
using VinEcomViewModel.StoreStaff;

namespace VinEcomService.Service
{
    public class StoreStaffService : UserService, IStoreStaffService
    {
        public StoreStaffService(IUnitOfWork unitOfWork,
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
            var storeStaff = await unitOfWork.StoreStaffRepository.AuthorizeAsync(vm.Phone, vm.Password);
            if (storeStaff is null) return null;
            string accessToken = storeStaff.User.GenerateToken(storeStaff.Id, config, timeService.GetCurrentTime(), 60 * 24 * 30, Role.Staff, storeStaff.StoreId);
            return new AuthorizedViewModel
            {
                AccessToken = accessToken,
                Name = storeStaff.User.Name,
                AvatarUrl = storeStaff.User.AvatarUrl ?? VinEcom.VINECOM_USER_AVATAR_URL_NOT_FOUND
            };
        }

        public async Task<bool> IsStoreExistedAsync(int storeId)
        {
            return await unitOfWork.StoreRepository.GetByIdAsync(storeId) is not null;
        }

        public async Task<bool> RegisterAsync(StoreStaffSignUpViewModel vm)
        {
            var user = mapper.Map<User>(vm);
            user.PasswordHash = vm.Password.BCryptSaltAndHash();
            var staff = new StoreStaff
            {
                User = user,
                StoreId = vm.StoreId
            };
            await unitOfWork.StoreStaffRepository.AddAsync(staff);
            if (await unitOfWork.SaveChangesAsync()) return true;
            return false;
        }

        public async Task<ValidationResult> ValidateRegistrationAsync(StoreStaffSignUpViewModel vm)
        {
            return await validator.StaffCreateValidator.ValidateAsync(vm);
        }

        #region CancelOrder
        public async Task<bool> CancelOrderAsync(Order order)
        {
            if (order is null || order.Status != OrderStatus.Preparing) return false;
            //
            order.Status = OrderStatus.Cancel;
            unitOfWork.OrderRepository.Update(order);
            return await unitOfWork.SaveChangesAsync();
        }
        #endregion

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            var storeId = claimService.GetStoreId();
            return await unitOfWork.OrderRepository.GetStoreOrderWithDetailAsync(orderId, storeId);
        }
    }
}
