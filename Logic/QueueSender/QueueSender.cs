using System;
using System.Collections.Generic;
using System.Text;
using Logic.DTO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Logic.QueueSender
{
    public class QueueSender : IQueueSender
    {
        private readonly string _hostname;
        private readonly string _password;
        private readonly string _queueName;
        private readonly string _username;
        private readonly int _port;
        private IConnection _connection;

        public QueueSender(IOptions<RabbitMqConfiguration> rabbitMqOptions)
        {
            _queueName = rabbitMqOptions.Value.SenderQueueName ?? rabbitMqOptions.Value.QueueName;
            _hostname = rabbitMqOptions.Value.HostName;
            _username = rabbitMqOptions.Value.UserName;
            _password = rabbitMqOptions.Value.Password;
            _port = rabbitMqOptions.Value.Port;

            CreateConnection();
        }

        public void SendMessage(MessageDto message)
        {
            if (ConnectionExists())
            {
                using (var channel = _connection.CreateModel())
                {
                    var args = new Dictionary<string, object>(1);
                    //args.Add("x-max-length", 5);               //  task 3
                    //args.Add("x-overflow", "drop-head");        //  task 3
                    //args.Add("x-overflow", "reject-publish");   //  task 3
                    //args.Add("x-message-ttl", 60000);           //  task 5

                    channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: args);

                    var json = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(json);

                    var props = channel.CreateBasicProperties();
                    props.DeliveryMode = 2;

                    channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: props, body: body);

                }
            }
        }

        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password,
                    Port = _port
                };
                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not create connection: {ex.Message}");
            }
        }

        private bool ConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }

            CreateConnection();

            return _connection != null;
        }
    }
}
