using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace BusinessLogicLayer.RabbitMQ;

public class RabbitMQPublisher : IRabbitMQPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IConfiguration _configuration;
    public RabbitMQPublisher(IConfiguration configuration)
    {
        _configuration = configuration;

        string hostName = _configuration["RABBITMQ_HOST"]!;
        string port = _configuration["RABBITMQ_PORT"]!;
        string userName = _configuration["RABBITMQ_USER_NAME"]!;
        string password = _configuration["RABBITMQ_PASSWORD"]!;

        ConnectionFactory connectionFactory = new ConnectionFactory()
        {
            HostName = hostName,
            Port = Convert.ToInt32(port),
            UserName = userName,
            Password = password
        };

        _connection = connectionFactory.CreateConnection();

        _channel = _connection.CreateModel();
    }
    public void Publisher<T>(Dictionary<string, object> headers, T message)
    {
        // message convert into json 
        string messageJson = JsonSerializer.Serialize(message);

        // the create byte into this json
        byte[] messageBodyInBytes = Encoding.UTF8.GetBytes(messageJson);

        // Create exchange name and exchange
        string exchangeName = _configuration["RabbitMQ_Products_Exchange"]!;

        _channel.ExchangeDeclare(exchange: exchangeName, durable: true, type: ExchangeType.Headers);

        // Publish the message
        var basicProperties = _channel.CreateBasicProperties();
        basicProperties.Headers = headers;

        _channel.BasicPublish(exchange: exchangeName, routingKey: string.Empty, body: messageBodyInBytes, basicProperties: basicProperties);
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }

}
