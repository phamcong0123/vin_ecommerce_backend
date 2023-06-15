﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcomDbContext;
using VinEcomDomain.Model;
using VinEcomInterface.IRepository;
using VinEcomUtility.UtilityMethod;

namespace VinEcomRepository.Repository
{
    public class ShipperRepository : BaseRepository<Shipper>, IShipperRepository
    {
        public ShipperRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<Shipper?> AuthorizeAsync(string phone, string password)
        {
            var shipper = await context.Set<Shipper>()
                                .AsNoTracking()
                                .Include(c => c.User)
                                .FirstOrDefaultAsync(c => c.User.Phone == phone);
            if (shipper == null) return null;
            if (password.IsCorrectHashSource(shipper.User.PasswordHash)) return shipper;
            return null;
        }
    }
}
