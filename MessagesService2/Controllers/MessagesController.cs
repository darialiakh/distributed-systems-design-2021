using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace MessagesService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly ILogger<MessagesController> _logger;
        private readonly IMemoryCache _cache;

        Dictionary<Guid, string> messages = new Dictionary<Guid, string>();

        public MessagesController(ILogger<MessagesController> logger, IMemoryCache memoryCache)
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

            _logger.LogInformation($"MessagesService2 return messages: {data}");

            return data;
        }
    }
}