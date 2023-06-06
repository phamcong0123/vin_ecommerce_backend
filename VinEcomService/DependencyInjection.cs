﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VinEcomDbContext;
using VinEcomDomain.Resources;
using VinEcomInterface;
using VinEcomInterface.IRepository;
using VinEcomInterface.IService;
using VinEcomRepository;
using VinEcomRepository.Repository;
using VinEcomService.Service;

namespace VinEcomService
{
    public static class DependencyInjection
    {
        public static void InjectInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            //services.AddDbContext<AppDbContext>(options =>
            //{
            //    options.UseNpgsql(config.GetConnectionString("VinEcomLocal"));
            //});
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(config.GetConnectionString("VinEcomCloud"));
            });
            //
            string resourcePath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(VinEcom)).Location);
            services.AddLocalization(options => options.ResourcesPath = resourcePath);
            //
            services.AddSingleton<ICacheService, RedisCacheService>();
            services.AddSingleton<ITimeService, TimeService>();
            services.AddSingleton<ICacheService, RedisCacheService>();
            //
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IStoreStaffRepository, StoreStaffRepository>();
            services.AddScoped<IShipperRepository, ShipperRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderDetailRepository,  OrderDetailRepository>();
            services.AddScoped<IBuildingRepository, BuildingRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            //
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IShipperService, ShipperService>();
            services.AddScoped<IStoreStaffService, StoreStaffService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IBaseService, BaseService>();
        }
    }
}
