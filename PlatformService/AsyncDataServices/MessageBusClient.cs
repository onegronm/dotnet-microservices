using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PlatformService.Models.DTOs;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _conn;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;

            var factory = new ConnectionFactory() { 
                HostName = _configuration["RabbitMQ:Host"],
                Port = int.Parse(_configuration["RabbitMQ:Port"])
            };

            try
            {
                using(_conn = factory.CreateConnection()) {
                    _channel = _conn.CreateModel();
                    _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                    _conn.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
                    Console.WriteLine("Connected to Message Bus");
                }
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }

        private void SendMessage(string message) {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(
                exchange: "trigger", 
                routingKey: "",
                basicProperties: null,
                body: body);

            Console.WriteLine("Sent message");
        }

        public void Dispose() {
            if(_channel.IsOpen) {
                _channel.Close();
                _conn.Close();
            }
        }

        public void PublishNewPlatform(PlatformPublishedDTO platformPublishedDTO)
        {
            var message = JsonSerializer.Serialize(platformPublishedDTO);

            if(_conn.IsOpen) {
                SendMessage(message);
            }
        }
    }
}