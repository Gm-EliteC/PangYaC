using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO.Pipes;
using System.IO;
using System.Configuration;


namespace WindowsFormsApp1
{
    public class MainForm : Form
    {
        private Label WidthLabel;
        private TextBox WidthInput;
        private Label HeightResolutionLabel;
        private TextBox HeightResolutionInput;
        private ComboBox ResolutionDropdown;
        private ComboBox PresetDropdown;
        private Label PowerLabel;
        private TextBox PowerInput;
        private Label RingPowerLabel;
        private TextBox RingPowerInput;
        private Label CardPowerLabel;
        private TextBox CardPowerInput;
        private Label MascotPowerLabel;
        private TextBox MascotPowerInput;
        private Label CardPowerShotPowerLabel;
        private TextBox CardPowerShotPowerInput;
        private CheckBox AutoDetectResolutionCheckbox;
        private Label Presets;
        private Label label1;
        private Label label2;
        private Button Prank;
        private Label label3;
        private CheckBox HideBackgroundCheckbox;
        private Button SaveButton;


        public MainForm()
        {
            InitializeComponent(); // Initialize components as usual
            LoadSettings();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            this.CenterToScreen();

            SaveButton.Click += new System.EventHandler(SaveButton_Click);

            
            

        }

        private void LoadSettings()
        {
            WidthInput.Text = ConfigurationManager.AppSettings["Width"];
            HeightResolutionInput.Text = ConfigurationManager.AppSettings["HeightResolution"];
            PowerInput.Text = ConfigurationManager.AppSettings["Power"];
            RingPowerInput.Text = ConfigurationManager.AppSettings["RingPower"];
            CardPowerInput.Text = ConfigurationManager.AppSettings["CardPower"];
            MascotPowerInput.Text = ConfigurationManager.AppSettings["MascotPower"];
            CardPowerShotPowerInput.Text = ConfigurationManager.AppSettings["CardPowerShotPower"];

            // Load background visibility setting
            string hideBackground = ConfigurationManager.AppSettings["HideBackground"];
            if (!string.IsNullOrEmpty(hideBackground))
            {
                HideBackgroundCheckbox.Checked = bool.Parse(hideBackground);
            }
        }




        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        private (int width, int height)? GetPangyaResolution()
        {
            IntPtr hWnd = FindWindow(null, "Pangya Reborn!");
            if (hWnd != IntPtr.Zero && GetClientRect(hWnd, out RECT rect))
            {
                // GetClientRect returns the client area size directly
                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;
                return (width, height);
            }
            return null; // Window not found
        }




        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.WidthLabel = new System.Windows.Forms.Label();
            this.WidthInput = new System.Windows.Forms.TextBox();
            this.HeightResolutionLabel = new System.Windows.Forms.Label();
            this.HeightResolutionInput = new System.Windows.Forms.TextBox();
            this.ResolutionDropdown = new System.Windows.Forms.ComboBox();
            this.PresetDropdown = new System.Windows.Forms.ComboBox();
            this.PowerLabel = new System.Windows.Forms.Label();
            this.PowerInput = new System.Windows.Forms.TextBox();
            this.RingPowerLabel = new System.Windows.Forms.Label();
            this.RingPowerInput = new System.Windows.Forms.TextBox();
            this.CardPowerLabel = new System.Windows.Forms.Label();
            this.CardPowerInput = new System.Windows.Forms.TextBox();
            this.MascotPowerLabel = new System.Windows.Forms.Label();
            this.MascotPowerInput = new System.Windows.Forms.TextBox();
            this.CardPowerShotPowerLabel = new System.Windows.Forms.Label();
            this.CardPowerShotPowerInput = new System.Windows.Forms.TextBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.AutoDetectResolutionCheckbox = new System.Windows.Forms.CheckBox();
            this.Presets = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Prank = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.HideBackgroundCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // WidthLabel
            // 
            this.WidthLabel.AutoSize = true;
            this.WidthLabel.Location = new System.Drawing.Point(20, 37);
            this.WidthLabel.Name = "WidthLabel";
            this.WidthLabel.Size = new System.Drawing.Size(38, 13);
            this.WidthLabel.TabIndex = 0;
            this.WidthLabel.Text = "Width:";
            // 
            // WidthInput
            // 
            this.WidthInput.Location = new System.Drawing.Point(80, 34);
            this.WidthInput.Name = "WidthInput";
            this.WidthInput.Size = new System.Drawing.Size(100, 20);
            this.WidthInput.TabIndex = 1;
            // 
            // HeightResolutionLabel
            // 
            this.HeightResolutionLabel.AutoSize = true;
            this.HeightResolutionLabel.Location = new System.Drawing.Point(20, 60);
            this.HeightResolutionLabel.Name = "HeightResolutionLabel";
            this.HeightResolutionLabel.Size = new System.Drawing.Size(41, 13);
            this.HeightResolutionLabel.TabIndex = 2;
            this.HeightResolutionLabel.Text = "Height:";
            // 
            // HeightResolutionInput
            // 
            this.HeightResolutionInput.Location = new System.Drawing.Point(80, 60);
            this.HeightResolutionInput.Name = "HeightResolutionInput";
            this.HeightResolutionInput.Size = new System.Drawing.Size(100, 20);
            this.HeightResolutionInput.TabIndex = 3;
            // 
            // ResolutionDropdown
            // 
            this.ResolutionDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ResolutionDropdown.Items.AddRange(new object[] {
            "2560x1440",
            "2048x1152",
            "1920x1080",
            "1600x900"});
            this.ResolutionDropdown.Location = new System.Drawing.Point(80, 7);
            this.ResolutionDropdown.Name = "ResolutionDropdown";
            this.ResolutionDropdown.Size = new System.Drawing.Size(100, 21);
            this.ResolutionDropdown.TabIndex = 4;
            this.ResolutionDropdown.SelectedIndexChanged += new System.EventHandler(this.ResolutionDropdown_SelectedIndexChanged);
            // 
            // PresetDropdown
            // 
            this.PresetDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PresetDropdown.Items.AddRange(new object[] {
            "450+10y"});
            this.PresetDropdown.Location = new System.Drawing.Point(38, 136);
            this.PresetDropdown.Name = "PresetDropdown";
            this.PresetDropdown.Size = new System.Drawing.Size(121, 21);
            this.PresetDropdown.TabIndex = 5;
            this.PresetDropdown.SelectedIndexChanged += new System.EventHandler(this.PresetDropdown_SelectedIndexChanged);
            // 
            // PowerLabel
            // 
            this.PowerLabel.AutoSize = true;
            this.PowerLabel.Location = new System.Drawing.Point(20, 180);
            this.PowerLabel.Name = "PowerLabel";
            this.PowerLabel.Size = new System.Drawing.Size(64, 13);
            this.PowerLabel.TabIndex = 6;
            this.PowerLabel.Text = "Stats Power";
            // 
            // PowerInput
            // 
            this.PowerInput.Location = new System.Drawing.Point(120, 173);
            this.PowerInput.Name = "PowerInput";
            this.PowerInput.Size = new System.Drawing.Size(60, 20);
            this.PowerInput.TabIndex = 7;
            // 
            // RingPowerLabel
            // 
            this.RingPowerLabel.AutoSize = true;
            this.RingPowerLabel.Location = new System.Drawing.Point(20, 204);
            this.RingPowerLabel.Name = "RingPowerLabel";
            this.RingPowerLabel.Size = new System.Drawing.Size(62, 13);
            this.RingPowerLabel.TabIndex = 8;
            this.RingPowerLabel.Text = "Ring Power";
            // 
            // RingPowerInput
            // 
            this.RingPowerInput.Location = new System.Drawing.Point(120, 197);
            this.RingPowerInput.Name = "RingPowerInput";
            this.RingPowerInput.Size = new System.Drawing.Size(60, 20);
            this.RingPowerInput.TabIndex = 9;
            // 
            // CardPowerLabel
            // 
            this.CardPowerLabel.AutoSize = true;
            this.CardPowerLabel.Location = new System.Drawing.Point(20, 230);
            this.CardPowerLabel.Name = "CardPowerLabel";
            this.CardPowerLabel.Size = new System.Drawing.Size(100, 13);
            this.CardPowerLabel.TabIndex = 10;
            this.CardPowerLabel.Text = "Card Power (Pippin)";
            // 
            // CardPowerInput
            // 
            this.CardPowerInput.Location = new System.Drawing.Point(120, 223);
            this.CardPowerInput.Name = "CardPowerInput";
            this.CardPowerInput.Size = new System.Drawing.Size(60, 20);
            this.CardPowerInput.TabIndex = 11;
            // 
            // MascotPowerLabel
            // 
            this.MascotPowerLabel.AutoSize = true;
            this.MascotPowerLabel.Location = new System.Drawing.Point(20, 256);
            this.MascotPowerLabel.Name = "MascotPowerLabel";
            this.MascotPowerLabel.Size = new System.Drawing.Size(78, 13);
            this.MascotPowerLabel.TabIndex = 12;
            this.MascotPowerLabel.Text = "Mascot Power:";
            // 
            // MascotPowerInput
            // 
            this.MascotPowerInput.Location = new System.Drawing.Point(120, 249);
            this.MascotPowerInput.Name = "MascotPowerInput";
            this.MascotPowerInput.Size = new System.Drawing.Size(60, 20);
            this.MascotPowerInput.TabIndex = 13;
            // 
            // CardPowerShotPowerLabel
            // 
            this.CardPowerShotPowerLabel.AutoSize = true;
            this.CardPowerShotPowerLabel.Location = new System.Drawing.Point(20, 282);
            this.CardPowerShotPowerLabel.Name = "CardPowerShotPowerLabel";
            this.CardPowerShotPowerLabel.Size = new System.Drawing.Size(69, 13);
            this.CardPowerShotPowerLabel.TabIndex = 14;
            this.CardPowerShotPowerLabel.Text = "Lolo Card PS";
            // 
            // CardPowerShotPowerInput
            // 
            this.CardPowerShotPowerInput.Location = new System.Drawing.Point(120, 275);
            this.CardPowerShotPowerInput.Name = "CardPowerShotPowerInput";
            this.CardPowerShotPowerInput.Size = new System.Drawing.Size(60, 20);
            this.CardPowerShotPowerInput.TabIndex = 15;
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(62, 363);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(80, 30);
            this.SaveButton.TabIndex = 16;
            this.SaveButton.Text = "Save";
            // 
            // AutoDetectResolutionCheckbox
            // 
            this.AutoDetectResolutionCheckbox.AutoSize = true;
            this.AutoDetectResolutionCheckbox.Location = new System.Drawing.Point(23, 86);
            this.AutoDetectResolutionCheckbox.Name = "AutoDetectResolutionCheckbox";
            this.AutoDetectResolutionCheckbox.Size = new System.Drawing.Size(136, 17);
            this.AutoDetectResolutionCheckbox.TabIndex = 17;
            this.AutoDetectResolutionCheckbox.Text = "Auto Detect Resolution";
            this.AutoDetectResolutionCheckbox.UseVisualStyleBackColor = true;
            this.AutoDetectResolutionCheckbox.CheckedChanged += new System.EventHandler(this.AutoDetectResolutionCheckbox_CheckedChanged);
            // 
            // Presets
            // 
            this.Presets.AutoSize = true;
            this.Presets.Location = new System.Drawing.Point(20, 10);
            this.Presets.Name = "Presets";
            this.Presets.Size = new System.Drawing.Size(45, 13);
            this.Presets.TabIndex = 18;
            this.Presets.Text = "Presets:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(46, 120);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Select Power Presets";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(70, 405);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "by ELITEC";
            // 
            // Prank
            // 
            this.Prank.Location = new System.Drawing.Point(44, 432);
            this.Prank.Name = "Prank";
            this.Prank.Size = new System.Drawing.Size(115, 32);
            this.Prank.TabIndex = 21;
            this.Prank.Text = "Enable Wind 0";
            this.Prank.UseVisualStyleBackColor = true;
            this.Prank.Click += new System.EventHandler(this.Prank_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(61, 442);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Nah, too easyy!";
            this.label3.Visible = false;
            // 
            // HideBackgroundCheckbox
            // 
            this.HideBackgroundCheckbox.AutoSize = true;
            this.HideBackgroundCheckbox.Location = new System.Drawing.Point(33, 323);
            this.HideBackgroundCheckbox.Name = "HideBackgroundCheckbox";
            this.HideBackgroundCheckbox.Size = new System.Drawing.Size(137, 17);
            this.HideBackgroundCheckbox.TabIndex = 23;
            this.HideBackgroundCheckbox.Text = "No Background Picture";
            this.HideBackgroundCheckbox.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(202, 483);
            this.Controls.Add(this.HideBackgroundCheckbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Prank);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Presets);
            this.Controls.Add(this.AutoDetectResolutionCheckbox);
            this.Controls.Add(this.WidthLabel);
            this.Controls.Add(this.WidthInput);
            this.Controls.Add(this.HeightResolutionLabel);
            this.Controls.Add(this.HeightResolutionInput);
            this.Controls.Add(this.ResolutionDropdown);
            this.Controls.Add(this.PresetDropdown);
            this.Controls.Add(this.PowerLabel);
            this.Controls.Add(this.PowerInput);
            this.Controls.Add(this.RingPowerLabel);
            this.Controls.Add(this.RingPowerInput);
            this.Controls.Add(this.CardPowerLabel);
            this.Controls.Add(this.CardPowerInput);
            this.Controls.Add(this.MascotPowerLabel);
            this.Controls.Add(this.MascotPowerInput);
            this.Controls.Add(this.CardPowerShotPowerLabel);
            this.Controls.Add(this.CardPowerShotPowerInput);
            this.Controls.Add(this.SaveButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Smart Calculator Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void ResolutionDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ResolutionDropdown.SelectedItem.ToString() == "2560x1440")
            {
                WidthInput.Text = "2560";
                HeightResolutionInput.Text = "1440";
            }
            
