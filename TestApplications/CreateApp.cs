using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SomiodPublisher.ConfigFiles;
using SomiodPublisher.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SomiodPublisher
{
    public partial class CreateApp : Form
    {
        RestClient client = null;
        bool isAtive = false;

        public CreateApp()
        {
            InitializeComponent();
            client = new RestClient(Configs.baseURI);
        }

        private void buttonCreateApp_Click(object sender, EventArgs e)
        {
            isAtive = false;

            if (string.IsNullOrWhiteSpace(textBoxCreateApp.Text))
            {
                MessageBox.Show("Application name is required");
                return;
            }

            if (!isValidName())
            {
                MessageBox.Show("Invalid Name", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!regexName(textBoxCreateApp.Text))
            {
                return;
            }

            var appName = textBoxCreateApp.Text;

            var body = new Dictionary<string, object>
            {
                ["resource-name"] = appName
            };

            var request = new RestRequest("/api/somiod", Method.Post);

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");

            request.AddJsonBody(body);

            RestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
            {
                MessageBox.Show("Error creating application", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            buttonCreateApp.BackColor = System.Drawing.Color.Green;
            buttonCreateApp.Text = "Added";
            buttonCreateApp.Enabled = false;
            textBoxCreateApp.Enabled = false;

            isAtive = true;
        }


        private bool isValidName()
        {
            var request = new RestRequest("/api/somiod", Method.Get);
            request.AddHeader("somiod-discovery", "application");

            var response = client.Execute<List<string>>(request);

            if (!response.IsSuccessful || response.Data == null)
                return true; 

            string appPath = $"/api/somiod/{textBoxCreateApp.Text}";

            return !response.Data.Contains(appPath);
        }


        public bool getIsAtive()
        {
            return isAtive;
        }
        private bool regexName(string name)
        {
            var regex = new Regex(@"[^a-zA-Z0-9\s]");
            if (regex.IsMatch(name))
            {
                MessageBox.Show("Only letters and numbers are allowed", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void LoadApplicationsToComboBox()
        {
            comboBoxSelectAppDel.Items.Clear();

            var request = new RestRequest("/api/somiod", Method.Get);
            request.AddHeader("somiod-discovery", "application");

            var response = client.Execute<List<string>>(request);

            if (!response.IsSuccessful || response.Data == null)
            {
                MessageBox.Show("Failed to load applications", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (var path in response.Data)
            {
                comboBoxSelectAppDel.Items.Add(path);
            }
        }

        private void CreateApp_Load(object sender, EventArgs e)
        {
            LoadApplicationsToComboBox();
        }

        private void buttonDeleteApp_Click(object sender, EventArgs e)
        {
            if (comboBoxSelectAppDel.SelectedItem == null)
            {
                MessageBox.Show("Select an application to delete",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string selectedPath = comboBoxSelectAppDel.SelectedItem.ToString();
            string appName = selectedPath.Split('/').Last();

            var confirm = MessageBox.Show(
                $"Are you sure you want to delete application '{appName}'?\n\nThis action cannot be undone.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm != DialogResult.Yes)
                return;

            var request = new RestRequest($"/api/somiod/{appName}", Method.Delete);
            RestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
            {
                MessageBox.Show($"Error deleting application ({response.StatusCode})",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            comboBoxSelectAppDel.Items.Remove(comboBoxSelectAppDel.SelectedItem);
            comboBoxSelectAppDel.SelectedIndex = -1;

            MessageBox.Show("Application deleted successfully",
                "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadApplicationsToComboBox();
        }
    }
}
