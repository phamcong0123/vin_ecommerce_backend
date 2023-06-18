﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VinEcomDomain.Resources;
using VinEcomInterface.IService;
using VinEcomViewModel.Store;

namespace VinEcomAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService storeService;
        public StoreController(IStoreService storeService)
        {
            this.storeService = storeService;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterStoreAsync([FromBody] StoreRegisterViewModel vm)
        {
            var validationResult = await storeService.ValidateStoreRegistrationAsync(vm);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage });
                return BadRequest(errors);
            }
            if (!await storeService.IsBuildingExistedAsync(vm.BuildingId)) return Conflict(new { message = VinEcom.VINECOM_BUILDING_NOT_EXIST });
            if (await storeService.RegisterAsync(vm)) return Ok(new { message = VinEcom.VINECOM_STORE_REGISTER_SUCCESS });
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = VinEcom.VINECOM_SERVER_ERROR });
        }
        [HttpPost("Filter")]
        public async Task<IActionResult> GetStoresByFilterAsync([FromBody] StoreFilterViewModel vm)
        {
            var validateResult = await storeService.ValidateStoreFilterAsync(vm);
            if (!validateResult.IsValid)
            {
                var errors = validateResult.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage });
                return BadRequest(errors);
            }
            var result = await storeService.GetStoreFilterResultAsync(vm);
            return Ok(result);
        }
    }
}
