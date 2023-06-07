﻿using FluentValidation;
using VinEcomViewModel.Product;

namespace VinEcomInterface.IValidator
{
    public interface IProductValidator
    {
         IValidator<ProductCreateModel> ProductCreateValidator { get; }
    }
}
