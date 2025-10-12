namespace BusinessLogicLayer.RabbitMQ;

public interface IRabbitMQPublisher
{
    void Publisher<T>(string? routingKey, T message);
}
