using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace ProjetoIs.Services
{
    [Flags]
    public enum SubscriptionEvent
    {
        Creation = 1,
        Deletion = 2
    }

    public class NotificationService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private static readonly object _mqttLock = new object();
        private static MqttClient _mqttClient;
        private static string _mqttHost;
        private static int _mqttPort;

        public void NotifySubscriptions(string applicationName, string containerName, string contentInstanceName, SubscriptionEvent evt, SqlConnection existingConn)
        {
            try
            {
                int evtMask = (int)evt;

                string query = @"
                   SELECT s.[resource-name], s.[endpoint]
                     FROM subscription s
                     JOIN container c
                       ON c.[resource-name] = s.[container-resource-name]
                    WHERE c.[application-resource-name] = @app
                      AND s.[container-resource-name] = @cont
                      AND (s.[evt] & @evtMask) = @evtMask";

                var subs = new List<(string Name, string Endpoint)>();

                using (var cmd = new SqlCommand(query, existingConn))
                {
                    cmd.Parameters.AddWithValue("@app", applicationName);
                    cmd.Parameters.AddWithValue("@cont", containerName);
                    cmd.Parameters.AddWithValue("@evtMask", evtMask);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            subs.Add((
                                reader["resource-name"].ToString(),
                                reader["endpoint"].ToString().Trim()
                            ));
                        }
                    }
                }

                if (subs.Count == 0)
                    return;

                string resourcePath = BuildResourcePath(applicationName, containerName, contentInstanceName);

                foreach (var sub in subs)
                {
                    var payload = new
                    {
                        eventType = evt == SubscriptionEvent.Creation ? "creation" : "deletion",
                        resourceType = "content-instance",
                        resourcePath,
                        subscription = sub.Name,
                        timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")
                    };

                    if (sub.Endpoint.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        SendHttpNotification(sub.Endpoint, payload);
                    }
                    else
                    {
                        string topic = $"api/somiod/{applicationName}/{containerName}";
                        SendMqttNotification(sub.Endpoint, topic, payload);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[NotifySubscriptions] " + ex.Message);
            }
        }

        private string BuildResourcePath(string app, string cont, string ci)
        {
            return $"/api/somiod/{app}/{cont}/{ci}";
        }

        private void SendHttpNotification(string endpoint, object payload)
        {
            try
            {
                string json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = _httpClient.PostAsync(endpoint, content).GetAwaiter().GetResult();

                System.Diagnostics.Debug.WriteLine($"[HTTP] {endpoint} -> {(int)response.StatusCode} {response.ReasonPhrase}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[HTTP] " + ex.Message);
            }
        }

        private void SendMqttNotification(string endpoint, string topic, object payload)
        {
            try
            {
                ParseMqttEndpoint(endpoint, out string host, out int port);

                if (string.IsNullOrWhiteSpace(host))
                {
                    System.Diagnostics.Debug.WriteLine("[MQTT] Invalid endpoint (empty host)");
                    return;
                }

                EnsureMqttConnected(host, port);

                string json = JsonConvert.SerializeObject(payload);
                byte[] msg = Encoding.UTF8.GetBytes(json);

                _mqttClient.Publish(topic, msg, MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);

                System.Diagnostics.Debug.WriteLine($"[MQTT] Published to {host}:{port} | topic={topic}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[MQTT] ERROR: " + ex.Message);
                ResetMqttClient();
            }
        }


        private static void EnsureMqttConnected(string host, int port)
        {
            lock (_mqttLock)
            {
                if (_mqttClient != null && _mqttClient.IsConnected && _mqttHost == host && _mqttPort == port)
                    return;

                ResetMqttClient_NoLock();

                _mqttHost = host;
                _mqttPort = port;

                _mqttClient = new MqttClient(host, port, false, null, null, MqttSslProtocols.None);

                _mqttClient.Connect("somiod-" + Guid.NewGuid().ToString("N"));
            }
        }

        private static void ResetMqttClient()
        {
            lock (_mqttLock)
            {
                ResetMqttClient_NoLock();
            }
        }

        private static void ResetMqttClient_NoLock()
        {
            try
            {
                if (_mqttClient != null && _mqttClient.IsConnected)
                    _mqttClient.Disconnect();
            }
            catch { }
            finally
            {
                _mqttClient = null;
                _mqttHost = null;
                _mqttPort = 0;
            }
        }

        private static void ParseMqttEndpoint(string endpoint, out string host, out int port)
        {
            host = (endpoint ?? "").Trim();
            port = 1883;

            if (host.Contains("://"))
                host = host.Substring(host.IndexOf("://") + 3);

            int slash = host.IndexOf('/');
            if (slash >= 0)
                host = host.Substring(0, slash);

            if (host.Contains(":"))
            {
                var parts = host.Split(':');
                host = parts[0];
                if (int.TryParse(parts[1], out int p))
                    port = p;
            }
        }
    }
}
