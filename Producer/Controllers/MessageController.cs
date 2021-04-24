using System;
using System.Threading;
using Logic.DTO;
using Logic.QueueSender;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Producer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {

        private readonly ILogger<MessageController> _logger;
        private readonly IQueueSender _queueSender;

        public MessageController(ILogger<MessageController> logger, IQueueSender sender)
        {
            _logger = logger;
            _queueSender = sender;
        }

        [HttpGet]
        public string Get()
        {
            for (int i=1; i<=10; i++)
            {
                Thread.Sleep(10000);

                var message = new MessageDto
                {
                    Id = Guid.NewGuid(),
                    Message = "msg" + i
                };

                _logger.LogInformation($"Sent message '{message.Message}' with id '{message.Id.ToString()}'");

                _queueSender.SendMessage(message);
            }

            return "OK";
        }
    }
}
