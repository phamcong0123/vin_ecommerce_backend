﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcomInterface;
using VinEcomInterface.IService;
using VinEcomRepository;
using VinEcomUtility.UtilityMethod;
using VinEcomViewModel.Global;

namespace VinEcomService.Service
{
    public class BaseService : IBaseService
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IConfiguration config;
        protected readonly ITimeService timeService;
        protected readonly ICacheService cacheService;
        public BaseService(IUnitOfWork unitOfWork, IConfiguration config, ITimeService timeService, ICacheService cacheService)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
            this.timeService = timeService;
            this.cacheService = cacheService;
        }
    }
}