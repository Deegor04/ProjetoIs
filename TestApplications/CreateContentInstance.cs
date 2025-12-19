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
using Newtonsoft.Json;


namespace SomiodPublisher
{
    public partial class CreateContentInstance : Form
    {
        private readonly RestClient client;

        public CreateContentInstance()
        {
            InitializeComponent();
            client = new RestClient(Configs.baseURI);
            LoadContainers();
            LoadContentInstances();
        }

        private void LoadContainers()
        {
            comboBoxContainer.Items.Clear();

            var request = new RestRequest("/api/somiod", Method.Get);
            request.AddHeader("somiod-discovery", "container");

            var response = client.Execute<List<string>>(request);

            if (!response.IsSuccessful || response.Data == null)
                return;

            foreach (var path in response.Data)
            {
                comboBoxContainer.Items.Add(path);
            }

            comboBoxContainer.SelectedIndex = -1;
            comboBoxContainer.Text = "";
        }

        private void LoadContentInstances()
        {
            comboBox2DeleteContInst.Items.Clear();

            var request = new RestRequest("/api/somiod", Method.Get);
            request.AddHeader("somiod-discovery", "content-instance");

            var response = client.Execute<List<string>>(request);

            if (!response.IsSuccessful || response.Data == null)
                return;

            foreach (var path in response.Data)
                comboBox2DeleteContInst.Items.Add(path);

            comboBox2DeleteContInst.SelectedIndex = -1;
            comboBox2DeleteContInst.Text = "";
        }


        private void buttonAddContentInstance_Click(object sender, EventArgs e)
        {
            if (comboBoxContainer.SelectedItem == null)
            {
                MessageBox.Show("Select a container");
                return;
            }

            string ciName = textBoxNameContInst.Text.Trim();
            if (string.IsNullOrWhiteSpace(textBoxNameContInst.Text))
            {
                MessageBox.Show("Content Instance name is required");
                return;
            }

            string containerPath = comboBoxContainer.SelectedItem.ToString();
            if (!TryParseContainerPath(containerPath, out string appName, out string containerName))
            {
                MessageBox.Show("Invalid container path format.");
                return;
            }

            var contentObj = new
            {
                value = textBoxAction1.Text,
            };

            // content tem de ser string (JSON “dentro” de string)
            string contentJson = JsonConvert.SerializeObject(contentObj);

            var body = new Dictionary<string, object>
            {
                { "resource-name", ciName },
                { "res-type", "content-instance" },
                { "content-type", "application/json" },
                { "content", contentJson }
            };

            var request = new RestRequest(
                $"/api/somiod/{appName}/{containerName}",
                Method.Post
            );

            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(body);

            var response = client.Execute(request);

            if (!response.IsSuccessful)
            {
                MessageBox.Show($"Error deleting content instance: {(int)response.StatusCode} {response.StatusDescription}\n{response.Content}");
                return;
            }

            MessageBox.Show("Content Instance '" + ciName + "' created successfully");

            LoadContentInstances();
            ResetUI();
        }


        private void ResetUI()
        {
            comboBoxContainer.SelectedIndex = -1;
            comboBoxContainer.Text = "";

            comboBox2DeleteContInst.SelectedIndex = -1;
            comboBox2DeleteContInst.Text = "";

            textBoxNameContInst.Clear();
            textBoxAction1.Clear();
        }

        private void buttonDeleteContentInstance_Click(object sender, EventArgs e)
        {
            if (comboBox2DeleteContInst.SelectedItem == null)
            {
                MessageBox.Show("Select a content-instance to delete");
                return;
            }

            string ciPath = comboBox2DeleteContInst.SelectedItem.ToString();

            if (!TryParseContentInstancePath(ciPath, out string appName, out string containerName, out string ciName))
            {
                MessageBox.Show("Invalid content-instance path format.");
                return;
            }

            var request = new RestRequest(
                $"/api/somiod/{appName}/{containerName}/{ciName}",
                Method.Delete
            );

            var response = client.Execute(request);

            if (!response.IsSuccessful)
            {
                MessageBox.Show(
                    $"Error deleting content instance: {(int)response.StatusCode} {response.StatusDescription}\n{response.Content}"
                );
                return;
            }

            MessageBox.Show("Content Instance '" + ciName + "' deleted successfully");

            LoadContentInstances();
            ResetUI();
        }


        // --------- helpers ---------

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

        private bool TryParseContentInstancePath(string path, out string appName, out string containerName, out string ciName)
        {
            appName = null;
            containerName = null;
            ciName = null;

            var parts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 5) return false;
            if (!parts[0].Equals("api", StringComparison.OrdinalIgnoreCase)) return false;
            if (!parts[1].Equals("somiod", StringComparison.OrdinalIgnoreCase)) return false;

            appName = parts[2];
            containerName = parts[3];
            ciName = parts[4];
            return true;
        }

    }
}
