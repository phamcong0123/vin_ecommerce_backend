﻿using VinEcomDomain.Enum;

#nullable disable warnings
namespace VinEcomViewModel.Store
{
    public class StoreViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public StoreCategory Category { get; set; }
        public double CommissionPercent { get; set; }
        public decimal Balance { get; set; }
        public bool IsWorking { get; set; }
        //
        public int BuildingId { get; set; }
        public string BuidingName { get; set; }
    }
}