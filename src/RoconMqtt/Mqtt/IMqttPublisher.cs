using System;
using System.Collections.Generic;
using System.Text;

namespace RoconMqtt.Mqtt;

public interface IMqttPublisher
{
    Task PublishAsync(string topic, string payload, CancellationToken token);
}
