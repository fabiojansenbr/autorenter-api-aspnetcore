﻿using AutoRenter.Api.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoRenter.Api.Controllers
{
    public abstract class ControllerBase : Controller
    {
        internal IActionResult ProcessResultCode(ResultCode resultCode)
        {
            switch (resultCode)
            {
                case ResultCode.NotFound:
                    return NotFound();
                case ResultCode.BadRequest:
                    return BadRequest();
                case ResultCode.Unknown:
                case ResultCode.Failed:
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
