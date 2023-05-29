﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcomDbContext;
using VinEcomDomain.Model;
using VinEcomInterface.IRepository;

namespace VinEcomRepository.Repository
{
    public class StoreStaffRepository : BaseRepository<StoreStaff>, IStoreStaffRepository
    {
        public StoreStaffRepository(AppDbContext context) : base(context)
        {
        }
    }
}