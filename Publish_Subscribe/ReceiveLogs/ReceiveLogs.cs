using System;
using System.Linq;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ReceiveLogs
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@localhost:5672/")};

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // channel.QueueDeclare(queue: "emit_log_queue",
            //                      durable: true,
            //                      exclusive: false,
            //                      autoDelete: false,
            //                      arguments: null);

            channel.ExchangeDeclare("logs", ExchangeType.Fanout);

            var queueName = channel.QueueDeclare().QueueName;


            channel.QueueBind(queue: queueName,
                              exchange: "logs",
                              routingKey: string.Empty);

            // Fair Dispatch
            // This configuration tells to RabbitMQ don't dispatch a new message 
            // to a worker until it has processed and acknowledged the previous one.
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            Console.WriteLine("[*] Waiting for messages!");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) => 
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Have one Task: {message}");
                int points = message.Split(" ").Length -1;
                Thread.Sleep(points * 100);

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine("Press any button to exit.");
            Console.ReadLine();
        }
    }
}
