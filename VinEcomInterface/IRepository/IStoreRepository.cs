﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcomDomain.Model;
using VinEcomUtility.Pagination;
using VinEcomViewModel.Store;

namespace VinEcomInterface.IRepository
{
    public interface IStoreRepository : IBaseRepository<Store>
    {
        Task<Pagination<Store>> FilterStoreAsync(StoreFilterViewModel vm);
    }
}
