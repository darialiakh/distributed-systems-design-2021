﻿using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Logic.DTO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace Logic.QueueListener
{
    public class QueueListener : BackgroundService
    {
        private readonly ILogger<QueueListener> _logger;
        private IModel _channel;
        private IConnection _connection;
        private readonly string _hostname;
        private readonly string _queueName;
        private readonly string _username;
        private readonly string _password;
        private readonly int _port;

        private readonly IMemoryCache _cache;

        Dictionary<Guid, string> messages = new Dictionary<Guid, string>();

        public QueueListener(ILogger<QueueListener> logger, IOptions<RabbitMqConfiguration> rabbitMqOptions, IMemoryCache cache)
        {
            _hostname = rabbitMqOptions.Value.HostName;
            _queueName = rabbitMqOptions.Value.QueueName;
            _username = rabbitMqOptions.Value.UserName;
            _password = rabbitMqOptions.Value.Password;
            _port = rabbitMqOptions.Value.Port;
            _logger = logger;
            _cache = cache;

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
            _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                Thread.Sleep(6000);

                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonConvert.DeserializeObject<MessageDto>(content);

                _logger.LogInformation($"Received message '{message.Message}' with id '{message.Id.ToString()}'");

                if (_cache.TryGetValue("messages", out messages))
                {
                    messages.Add(message.Id, message.Message);

                    _cache.Set("messages", messages);
                }
                else
                {
                    var messages = new Dictionary<Guid, string>
                {
                    { message.Id, message.Message }
                };

                    _cache.Set("messages", messages);
                }

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