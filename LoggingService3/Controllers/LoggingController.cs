using LoggingService.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LoggingService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoggingController : ControllerBase
    {
        private readonly ILogger<LoggingController> _logger;
        private readonly LoggingService _loggingService;

        public LoggingController(ILogger<LoggingController> logger, LoggingService loggingService)
        {
            _logger = logger;
            _loggingService = loggingService;
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<string> GetAsync()
        {
            await _loggingService.InitializeAsync().ConfigureAwait(false);

            return await _loggingService.GetMessagesAsync().ConfigureAwait(false);
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<string> PostAsync([FromBody] MessageDto model)
        {
            _logger.LogInformation($"Get message {model.Message} with id {model.Id}");

            await _loggingService.InitializeAsync().ConfigureAwait(false);

            return await _loggingService.PutMessageAsync(model).ConfigureAwait(false);
        }
    }
}