using System;
using System.Text;
using RabbitMQ.Client;

namespace NewTask
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@localhost:5672/")};

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "task_queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
                       
            string message = GetMessage(args);
            var body = Encoding.UTF8.GetBytes(message);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: "task_queue",
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
