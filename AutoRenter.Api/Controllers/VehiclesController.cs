using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using AutoRenter.Api.Domain;
using AutoRenter.Api.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoRenter.Api.DomainInterfaces;

namespace AutoRenter.Api.Controllers
{
    [Route("api/vehicles")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService vehicleService;
        public VehiclesController(IVehicleService vehicleService)
        {
            this.vehicleService = vehicleService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            var result = vehicleService.GetAll();
            if (result.ResultCode == ResultCode.Success)
            {
                var formattedResult = new Dictionary<string, object>
                {
                    { "vehicles", result.Data }
                };
                Response.Headers.Add("x-total-count", result.Data.ToString());
                return Ok(formattedResult);
            }

            return ProcessResultCode(result.ResultCode);
        }

        [HttpGet("{id:Guid}", Name = "GetVehicle")]
        [AllowAnonymous]
        public async Task<IActionResult> Get([Required] Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                return BadRequest(id);
            }

            var result = await vehicleService.Get(id);
            if (result.ResultCode == ResultCode.Success)
            {
                var formattedResult = new Dictionary<string, object>();
                formattedResult.Add("vehicle", result.Data);
                return Ok(formattedResult);
            }

            return ProcessResultCode(result.ResultCode);
        }

        [HttpGet("{name}")]
        [AllowAnonymous]
        public IActionResult GetByName([Required] string name)
        {
            Response.Headers.Add("x-status-reason",
                $"The value '{name}' is not recognize as a valid guid to uniquely identify a resource.");
            return BadRequest();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] Vehicle vehicle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await vehicleService.Insert(vehicle);
            if (result.ResultCode == ResultCode.Success)
            {
                return CreatedAtRoute("GetVehicle", new { id = result.Data }, result.Data);
            }

            return ProcessResultCode(result.ResultCode);
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(id);
            }

            var resultCode = await vehicleService.Delete(id);
            if (resultCode == ResultCode.Success)
            {
                return NoContent();
            }

            return ProcessResultCode(resultCode);
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Put(Guid id, [FromBody] Vehicle vehicle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await vehicleService.Update(vehicle);
            if (result.ResultCode == ResultCode.Success)
            {
                return Ok(result.Data);
            }

            return ProcessResultCode(result.ResultCode);
        }
    }
}