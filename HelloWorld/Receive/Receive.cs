using System;
using RabbitMQ.Client;

namespace Receive
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory {HostName = "localhost"};
        }
    }
}
