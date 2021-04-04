﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MessagesService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(ILogger<MessagesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            return "not implemented yet";
        }
    }
}