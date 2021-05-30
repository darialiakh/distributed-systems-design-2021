﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

using FacadeService.Dto;
using System.Text.Json;

namespace FacadeService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FacadeController : ControllerBase
    {
        private readonly ILogger<FacadeController> _logger;
        private readonly ConsulService _consulService;

        public FacadeController(ILogger<FacadeController> logger, ConsulService consulService)
        {
            _logger = logger;
            _consulService = consulService;
        }

        [HttpGet]
        public string Get()
        {
            /*var loggingData = WebUtilities.Get($"https://localhost:44303/logging");

            var messagesData = WebUtilities.Get($"https://localhost:44348/messages");

            return loggingData + messagesData;*/

            return "no data";
        }

        [HttpPost]
        public string Post([FromBody] string str)
        {
            var msg = new MessageDto()
            {
                Id = Guid.NewGuid(),
                Message = str
            };

            var data = WebUtilities.Post($"https://localhost:44303/logging", JsonSerializer.Serialize(msg));

            return data;
        }
    }
}