            if (ResolutionDropdown.SelectedItem.ToString() == "1920x1080")
            {
                WidthInput.Text = "1920";
                HeightResolutionInput.Text = "1080";
            }

            if (ResolutionDropdown.SelectedItem.ToString() == "1600x900")
            {
                WidthInput.Text = "1600";
                HeightResolutionInput.Text = "900";
            }

            if (ResolutionDropdown.SelectedItem.ToString() == "2048x1152")
            {
                WidthInput.Text = "2048";
                HeightResolutionInput.Text = "1152";
            }

            if (ResolutionDropdown.SelectedItem.ToString() == "1280x960")
            {
                WidthInput.Text = "1280";
                HeightResolutionInput.Text = "960";
            }
        }

        private void PresetDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PresetDropdown.SelectedItem.ToString() == "450+10y")
            {
                PowerInput.Text = "100";
                RingPowerInput.Text = "20";
                CardPowerInput.Text = "16";
                MascotPowerInput.Text = "14";
                CardPowerShotPowerInput.Text = "10";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Check if any required field is empty
            if (string.IsNullOrWhiteSpace(WidthInput.Text) || string.IsNullOrWhiteSpace(HeightResolutionInput.Text) ||
                string.IsNullOrWhiteSpace(PowerInput.Text) || string.IsNullOrWhiteSpace(RingPowerInput.Text) ||
                string.IsNullOrWhiteSpace(CardPowerInput.Text) || string.IsNullOrWhiteSpace(MascotPowerInput.Text) ||
                string.IsNullOrWhiteSpace(CardPowerShotPowerInput.Text))
            {
                MessageBox.Show("Please fill in all fields before saving.");
            }
            else
            {

               
                dynamic inputData = new
                {
                    Width = WidthInput.Text,
                    HeightResolution = HeightResolutionInput.Text,
                    Power = PowerInput.Text,
                    RingPower = RingPowerInput.Text,
                    CardPower = CardPowerInput.Text,
                    MascotPower = MascotPowerInput.Text,
                    CardPowerShotPower = CardPowerShotPowerInput.Text,
                    HideBackground = HideBackgroundCheckbox.Checked
                };
                SaveSettings();

                //MessageBox.Show("Data saved!");
                PangYaC inputValues = new PangYaC(inputData);
                inputValues.Show();
                this.Hide();

               
            }
        }


        private void AutoDetectResolutionCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (AutoDetectResolutionCheckbox.Checked)
            {
                var resolution = GetPangyaResolution();
                if (resolution.HasValue)
                {
                    // Get the raw window dimensions
                    int width = resolution.Value.width;
                    int height = resolution.Value.height;

                    // Map to standard resolutions
                    if (IsCloseToResolution(width, height, 2560, 1440))
                    {
                        WidthInput.Text = "2560";
                        HeightResolutionInput.Text = "1440";
                    }
                    else if (IsCloseToResolution(width, height, 2048, 1152))
                    {
                        WidthInput.Text = "2048";
                        HeightResolutionInput.Text = "1152";
                    }
                    else if (IsCloseToResolution(width, height, 1920, 1080))
                    {
                        WidthInput.Text = "1920";
                        HeightResolutionInput.Text = "1080";
                    }
                    else if (IsCloseToResolution(width, height, 1600, 900))
                    {
                        WidthInput.Text = "1600";
                        HeightResolutionInput.Text = "900";
                    }
                    else if (IsCloseToResolution(width, height, 1280, 960))
                    {
                        WidthInput.Text = "1280";
                        HeightResolutionInput.Text = "960";
                    }
                    else
                    {
                        WidthInput.Text = width.ToString();
                        HeightResolutionInput.Text = height.ToString();
                    }
                }
                else
                {
                    MessageBox.Show("Resolution Not Supported Yet.");
                }
            }
        }

        private bool IsCloseToResolution(int detectedWidth, int detectedHeight, int targetWidth, int targetHeight)
        {
            // Allow for small variations in resolution
            const int tolerance = 20;
            return Math.Abs(detectedWidth - targetWidth) <= tolerance &&
                   Math.Abs(detectedHeight - targetHeight) <= tolerance;
        }
        private void SaveSettings()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.AppSettings.Settings["Width"] == null)
                config.AppSettings.Settings.Add("Width", WidthInput.Text);
            else
                config.AppSettings.Settings["Width"].Value = WidthInput.Text;

            if (config.AppSettings.Settings["HeightResolution"] == null)
                config.AppSettings.Settings.Add("HeightResolution", HeightResolutionInput.Text);
            else
                config.AppSettings.Settings["HeightResolution"].Value = HeightResolutionInput.Text;

            if (config.AppSettings.Settings["Power"] == null)
                config.AppSettings.Settings.Add("Power", PowerInput.Text);
            else
                config.AppSettings.Settings["Power"].Value = PowerInput.Text;

            if (config.AppSettings.Settings["RingPower"] == null)
                config.AppSettings.Settings.Add("RingPower", RingPowerInput.Text);
            else
                config.AppSettings.Settings["RingPower"].Value = RingPowerInput.Text;

            if (config.AppSettings.Settings["CardPower"] == null)
                config.AppSettings.Settings.Add("CardPower", CardPowerInput.Text);
            else
                config.AppSettings.Settings["CardPower"].Value = CardPowerInput.Text;

            if (config.AppSettings.Settings["MascotPower"] == null)
                config.AppSettings.Settings.Add("MascotPower", MascotPowerInput.Text);
            else
                config.AppSettings.Settings["MascotPower"].Value = MascotPowerInput.Text;

            if (config.AppSettings.Settings["CardPowerShotPower"] == null)
                config.AppSettings.Settings.Add("CardPowerShotPower", CardPowerShotPowerInput.Text);
            else
                config.AppSettings.Settings["CardPowerShotPower"].Value = CardPowerShotPowerInput.Text;

            if (config.AppSettings.Settings["HideBackground"] == null)
                config.AppSettings.Settings.Add("HideBackground", HideBackgroundCheckbox.Checked.ToString());
            else
                config.AppSettings.Settings["HideBackground"].Value = HideBackgroundCheckbox.Checked.ToString();

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
           
        }

        private void Prank_Click(object sender, EventArgs e)
        {
            Prank.Visible = false;
            label3.Visible = true;
        }
    }

}
