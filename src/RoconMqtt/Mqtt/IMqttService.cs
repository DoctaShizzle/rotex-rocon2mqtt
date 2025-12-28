using System;
using System.Collections.Generic;
using System.Text;

namespace RoconMqtt.Mqtt;

public interface IMqttService
{
    Task ConnectAsync();
    Task PublishAsync(string topic, string payload, CancellationToken cancellationToken = default);
    bool IsConnected { get; }
}
