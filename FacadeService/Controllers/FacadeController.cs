using Logic.DTO;
using Logic.QueueSender;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FacadeService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FacadeController : ControllerBase
    {
        private readonly ILogger<FacadeController> _logger;
        private readonly IQueueSender _queueSender;

        private readonly List<string> loggingServiceUrls = new List<string>
        {
            $"http://localhost:7007/logging",
            $"http://localhost:7008/logging",
            $"http://localhost:7009/logging"
        };

        private readonly List<string> messageServiceUrls = new List<string>
        {
            $"http://localhost:7001/messages",
            $"http://localhost:7002/messages"
        };

        public FacadeController(ILogger<FacadeController> logger, IQueueSender sender)
        {
            _logger = logger;
            _queueSender = sender;
        }

        [HttpGet]
        public string Get()
        {
            var loggingData = WebUtilities.Get(GetRandomServiceString(loggingServiceUrls));

            var messagesData = WebUtilities.Get(GetRandomServiceString(messageServiceUrls));

            _logger.LogInformation("Logging: " + loggingData + "\nMessage: " + messagesData);

            return "Logging: " + loggingData + "\nMessage: " + messagesData;
        }

        [HttpPost]
        public string Post([FromBody] string str)
        {
            var msg = new MessageDto()
            {
                Id = Guid.NewGuid(),
                Message = str
            };

            _logger.LogInformation($"Send message {msg.Message} with id {msg.Id}");

            var data = WebUtilities.Post(GetRandomServiceString(loggingServiceUrls), JsonConvert.SerializeObject(msg));

            _queueSender.SendMessage(msg);

            return data;
        }

        private string GetRandomServiceString(List<string> urls)
        {
            var random = new Random();
            int index = random.Next(urls.Count);
            return urls[index];
        }
    }
}