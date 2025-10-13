namespace BusinessLogicLayer.RabbitMQ;

public interface IRabbitMQPublisher
{
    void Publisher<T>(Dictionary<string, object> headers, T message);
}
