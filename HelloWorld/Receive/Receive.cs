using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Receive
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@localhost:5672/")};

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "hello",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            Console.WriteLine("[*] Waiting for messages!");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) => 
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Have one message: {message}");
            };

            channel.BasicConsume(queue: "hello",
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine("Press any button to exit.");
            Console.ReadLine();
        }
    }
}
