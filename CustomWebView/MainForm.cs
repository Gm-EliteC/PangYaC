using Microsoft.Web.WebView2.WinForms;
using System;
using System.IO;
using System.Windows.Forms;

namespace YourAppNamespace
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private void InitializeComponent()
        {
            // Manually initialize and add the WebView2 control to the form
            this.ClientSize = new System.Drawing.Size(800, 600); // Set an appropriate size for your form
            this.Text = "Smart Calculator";
        }

        private async void InitializeWebView()
        {
            WebView2 webView = new WebView2
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(webView);

            // Ensure WebView2 is initialized before setting Source
            await webView.EnsureCoreWebView2Async();

            // Get the path to the HTML file in the output directory
            string htmFileName = "index.htm";
            string htmPath = Path.Combine(Application.StartupPath, htmFileName);

            // Check if the file exists and set the Source
            if (File.Exists(htmPath))
            {
                webView.Source = new Uri("file:///" + htmPath.Replace("\\", "/"));
            }
            else
            {
                MessageBox.Show("HTML file not found: " + htmPath);
            }
        }
    }
}
