using System;
using System.Text;
using RabbitMQ.Client;

namespace EmitLogDirect
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@localhost:5672/") };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare("direct_logs", ExchangeType.Direct);
            var gravidade = (args.Length > 0) ? args[0] : "informações";

            string message = GetMessage(args);
            var body = Encoding.UTF8.GetBytes(message);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "direct_logs",
                                 routingKey: gravidade,
                                 basicProperties: null,
                                 body: body);

            Console.WriteLine("Message sent. Press any button to exit.");
            Console.ReadLine();
        }

        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hello world!");
        }
    }
}

