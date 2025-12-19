using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using SomiodSubscriber.Services;

namespace SomiodSubscriber
{
    public partial class Form1 : Form
    {
        private MqttListener _listener;

        public Form1()
        {
            InitializeComponent();
        }

        private static readonly HttpClient _http = new HttpClient();

        private readonly string _apiBase = "http://localhost:58066";

        private void Form1_Load(object sender, EventArgs e)
        {
            _listener = new MqttListener();
            _listener.OnLog += msg => LogEvent("System", msg);
            _listener.OnMessage += HandleNotification;

            _listener.Start(
                broker: "localhost",
                port: 1883,
                topic: "api/somiod/#"
            );
            LogEvent("System", "Connected and listening");
            lblConnection.Text = "Connected";
        }

        private void Log(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(Log), message);
                return;
            }

            if (message.StartsWith("----- NOTIFICATION -----"))
            {
                txtLog.SelectionStart = txtLog.TextLength;
                txtLog.SelectionFont = new Font(txtLog.Font, FontStyle.Bold);
                txtLog.AppendText(message + Environment.NewLine);
                txtLog.SelectionFont = txtLog.Font;
                return;
            }

            txtLog.AppendText(message + Environment.NewLine);
        }


        private async void HandleNotification(string topic, string payload)
        {
            try
            {
                var notif = JObject.Parse(payload);

                try
                {
                    var saved = NotificationXmlService.SaveJsonNotificationAsValidatedXml(topic, payload, "SomiodSubscriber");
                }
                catch (Exception xex)
                {
                    LogEvent("XML ERROR", xex.Message);
                }

                var eventType = notif["eventType"]?.ToString();
                var resourceType = notif["resourceType"]?.ToString();
                var resourcePath = notif["resourcePath"]?.ToString();

                if (!string.Equals(eventType, "creation", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(resourceType, "content-instance", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(resourcePath))
                {
                    Log("Notification missing resourcePath.");
                    return;
                }

                if (!TryParseResourcePath(resourcePath, out var application, out var container, out var ciName))
                {
                    Log($"Unexpected resourcePath format: {resourcePath}");
                    return;
                }

                var url = ToAbsoluteUrl(resourcePath);

                string ciJson;
                try
                {
                    using (var resp = await _http.GetAsync(url))
                    {
                        var body = await resp.Content.ReadAsStringAsync();
                        if (!resp.IsSuccessStatusCode)
                        {
                            Log($"GET failed ({(int)resp.StatusCode} {resp.ReasonPhrase}) for {url}\n{body}");
                            return;
                        }
                        ciJson = body;
                    }
                }
                catch (Exception ex)
                {
                    Log($"GET exception ({url}): {ex.Message}");
                    return;
                }

                var ci = JObject.Parse(ciJson);

                var contentStr = ci["content"]?.ToString();
                if (string.IsNullOrWhiteSpace(contentStr))
                {
                    Log($"Content-instance has no 'content' field: {resourcePath}");
                    return;
                }

                JObject contentObj;
                try
                {
                    contentObj = JObject.Parse(contentStr);
                }
                catch
                {
                    Log($"'content' is not valid JSON: {contentStr}");
                    return;
                }

                var value = contentObj["value"]?.ToString();
                if (string.IsNullOrWhiteSpace(value))
                {
                    Log("'value' not found inside content.");
                    return;
                }

                LogEvent("New command",$"{application}/{container} → {value} ({ciName})");

                SimulateDeviceAction(application, container, value);
                UpdateLastCommand(application, container, value);
                UpdateDeviceImage(container, value);
            }
            catch (Exception ex)
            {
                Log("Error handling notification: " + ex.Message);
            }
        }

        private void SimulateDeviceAction(string application, string container, string value)
        {
            var c = (container ?? "").Trim().ToLowerInvariant();
            var v = (value ?? "").Trim();

            if (c == "door" || c.StartsWith("door-"))
            {
                Log($"[DEVICE] Door '{container}' -> {v}");
                return;
            }

            if (c == "light" || c == "lamp" || c.StartsWith("light-") || c.StartsWith("lamp-"))
            {
                Log($"[DEVICE] Light '{container}' -> {v}%");
                return;
            }

            if (c == "blind" || c == "blinds" || c.StartsWith("blind-") || c.StartsWith("blinds-") ||
                c == "shutter" || c.StartsWith("shutter-") ||
                c == "curtain" || c.StartsWith("curtain-"))
            {
                Log($"[DEVICE] Blinds '{container}' -> {v}%");
                return;
            }

            Log($"[DEVICE] '{container}' -> {v}");
        }
        private void UpdateDeviceImage(string container, string value)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, string>(UpdateDeviceImage), container, value);
                return;
            }

            var c = (container ?? "").Trim().ToLowerInvariant();
            var v = (value ?? "").Trim().ToLowerInvariant();

            if (c == "door" || c.StartsWith("door"))
            {
                if (v == "open")
                    pictureBoxState.Image = Properties.Resources.door_open;
                else if (v == "close" || v == "closed")
                    pictureBoxState.Image = Properties.Resources.door_close;
                return;
            }

            if (c == "light" || c == "lamp" || c.StartsWith("light") || c.StartsWith("lamp"))
            {
                var level = ParsePercent(v);
                pictureBoxState.Image = GetBrightnessImage(level);
                return;
            }

            if (c == "blind" || c == "blinds" || c.StartsWith("blind") || c.StartsWith("blinds") ||
                c == "shutter" || c.StartsWith("shutter") ||
                c == "curtain" || c.StartsWith("curtain"))
            {
                var pos = ParsePercent(v);
                pictureBoxState.Image = GetBlindImage(pos);
                return;
            }

            pictureBoxState.Image = null;
        }

        private int ParsePercent(string v)
        {
            v = (v ?? "").Replace("%", "").Trim();
            if (!int.TryParse(v, out var n))
                n = 0;

            if (n < 0) n = 0;
            if (n > 100) n = 100;
            return n;
        }

        private Image GetBrightnessImage(int level)
        {
            if (level <= 0) return Properties.Resources.brightness_0;
            if (level <= 25) return Properties.Resources.brightness_25;
            if (level <= 50) return Properties.Resources.brightness_50;
            if (level <= 75) return Properties.Resources.brightness_75;
            return Properties.Resources.brightness_100;
        }

        private Image GetBlindImage(int pos)
        {
            if (pos <= 0) return Properties.Resources.blind_0;
            if (pos <= 33) return Properties.Resources.blind_33;
            if (pos <= 66) return Properties.Resources.blind_66;
            return Properties.Resources.blind_100;
        }

        private void UpdateLastCommand(string application, string container, string value)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, string, string>(UpdateLastCommand), application, container, value);
                return;
            }

            lblLastCommand.Text = $"Last command: {application}/{container} -> {value}";
        }

        private void LogEvent(string title, string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, string>(LogEvent), title, message);
                return;
            }

            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.SelectionFont = new Font(txtLog.Font, FontStyle.Bold);
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {title}\n");
            txtLog.SelectionFont = txtLog.Font;

            txtLog.AppendText($"  {message}\n\n");
            txtLog.ScrollToCaret();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try { _listener?.Stop(); } catch { }
            base.OnFormClosing(e);
            lblConnection.Text = "Disconnected";
        }

        private bool TryParseResourcePath(string resourcePath, out string application, out string container, out string ciName)
        {
            application = null;
            container = null;
            ciName = null;

            if (string.IsNullOrWhiteSpace(resourcePath))
                return false;

            var path = resourcePath.StartsWith("/") ? resourcePath.Substring(1) : resourcePath;

            var parts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 5) return false;
            if (!parts[0].Equals("api", StringComparison.OrdinalIgnoreCase)) return false;
            if (!parts[1].Equals("somiod", StringComparison.OrdinalIgnoreCase)) return false;

            application = parts[2];
            container = parts[3];
            ciName = parts[4];
            return true;
        }

        private string ToAbsoluteUrl(string urlOrPath)
        {
            if (string.IsNullOrWhiteSpace(urlOrPath))
                return urlOrPath;

            if (Uri.IsWellFormedUriString(urlOrPath, UriKind.Absolute))
                return urlOrPath;

            if (urlOrPath.StartsWith("/"))
                return _apiBase.TrimEnd('/') + urlOrPath;

            return _apiBase.TrimEnd('/') + "/" + urlOrPath;
        }
    }
}
