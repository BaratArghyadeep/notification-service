using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace NotificationService.API.Services
{
    public class RabbitMqConsumer : BackgroundService
    {
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = "host.docker.internal"
            };

            var connection =
                await factory.CreateConnectionAsync();

            var channel =
                await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "order-created",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer =
                new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var body =
                    ea.Body.ToArray();

                var message =
                    Encoding.UTF8.GetString(body);

                Console.WriteLine(
                    $"Order Event Received: {message}");

                await Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(
                queue: "order-created",
                autoAck: true,
                consumer: consumer);

            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }
    }
}
