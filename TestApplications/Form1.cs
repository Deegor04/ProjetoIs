using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SomiodPublisher
{
    public partial class Form1 : Form
    {

        private static readonly HttpClient _http = new HttpClient();
        private readonly string _hostBase = "http://localhost:58066";
        private readonly string _baseUrl = "http://localhost:58066/api/somiod";
        private readonly Dictionary<string, string> _doorState = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadTreeView();
        }

        private void btnRefreshTree_Click(object sender, EventArgs e)
        {
            LoadTreeView();
        }

        private void buttonNewApplication_Click(object sender, EventArgs e)
        {
            var form = new CreateApp();
            form.Show();
        }

        private void buttonNewContainer_Click(object sender, EventArgs e)
        {
            var form = new CreateContainer();
            form.Show();
        }

        private void buttonContentInstance_Click(object sender, EventArgs e)
        {
            var form = new CreateContentInstance();
            form.Show();
        }

        private void buttonSubscription_Click(object sender, EventArgs e)
        {
            var form = new CreateSubs();
            form.Show();
        }

        private async void LoadTreeView()
        {
            try
            {
                treeViewSomiod.Nodes.Clear();

                var root = new TreeNode("SOMIOD");
                treeViewSomiod.Nodes.Add(root);

                var allSubs = await DiscoverAsync(_baseUrl, "subscription");

                // 1) Applications
                var applications = await DiscoverAsync(_baseUrl, "application");

                foreach (var appPathRaw in applications)
                {
                    var appName = appPathRaw.Split('/').Last();
                    var appPath = ToAbsoluteUrl(appPathRaw);

                    var appNode = new TreeNode(appName)
                    {
                        Tag = appPath
                    };
                    root.Nodes.Add(appNode);

                    // 2) Containers
                    var containers = await DiscoverAsync(appPath, "container");

                    foreach (var contPathRaw in containers)
                    {
                        var contName = contPathRaw.Split('/').Last();
                        var contPath = ToAbsoluteUrl(contPathRaw);

                        var contNode = new TreeNode(contName)
                        {
                            Tag = contPath
                        };
                        appNode.Nodes.Add(contNode);

                        // A) Content Instances
                        var ci = new TreeNode("Content Instances");
                        contNode.Nodes.Add(ci);

                        var cis = await DiscoverAsync(contPath, "content-instance");
                        foreach (var ciPathRaw in cis)
                        {
                            ci.Nodes.Add(new TreeNode(ciPathRaw.Split('/').Last())
                            {
                                Tag = ToAbsoluteUrl(ciPathRaw)
                            });
                        }

                        // B) Subscriptions
                        var sub = new TreeNode("Subscriptions");
                        contNode.Nodes.Add(sub);

                        var subPrefix = $"/api/somiod/{appName}/{contName}/subs/";
                        var subsForThisContainer = allSubs
                            .Where(s => s.StartsWith(subPrefix, StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        foreach (var subPathRaw in subsForThisContainer)
                        {
                            sub.Nodes.Add(new TreeNode(subPathRaw.Split('/').Last())
                            {
                                Tag = ToAbsoluteUrl(subPathRaw)
                            });
                        }
                    }
                }
                root.Expand();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load TreeView: " + ex.Message);
            }
        }


        private async void treeViewSomiod_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = e.Node;
            var tag = node.Tag as string;

            if (string.IsNullOrWhiteSpace(tag))
            {
                richTextBoxDetails.Text = $"Selected: {e.Node.Text}\n(No resource to fetch)";
                return;
            }

            var url = ToAbsoluteUrl(tag);

            try
            {
                richTextBoxDetails.Text =
                    $"Selected: {e.Node.Text}\n" +
                    $"{url}\n\n" +
                    "Loading...";

                var json = await HttpGetAsync(url);

                richTextBoxDetails.Text =
                    $"Selected: {e.Node.Text}\n" +
                    $"{url}\n\n" +
                    NormalizeToOfficialJson(json);
            }
            catch (Exception ex)
            {
                richTextBoxDetails.Text =
                    $"Selected: {e.Node.Text}\n" +
                    $"{url}\n\n" +
                    "ERROR:\n" + ex.Message;
            }

            panelActions.Controls.Clear();

            if (node.Level == 2 && !string.IsNullOrWhiteSpace(tag))
            {
                var containerName = node.Text;
                var containerUrl = ToAbsoluteUrl(tag);

                ShowActionsForContainer(containerName, containerUrl);
            }
        }

        private void ShowActionsForContainer(string containerName, string containerUrl)
        {
            var title = new Label { Text = $"Actions: {containerName}", AutoSize = true, Top = 10, Left = 10 };
            panelActions.Controls.Add(title);

            var c = (containerName ?? "").Trim().ToLowerInvariant();

            if (c == "door" || c.StartsWith("door"))
            {
                AddDoorActions(containerUrl);
                return;
            }

            if (c == "light" || c == "lamp" || c.StartsWith("light") || c.StartsWith("lamp"))
            {
                AddLightActions(containerUrl);
                return;
            }

            if (c == "blind" || c == "blinds" || c.StartsWith("blind") || c.StartsWith("blinds") ||
                c == "shutter" || c.StartsWith("shutter") ||
                c == "curtain" || c == "curtains" || c.StartsWith("curtain") || c.StartsWith("curtains"))
            {
                AddBlindsActions(containerUrl);
                return;
            }

            AddGenericValueActions(containerUrl);
        }


        private void AddDoorActions(string containerUrl)
        {
            var btnOpen = new Button { Text = "Open", Width = 200, Top = 40, Left = 10 };
            var btnClose = new Button { Text = "Close", Width = 200, Top = 80, Left = 10 };
            var btnToggle = new Button { Text = "Toggle", Width = 200, Top = 120, Left = 10 };

            btnOpen.Click += async (s, e) =>
            {
                await SendValueToContainer(containerUrl, "open");
                _doorState[containerUrl] = "open";
            };

            btnClose.Click += async (s, e) =>
            {
                await SendValueToContainer(containerUrl, "close");
                _doorState[containerUrl] = "close";
            };

            btnToggle.Click += async (s, e) =>
            {
                var current = _doorState.ContainsKey(containerUrl) ? _doorState[containerUrl] : "close";
                var next = current.Equals("open", StringComparison.OrdinalIgnoreCase) ? "close" : "open";

                await SendValueToContainer(containerUrl, next);
                _doorState[containerUrl] = next;
            };

            panelActions.Controls.Add(btnOpen);
            panelActions.Controls.Add(btnClose);
            panelActions.Controls.Add(btnToggle);
        }

        private void AddLightActions(string containerUrl)
        {
            var lbl = new Label { Text = "Brightness", AutoSize = true, Top = 40, Left = 10 };

            var track = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 50,
                TickFrequency = 10,
                Width = 200,
                Top = 60,
                Left = 10
            };

            track.Scroll += (s, e) => { lbl.Text = $"Brightness: {track.Value}%"; };

            var btnSend = new Button { Text = "Publish", Width = 200, Top = 120, Left = 10 };
            btnSend.Click += async (s, e) =>
            {
                await SendValueToContainer(containerUrl, track.Value.ToString());
            };

            panelActions.Controls.Add(lbl);
            panelActions.Controls.Add(track);
            panelActions.Controls.Add(btnSend);
        }

        private void AddBlindsActions(string containerUrl)
        {
            var lbl = new Label { Text = "Blinds", AutoSize = true, Top = 40, Left = 10 };

            var track = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 50,
                TickFrequency = 5,
                Width = 200,
                Top = 60,
                Left = 10
            };

            track.Scroll += (s, e) => { lbl.Text = $"Blinds: {track.Value}%"; };

            var btnSend = new Button { Text = "Publish", Width = 200, Top = 120, Left = 10 };
            btnSend.Click += async (s, e) =>
            {
                await SendValueToContainer(containerUrl, track.Value.ToString());
            };

            panelActions.Controls.Add(lbl);
            panelActions.Controls.Add(track);
            panelActions.Controls.Add(btnSend);
        }

        private void AddGenericValueActions(string containerUrl)
        {
            var txt = new TextBox { Width = 200, Top = 40, Left = 10 };
            var btn = new Button { Text = "Publish", Width = 200, Top = 80, Left = 10 };

            btn.Click += async (s, e) =>
            {
                var v = txt.Text.Trim();
                if (string.IsNullOrWhiteSpace(v)) return;
                await SendValueToContainer(containerUrl, v);
            };

            panelActions.Controls.Add(txt);
            panelActions.Controls.Add(btn);
        }

        private async Task SendValueToContainer(string containerUrl, string value)
        {
            if (!TryParseContainerFromAny(containerUrl, out var appName, out var containerName))
            {
                MessageBox.Show("Invalid container path.");
                return;
            }

            var ciName = "ci_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");

            var contentObj = new { value = value };
            var contentJson = Newtonsoft.Json.JsonConvert.SerializeObject(contentObj);

            var body = new Dictionary<string, object>
            {
                { "resource-name", ciName },
                { "res-type", "content-instance" },
                { "content-type", "application/json" },
                { "content", contentJson }
            };

            var url = $"{_hostBase.TrimEnd('/')}/api/somiod/{appName}/{containerName}";
            var jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(body);

            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync(url, httpContent);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(
                    $"Error creating content instance:\n" +
                    $"{(int)response.StatusCode} {response.ReasonPhrase}\n\n{responseBody}"
                );
            }
        }

        private bool TryParseContainerFromAny(string input, out string appName, out string containerName)
        {
            appName = null;
            containerName = null;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            if (Uri.TryCreate(input, UriKind.Absolute, out var uri))
                input = uri.AbsolutePath;

            input = input.Trim();
            if (input.StartsWith("/"))
                input = input.Substring(1);

            var parts = input.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 4) return false;
            if (!parts[0].Equals("api", StringComparison.OrdinalIgnoreCase)) return false;
            if (!parts[1].Equals("somiod", StringComparison.OrdinalIgnoreCase)) return false;

            appName = parts[2];
            containerName = parts[3];
            return true;
        }

        private async Task<List<string>> DiscoverAsync(string url, string discoveryType)
        {
            url = ToAbsoluteUrl(url);

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("somiod-discovery", discoveryType);

            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return new List<string>();

            var json = await response.Content.ReadAsStringAsync();
            var arr = JArray.Parse(json);

            return arr.Select(x => x.ToString()).ToList();
        }
        private string ToAbsoluteUrl(string urlOrPath)
        {
            if (string.IsNullOrWhiteSpace(urlOrPath))
                return urlOrPath;

            if (Uri.IsWellFormedUriString(urlOrPath, UriKind.Absolute))
                return urlOrPath;

            if (urlOrPath.StartsWith("/"))
                return _hostBase.TrimEnd('/') + urlOrPath;

            return _hostBase.TrimEnd('/') + "/" + urlOrPath;
        }

        private string NormalizeToOfficialJson(string rawJson)
        {
            JObject jo;
            try { jo = JObject.Parse(rawJson); }
            catch { return rawJson; }

            string GetStr(params string[] keys)
            {
                foreach (var k in keys)
                {
                    var t = jo[k];
                    if (t != null && t.Type != JTokenType.Null)
                        return t.ToString();
                }
                return null;
            }

            var official = new JObject();

            // Base comum (application/container/subscription/content-instance)
            var resourceName = GetStr("resource-name", "ResourceName");
            if (!string.IsNullOrWhiteSpace(resourceName))
                official["resource-name"] = resourceName;

            var resType = GetStr("res-type", "ResType");
            if (!string.IsNullOrWhiteSpace(resType))
                official["res-type"] = resType;

            var creation = GetStr("creation-datetime", "CreationDatetime", "CreationDateTime");
            if (!string.IsNullOrWhiteSpace(creation))
                official["creation-datetime"] = creation;

            // Campos opcionais conforme o recurso
            var appRef = GetStr("application-resource-name", "ApplicationResourceName");
            if (!string.IsNullOrWhiteSpace(appRef))
                official["application-resource-name"] = appRef;

            var containerRef = GetStr("container-resource-name", "ContainerResourceName");
            if (!string.IsNullOrWhiteSpace(containerRef))
                official["container-resource-name"] = containerRef;

            var contentType = GetStr("content-type", "ContentType");
            if (!string.IsNullOrWhiteSpace(contentType))
                official["content-type"] = contentType;

            var content = GetStr("content", "Content");
            if (!string.IsNullOrWhiteSpace(content))
                official["content"] = content;

            var endpoint = GetStr("endpoint", "Endpoint");
            if (!string.IsNullOrWhiteSpace(endpoint))
                official["endpoint"] = endpoint;

            var ev = GetStr("event", "Event");
            if (!string.IsNullOrWhiteSpace(ev))
                official["event"] = ev;

            if (!official.Properties().Any())
                return rawJson;

            return official.ToString(Newtonsoft.Json.Formatting.Indented);
        }

        private async Task<string> HttpGetAsync(string absoluteUrl)
        {
            var response = await _http.GetAsync(absoluteUrl);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"HTTP {(int)response.StatusCode} - {response.ReasonPhrase}\n\n{body}");

            return body;
        }
    }
}
