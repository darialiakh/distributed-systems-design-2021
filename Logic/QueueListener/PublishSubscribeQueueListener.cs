using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Logic.DTO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Logic.QueueListener
{
    public class PublishSubscribeQueueListener : BackgroundService
    {
        private readonly ILogger<PublishSubscribeQueueListener> _logger;
        private IModel _channel;
        private IConnection _connection;
        private readonly string _hostname;
        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly string _username;
        private readonly string _password;
        private readonly int _port;

        public PublishSubscribeQueueListener(ILogger<PublishSubscribeQueueListener> logger, IOptions<RabbitMqConfiguration> rabbitMqOptions)
        {
            _logger = logger;
            _hostname = rabbitMqOptions.Value.HostName;
            _exchangeName = rabbitMqOptions.Value.ExchangeName;
            _queueName = rabbitMqOptions.Value.QueueName;
            _username = rabbitMqOptions.Value.UserName;
            _password = rabbitMqOptions.Value.Password;
            _port = rabbitMqOptions.Value.Port;

            InitializeRabbitMqListener();
        }

        private void InitializeRabbitMqListener()
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostname,
                UserName = _username,
                Password = _password,
                Port = _port
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Topic);
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: "rabbit.*");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonConvert.DeserializeObject<MessageDto>(content);

                _logger.LogInformation($"Received message '{message.Message}' with id '{message.Id.ToString()}'");

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(_queueName, false, consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}