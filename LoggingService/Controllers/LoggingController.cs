using LoggingService.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace LoggingService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoggingController : ControllerBase
    {

        private readonly ILogger<LoggingController> _logger;
        private readonly IMemoryCache _cache;

        Dictionary<Guid, string> messages = new Dictionary<Guid, string>();

        public LoggingController(ILogger<LoggingController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _cache = memoryCache;
        }

        [HttpGet]
        public string Get()
        {
            var data = "";

            if (_cache.TryGetValue("messages", out messages))
            {
                foreach (var msg in messages.Values)
                {
                    data += msg + " ";
                }
            }

            return data;
        }

        [HttpPost]
        public string Post([FromBody] MessageDto model)
        {
            if (_cache.TryGetValue("messages", out messages))
            {
                messages.Add(model.Id, model.Message);

                _cache.Set("messages", messages);
            } 
            else
            {
                var messages = new Dictionary<Guid, string>
                {
                    { model.Id, model.Message }
                };

                _cache.Set("messages", messages);
            }

            return "OK";
        }
    }
}
