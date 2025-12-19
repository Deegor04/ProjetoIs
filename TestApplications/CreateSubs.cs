using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SomiodPublisher.ConfigFiles;

namespace SomiodPublisher
{
    public partial class CreateSubs : Form
    {

        private readonly RestClient client;

        public CreateSubs()
        {
            InitializeComponent();

            client = new RestClient(Configs.baseURI);

            comboBoxEvent.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxEvent.Items.Clear();
            comboBoxEvent.Items.Add(new ComboItem { Value = 1, Text = "1  --->  Creation" });
            comboBoxEvent.Items.Add(new ComboItem { Value = 2, Text = "2  --->  Deletion" });
            comboBoxEvent.SelectedIndex = -1;

            LoadContainers();
            LoadSubscriptions();
        }


        private void LoadContainers()
        {
            comboBoxSelectContainerSubs.Items.Clear();

            var request = new RestRequest("/api/somiod", Method.Get);
            request.AddHeader("somiod-discovery", "container");

            var response = client.Execute<List<string>>(request);

            if (!response.IsSuccessful || response.Data == null)
                return;

            foreach (var path in response.Data)
            {
                comboBoxSelectContainerSubs.Items.Add(path);
            }

            comboBoxSelectContainerSubs.SelectedIndex = -1;
        }


        private void LoadSubscriptions()
        {
            comboBoxSelectSubscription.Items.Clear();

            var request = new RestRequest("/api/somiod", Method.Get);
            request.AddHeader("somiod-discovery", "subscription");

            var response = client.Execute<List<string>>(request);

            if (!response.IsSuccessful || response.Data == null)
                return;

            foreach (var path in response.Data)
            {
                comboBoxSelectSubscription.Items.Add(path);
            }

            comboBoxSelectSubscription.SelectedIndex = -1;
        }

        private void buttonAddSubscription_Click(object sender, EventArgs e)
        {
            if (comboBoxSelectContainerSubs.SelectedItem == null)
            {
                MessageBox.Show("Select a container");
                return;
            }

            string subName = textBoxSubscriptionName.Text?.Trim();
            if (string.IsNullOrWhiteSpace(subName))
            {
                MessageBox.Show("Subscription name is required");
                return;
            }

            if (!(comboBoxEvent.SelectedItem is ComboItem selectedEvt))
            {
                MessageBox.Show("Select a valid event (1 or 2)");
                return;
            }

            int evt = selectedEvt.Value;

            string endpoint = textBoxEndpoint.Text?.Trim();
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                MessageBox.Show("Endpoint is required");
                return;
            }

            bool isHttp = endpoint.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                       || endpoint.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

            if (!isHttp)
            {
                if (endpoint.Contains("://"))
                {
                    MessageBox.Show("For MQTT endpoint use 'host' or 'host:port' (no mqtt://). Example: 127.0.0.1:1883");
                    return;
                }

                var partsEp = endpoint.Split(':');
                if (partsEp.Length > 2 || string.IsNullOrWhiteSpace(partsEp[0]))
                {
                    MessageBox.Show("Invalid MQTT endpoint. Use 'host' or 'host:port'. Example: 127.0.0.1:1883");
                    return;
                }

                if (partsEp.Length == 2 && !int.TryParse(partsEp[1], out _))
                {
                    MessageBox.Show("Invalid MQTT port. Example: 127.0.0.1:1883");
                    return;
                }
            }

            // Container path vem do discovery: /api/somiod/{app}/{container}
            string containerPath = comboBoxSelectContainerSubs.SelectedItem.ToString();
            if (!TryParseContainerPath(containerPath, out string appName, out string containerName))
            {
                MessageBox.Show("Invalid container path format.");
                return;
            }

            var body = new Dictionary<string, object>
            {
                { "resource-name", subName },
                { "res-type", "subscription" },
                { "evt", evt },
                { "endpoint", endpoint }
            };

            var request = new RestRequest($"/api/somiod/{appName}/{containerName}", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(body);

            var response = client.Execute(request);

            if (!response.IsSuccessful)
            {
                MessageBox.Show($"Error creating subscription: {(int)response.StatusCode} {response.StatusDescription}\n{response.Content}");
                return;
            }

            MessageBox.Show("Subscription '" + subName + "' created successfully");

            // reset
            textBoxSubscriptionName.Clear();
            textBoxEndpoint.Clear();
            comboBoxSelectContainerSubs.SelectedIndex = -1;
            comboBoxSelectContainerSubs.Text = "";

            LoadSubscriptions();
        }


        private void buttonDeleteSubscription_Click(object sender, EventArgs e)
        {
            if (comboBoxSelectSubscription.SelectedItem == null)
            {
                MessageBox.Show("Select a subscription");
                return;
            }

            string path = comboBoxSelectSubscription.SelectedItem.ToString();
            var parts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            // Esperado: api/somiod/{app}/{container}/subs/{sub}
            if (parts.Length < 6 || !parts[0].Equals("api", StringComparison.OrdinalIgnoreCase)
                                 || !parts[1].Equals("somiod", StringComparison.OrdinalIgnoreCase)
                                 || !parts[4].Equals("subs", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Invalid subscription path format.");
                return;
            }

            string appName = parts[2];
            string containerName = parts[3];
            string subName = parts[5];

            var request = new RestRequest(
                $"/api/somiod/{appName}/{containerName}/subs/{subName}",
                Method.Delete
            );

            var response = client.Execute(request);

            if (!response.IsSuccessful)
            {
                MessageBox.Show($"Error deleting subscription: {(int)response.StatusCode} {response.StatusDescription}");
                return;
            }

            MessageBox.Show("Subscription '" + subName + "' deleted successfully");

            comboBoxSelectSubscription.SelectedIndex = -1;
            comboBoxSelectSubscription.Text = "";
            LoadSubscriptions();
        }


        // --------- helpers ---------

        // Esperado: /api/somiod/{app}/{container}
        private bool TryParseContainerPath(string path, out string appName, out string containerName)
        {
            appName = null;
            containerName = null;

            var parts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4) return false;
            if (!parts[0].Equals("api", StringComparison.OrdinalIgnoreCase)) return false;
            if (!parts[1].Equals("somiod", StringComparison.OrdinalIgnoreCase)) return false;

            appName = parts[2];
            containerName = parts[3];
            return true;
        }

        // Esperado: /api/somiod/{app}/{container}/subs/{sub}
        private bool TryParseSubscriptionPath(string path, out string appName, out string containerName, out string subName)
        {
            appName = null;
            containerName = null;
            subName = null;

            var parts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 6) return false;
            if (!parts[0].Equals("api", StringComparison.OrdinalIgnoreCase)) return false;
            if (!parts[1].Equals("somiod", StringComparison.OrdinalIgnoreCase)) return false;
            if (!parts[4].Equals("subs", StringComparison.OrdinalIgnoreCase)) return false;

            appName = parts[2];
            containerName = parts[3];
            subName = parts[5];
            return true;
        }

        private class ComboItem
        {
            public int Value { get; set; }
            public string Text { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }




        private void comboBoxSelectSubscription_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBoxSubscriptionName_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxSelectContainerSubs_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxEvent_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBoxEndpoint_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
