using FacadeService.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace FacadeService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FacadeController : ControllerBase
    {
        private readonly ILogger<FacadeController> _logger;

        private readonly List<string> loggingServiceUrls = new List<string>
        {
            $"https://localhost:44303/logging",
            $"https://localhost:44304/logging",
            $"https://localhost:44305/logging"
        };

        public FacadeController(ILogger<FacadeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            var loggingData = WebUtilities.Get(GetRandomServiceString());

            //var messagesData = WebUtilities.Get($"https://localhost:44348/messages");

            return loggingData;// + messagesData;
        }

        [HttpPost]
        public string Post([FromBody] string str)
        {
            var msg = new MessageDto()
            {
                Id = Guid.NewGuid(),
                Message = str
            };

            var data = WebUtilities.Post(GetRandomServiceString(), JsonSerializer.Serialize(msg));

            return data;
        }

        private string GetRandomServiceString()
        {
            var random = new Random();
            int index = random.Next(loggingServiceUrls.Count);
            return loggingServiceUrls[index];
        }
    }
}