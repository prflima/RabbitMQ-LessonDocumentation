using System;
using System.Text;
using RabbitMQ.Client;

namespace Send
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

            const string message = "Hello World, I am Paulo Ricardo!";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: "hello",
                                 basicProperties: null,
                                 body: body);
            
            Console.WriteLine($"Sent - {message}");

            Console.WriteLine(" Press any button to exit.");
            Console.ReadLine();
        }
    }
}
