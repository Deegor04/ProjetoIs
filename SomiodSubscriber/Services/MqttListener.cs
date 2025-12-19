using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace SomiodSubscriber.Services
{
    public class MqttListener
    {
        private MqttClient _client;

        public event Action<string> OnLog;
        public event Action<string, string> OnMessage;

        public void Start(string broker = "localhost", int port = 1883, string topic = "#")
        {
            _client = new MqttClient(broker, port, false, null, null, MqttSslProtocols.None);

            _client.MqttMsgPublishReceived += (s, e) =>
            {
                var payload = Encoding.UTF8.GetString(e.Message ?? Array.Empty<byte>());
                OnMessage?.Invoke(e.Topic, payload);
            };

            var clientId = "somiod-subscriber-" + Guid.NewGuid().ToString("N").Substring(0, 8);
            _client.Connect(clientId);

            _client.Subscribe(
                new[] { topic },
                new[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE }
            );

            OnLog?.Invoke($"Connected to MQTT {broker}:{port}");
            OnLog?.Invoke($"Subscribed to topic: {topic}");
        }

        public void Stop()
        {
            if (_client != null && _client.IsConnected)
            {
                _client.Disconnect();
                OnLog?.Invoke("MQTT disconnected");
            }
        }
    }
}
