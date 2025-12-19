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
    public partial class CreateContainer : Form
    {
        private readonly RestClient client;
        public CreateContainer()
        {
            InitializeComponent();
            client = new RestClient(Configs.baseURI);
            LoadApplicationsForAdd();
            LoadContainers();
            ClearSelections();
        }

        private void ClearSelections()
        {
            comboBoxSelectApp.SelectedIndex = -1;
            comboBoxSelectApp.SelectedItem = null;
            comboBoxSelectApp.Text = "";
            textBoxContainerName.Clear();

            comboBoxSelContDelete.SelectedIndex = -1;
            comboBoxSelContDelete.SelectedItem = null;
            comboBoxSelContDelete.Text = "";
        }


        private void LoadContainers()
        {
            comboBoxSelContDelete.Items.Clear();

            var request = new RestRequest("/api/somiod", Method.Get);
            request.AddHeader("somiod-discovery", "container");

            var response = client.Execute<List<string>>(request);

            if (!response.IsSuccessful || response.Data == null)
                return;

            foreach (var path in response.Data)
            {
                comboBoxSelContDelete.Items.Add(path);
            }
        }


        private void LoadApplicationsForAdd()
        {
            comboBoxSelectApp.Items.Clear();

            var request = new RestRequest("/api/somiod", Method.Get);
            request.AddHeader("somiod-discovery", "application");

            var response = client.Execute<List<string>>(request);

            if (!response.IsSuccessful || response.Data == null)
            {
                MessageBox.Show("Failed to load applications",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (var path in response.Data)
            {
                comboBoxSelectApp.Items.Add(path);
            }

            comboBoxSelectApp.SelectedIndex = -1;
            comboBoxSelectApp.SelectedItem = null;
            comboBoxSelectApp.Text = "";
        }

        private void buttonAddContainer_Click(object sender, EventArgs e)
        {
            if (comboBoxSelectApp.SelectedItem == null)
            {
                MessageBox.Show("Select an application");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxContainerName.Text))
            {
                MessageBox.Show("Container name is required");
                return;
            }

            string appPath = comboBoxSelectApp.SelectedItem.ToString();
            string appName = appPath.Split('/').Last();
            string containerName = textBoxContainerName.Text.Trim();

            var body = new Dictionary<string, object>
            {
                { "resource-name", containerName }
            };

            var request = new RestRequest($"/api/somiod/{appName}", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(body);

            var response = client.Execute(request);

            if (!response.IsSuccessful)
            {
                MessageBox.Show("Error creating container");
                return;
            }

            MessageBox.Show("Container created successfully");
            LoadContainers();
            ClearSelections();
        }

        private void buttonDeleteContainer_Click(object sender, EventArgs e)
        {
            if (comboBoxSelContDelete.SelectedItem == null)
            {
                MessageBox.Show("Select a container to delete", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string path = comboBoxSelContDelete.SelectedItem.ToString();
            var parts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 3)
            {
                MessageBox.Show("Invalid container path format", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string appName = parts[parts.Length - 2];
            string containerName = parts[parts.Length - 1];

            var confirm = MessageBox.Show(
                $"Delete container '{containerName}' from app '{appName}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm != DialogResult.Yes)
                return;

            var request = new RestRequest($"/api/somiod/{appName}/{containerName}", Method.Delete);

            RestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
            {
                MessageBox.Show($"Error deleting container ({response.StatusCode})\n{response.Content}",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Container deleted successfully",
                "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadContainers();
            ClearSelections();
        }
    }
}
