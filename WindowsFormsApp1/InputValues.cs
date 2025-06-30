using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Text;
using System.Runtime.InteropServices;
using PangYaC;
using Timer = System.Windows.Forms.Timer;
using System.Configuration;

namespace WindowsFormsApp1
{
    // This class is the main form for the application
    public partial class PangYaC : Form
    {
        private const string SETTINGS_FILE = "screen_capture_settings.txt";
        private Microsoft.Web.WebView2.WinForms.WebView2 calculationWebView;
        private Button PangYa; // Button to trigger calculation
        private Label ClubLabel, DistanceLabel, PowerShotLabel, ShotLabel;
        private ComboBox ClubDropdown, ShotDropdown, PowerShotDropdown; // Dropdowns for input
        private Label SpinLabel;
        private Label CurveLabel;
        private Label SlopeBreakLabel;
        private Label GroundLabel;
        private Label DegreeLabel;
        private Label WindLabel;
        private Label HeightLabel;
        public string Finalresult { get; private set; }
        private Label AIM;
        private Label Caliper;
        private dynamic _inputData;
        private bool isExiting = false;


        // Text inputs for data
        private TextBox WidthInput, HeightResolutionInput, PowerInput, RingPowerInput, CardPowerInput,
            MascotPowerInput, CardPowerShotPowerInput, DistanceInput, HeightInput, WindInput, DegreeInput,
            GroundInput, SpinInput, CurveInput, SlopeBreakInput;

        private Button Reset;
        private PictureBox WindCapture;
        private Dictionary<string, string> defaultValues = new Dictionary<string, string>();
        private Label F11;
        private CheckBox StayOnTop;
        private Button pinSelectionButton;
        private Thread pipeServerThread;
        private int offsetX = 0;
        private int offsetY = 0;
        private int zoom = 0;
        private Button btnUp;
        private Button btnDown;
        private Button Save;
        private Button btnLeft;
        private Button btnRight;
        private Button zoomIn;
        private Button zoomOut;
        private CheckBox CalculateOptimalSpin;
        private Bitmap originalImage;


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        private RECT? GetGameWindowRect(string windowName)
        {
            IntPtr hWnd = FindWindow(null, windowName);
            if (hWnd == IntPtr.Zero)
                return null;

            if (GetWindowRect(hWnd, out RECT rect))
                return rect;

            return null;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }


        // Constructor to receive input data
        public PangYaC(dynamic inputData)
        {
            InitializeComponent();

            try
            {
                if (!inputData.HideBackground)
                {
                    this.BackgroundImage = System.Drawing.Image.FromFile("Resources/Images/background_26.png");
                    this.BackgroundImageLayout = ImageLayout.Stretch;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading image: " + ex.Message);
            }


            _inputData = inputData;

            InputValues_Load(this, EventArgs.Empty);

            // Initialize WebView2 asynchronously
            _ = InitializeWebView();

            

            StartPipeListener();

            this.FormClosing += Form1_FormClosing;

        }

        private void InitializeComponent()
        {
            this.ClubLabel = new System.Windows.Forms.Label();
            this.ClubDropdown = new System.Windows.Forms.ComboBox();
            this.ShotLabel = new System.Windows.Forms.Label();
            this.ShotDropdown = new System.Windows.Forms.ComboBox();
            this.PowerShotLabel = new System.Windows.Forms.Label();
            this.PowerShotDropdown = new System.Windows.Forms.ComboBox();
            this.DistanceLabel = new System.Windows.Forms.Label();
            this.DistanceInput = new System.Windows.Forms.TextBox();
            this.HeightLabel = new System.Windows.Forms.Label();
            this.HeightInput = new System.Windows.Forms.TextBox();
            this.WindLabel = new System.Windows.Forms.Label();
            this.WindInput = new System.Windows.Forms.TextBox();
            this.DegreeLabel = new System.Windows.Forms.Label();
            this.DegreeInput = new System.Windows.Forms.TextBox();
            this.GroundLabel = new System.Windows.Forms.Label();
            this.GroundInput = new System.Windows.Forms.TextBox();
            this.SpinLabel = new System.Windows.Forms.Label();
            this.SpinInput = new System.Windows.Forms.TextBox();
            this.CurveLabel = new System.Windows.Forms.Label();
            this.CurveInput = new System.Windows.Forms.TextBox();
            this.SlopeBreakLabel = new System.Windows.Forms.Label();
            this.SlopeBreakInput = new System.Windows.Forms.TextBox();
            this.PangYa = new System.Windows.Forms.Button();
            this.WidthInput = new System.Windows.Forms.TextBox();
            this.HeightResolutionInput = new System.Windows.Forms.TextBox();
            this.PowerInput = new System.Windows.Forms.TextBox();
            this.RingPowerInput = new System.Windows.Forms.TextBox();
            this.CardPowerInput = new System.Windows.Forms.TextBox();
            this.MascotPowerInput = new System.Windows.Forms.TextBox();
            this.CardPowerShotPowerInput = new System.Windows.Forms.TextBox();
            this.AIM = new System.Windows.Forms.Label();
            this.Caliper = new System.Windows.Forms.Label();
            this.calculationWebView = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.Reset = new System.Windows.Forms.Button();
            this.WindCapture = new System.Windows.Forms.PictureBox();
            this.F11 = new System.Windows.Forms.Label();
            this.StayOnTop = new System.Windows.Forms.CheckBox();
            this.pinSelectionButton = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.Save = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.zoomIn = new System.Windows.Forms.Button();
            this.zoomOut = new System.Windows.Forms.Button();
            this.CalculateOptimalSpin = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.calculationWebView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WindCapture)).BeginInit();
            this.SuspendLayout();
            // 
            // ClubLabel
            // 
            this.ClubLabel.AutoSize = true;
            this.ClubLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClubLabel.Location = new System.Drawing.Point(18, 53);
            this.ClubLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ClubLabel.Name = "ClubLabel";
            this.ClubLabel.Size = new System.Drawing.Size(45, 20);
            this.ClubLabel.TabIndex = 0;
            this.ClubLabel.Text = "Club:";
            // 
            // ClubDropdown
            // 
            this.ClubDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ClubDropdown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClubDropdown.Items.AddRange(new object[] {
            "1W",
            "2W",
            "3W",
            "2I",
            "3I",
            "4I",
            "5I",
            "6I",
            "7I",
            "8I",
            "9I",
            "PW",
            "SW"});
            this.ClubDropdown.Location = new System.Drawing.Point(91, 53);
            this.ClubDropdown.Margin = new System.Windows.Forms.Padding(2);
            this.ClubDropdown.Name = "ClubDropdown";
            this.ClubDropdown.Size = new System.Drawing.Size(133, 28);
            this.ClubDropdown.TabIndex = 1;
            // 
            // ShotLabel
            // 
            this.ShotLabel.AutoSize = true;
            this.ShotLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ShotLabel.Location = new System.Drawing.Point(18, 89);
            this.ShotLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ShotLabel.Name = "ShotLabel";
            this.ShotLabel.Size = new System.Drawing.Size(47, 20);
            this.ShotLabel.TabIndex = 2;
            this.ShotLabel.Text = "Shot:";
            // 
            // ShotDropdown
            // 
            this.ShotDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ShotDropdown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ShotDropdown.Items.AddRange(new object[] {
            "Dunk",
            "Tomahawk",
            "Spike",
            "Cobra"});
            this.ShotDropdown.Location = new System.Drawing.Point(91, 89);
            this.ShotDropdown.Margin = new System.Windows.Forms.Padding(2);
            this.ShotDropdown.Name = "ShotDropdown";
            this.ShotDropdown.Size = new System.Drawing.Size(133, 28);
            this.ShotDropdown.TabIndex = 3;
            this.ShotDropdown.SelectedIndexChanged += new System.EventHandler(this.comboBoxShot_SelectedIndexChanged);
            // 
            // PowerShotLabel
            // 
            this.PowerShotLabel.AutoSize = true;
            this.PowerShotLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PowerShotLabel.Location = new System.Drawing.Point(18, 125);
            this.PowerShotLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.PowerShotLabel.Name = "PowerShotLabel";
            this.PowerShotLabel.Size = new System.Drawing.Size(34, 20);
            this.PowerShotLabel.TabIndex = 4;
            this.PowerShotLabel.Text = "PS:";
            // 
            // PowerShotDropdown
            // 
            this.PowerShotDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PowerShotDropdown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PowerShotDropdown.Items.AddRange(new object[] {
            "No Power Shot",
            "1 Power Shot",
            "2 Power Shot",
            "15y Power Shot"});
            this.PowerShotDropdown.Location = new System.Drawing.Point(91, 125);
            this.PowerShotDropdown.Margin = new System.Windows.Forms.Padding(2);
            this.PowerShotDropdown.Name = "PowerShotDropdown";
            this.PowerShotDropdown.Size = new System.Drawing.Size(133, 28);
            this.PowerShotDropdown.TabIndex = 5;
            // 
            // DistanceLabel
            // 
            this.DistanceLabel.AutoSize = true;
            this.DistanceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DistanceLabel.Location = new System.Drawing.Point(18, 166);
            this.DistanceLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.DistanceLabel.Name = "DistanceLabel";
            this.DistanceLabel.Size = new System.Drawing.Size(76, 20);
            this.DistanceLabel.TabIndex = 6;
            this.DistanceLabel.Text = "Distance:";
            // 
            // DistanceInput
            // 
            this.DistanceInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DistanceInput.Location = new System.Drawing.Point(138, 158);
            this.DistanceInput.Margin = new System.Windows.Forms.Padding(2);
            this.DistanceInput.Name = "DistanceInput";
            this.DistanceInput.Size = new System.Drawing.Size(86, 26);
            this.DistanceInput.TabIndex = 7;
            // 
            // HeightLabel
            // 
            this.HeightLabel.AutoSize = true;
            this.HeightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HeightLabel.Location = new System.Drawing.Point(18, 199);
            this.HeightLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.HeightLabel.Name = "HeightLabel";
            this.HeightLabel.Size = new System.Drawing.Size(60, 20);
            this.HeightLabel.TabIndex = 8;
            this.HeightLabel.Text = "Height:";
            // 
            // HeightInput
            // 
            this.HeightInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HeightInput.Location = new System.Drawing.Point(138, 194);
            this.HeightInput.Margin = new System.Windows.Forms.Padding(2);
            this.HeightInput.Name = "HeightInput";
            this.HeightInput.Size = new System.Drawing.Size(86, 26);
            this.HeightInput.TabIndex = 9;
            // 
            // WindLabel
            // 
            this.WindLabel.AutoSize = true;
            this.WindLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WindLabel.Location = new System.Drawing.Point(18, 232);
            this.WindLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.WindLabel.Name = "WindLabel";
            this.WindLabel.Size = new System.Drawing.Size(49, 20);
            this.WindLabel.TabIndex = 10;
            this.WindLabel.Text = "Wind:";
            // 
            // WindInput
            // 
            this.WindInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WindInput.Location = new System.Drawing.Point(138, 227);
            this.WindInput.Margin = new System.Windows.Forms.Padding(2);
            this.WindInput.Name = "WindInput";
            this.WindInput.Size = new System.Drawing.Size(86, 26);
            this.WindInput.TabIndex = 11;
            // 
            // DegreeLabel
            // 
            this.DegreeLabel.AutoSize = true;
            this.DegreeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DegreeLabel.Location = new System.Drawing.Point(18, 260);
            this.DegreeLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.DegreeLabel.Name = "DegreeLabel";
            this.DegreeLabel.Size = new System.Drawing.Size(66, 20);
            this.DegreeLabel.TabIndex = 12;
            this.DegreeLabel.Text = "Degree:";
            // 
            // DegreeInput
            // 
            this.DegreeInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DegreeInput.Location = new System.Drawing.Point(138, 260);
            this.DegreeInput.Margin = new System.Windows.Forms.Padding(2);
            this.DegreeInput.Name = "DegreeInput";
            this.DegreeInput.Size = new System.Drawing.Size(86, 26);
            this.DegreeInput.TabIndex = 13;
            // 
            // GroundLabel
            // 
            this.GroundLabel.AutoSize = true;
            this.GroundLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroundLabel.Location = new System.Drawing.Point(17, 293);
            this.GroundLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.GroundLabel.Name = "GroundLabel";
            this.GroundLabel.Size = new System.Drawing.Size(67, 20);
            this.GroundLabel.TabIndex = 14;
            this.GroundLabel.Text = "Ground:";
            // 
            // GroundInput
            // 
            this.GroundInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroundInput.Location = new System.Drawing.Point(138, 293);
            this.GroundInput.Margin = new System.Windows.Forms.Padding(2);
            this.GroundInput.Name = "GroundInput";
            this.GroundInput.Size = new System.Drawing.Size(86, 26);
            this.GroundInput.TabIndex = 15;
            // 
            // SpinLabel
            // 
            this.SpinLabel.AutoSize = true;
            this.SpinLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SpinLabel.Location = new System.Drawing.Point(17, 326);
            this.SpinLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.SpinLabel.Name = "SpinLabel";
            this.SpinLabel.Size = new System.Drawing.Size(45, 20);
            this.SpinLabel.TabIndex = 16;
            this.SpinLabel.Text = "Spin:";
            // 
            // SpinInput
            // 
            this.SpinInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SpinInput.Location = new System.Drawing.Point(138, 326);
            this.SpinInput.Margin = new System.Windows.Forms.Padding(2);
            this.SpinInput.Name = "SpinInput";
            this.SpinInput.Size = new System.Drawing.Size(86, 26);
            this.SpinInput.TabIndex = 17;
            // 
            // CurveLabel
            // 
            this.CurveLabel.AutoSize = true;
            this.CurveLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurveLabel.Location = new System.Drawing.Point(18, 361);
            this.CurveLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.CurveLabel.Name = "CurveLabel";
            this.CurveLabel.Size = new System.Drawing.Size(54, 20);
            this.CurveLabel.TabIndex = 18;
            this.CurveLabel.Text = "Curve:";
            // 
            // CurveInput
            // 
            this.CurveInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurveInput.Location = new System.Drawing.Point(138, 359);
            this.CurveInput.Margin = new System.Windows.Forms.Padding(2);
            this.CurveInput.Name = "CurveInput";
            this.CurveInput.Size = new System.Drawing.Size(86, 26);
            this.CurveInput.TabIndex = 19;
            // 
            // SlopeBreakLabel
            // 
            this.SlopeBreakLabel.AutoSize = true;
            this.SlopeBreakLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SlopeBreakLabel.Location = new System.Drawing.Point(18, 390);
            this.SlopeBreakLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.SlopeBreakLabel.Name = "SlopeBreakLabel";
            this.SlopeBreakLabel.Size = new System.Drawing.Size(100, 20);
            this.SlopeBreakLabel.TabIndex = 20;
            this.SlopeBreakLabel.Text = "Slope Break:";
            // 
            // SlopeBreakInput
            // 
            this.SlopeBreakInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SlopeBreakInput.Location = new System.Drawing.Point(138, 392);
            this.SlopeBreakInput.Margin = new System.Windows.Forms.Padding(2);
            this.SlopeBreakInput.Name = "SlopeBreakInput";
            this.SlopeBreakInput.Size = new System.Drawing.Size(86, 26);
            this.SlopeBreakInput.TabIndex = 21;
            // 
            // PangYa
            // 
            this.PangYa.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.PangYa.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PangYa.Location = new System.Drawing.Point(607, 470);
            this.PangYa.Margin = new System.Windows.Forms.Padding(2);
            this.PangYa.Name = "PangYa";
            this.PangYa.Size = new System.Drawing.Size(93, 39);
            this.PangYa.TabIndex = 22;
            this.PangYa.Text = "PangYa";
            this.PangYa.UseVisualStyleBackColor = false;
            this.PangYa.Click += new System.EventHandler(this.PangYaButton_Click);
            // 
            // WidthInput
            // 
            this.WidthInput.Location = new System.Drawing.Point(0, 0);
            this.WidthInput.Name = "WidthInput";
            this.WidthInput.Size = new System.Drawing.Size(100, 20);
            this.WidthInput.TabIndex = 0;
            // 
            // HeightResolutionInput
            // 
            this.HeightResolutionInput.Location = new System.Drawing.Point(0, 0);
            this.HeightResolutionInput.Name = "HeightResolutionInput";
            this.HeightResolutionInput.Size = new System.Drawing.Size(100, 20);
            this.HeightResolutionInput.TabIndex = 0;
            // 
            // PowerInput
            // 
            this.PowerInput.Location = new System.Drawing.Point(0, 0);
            this.PowerInput.Name = "PowerInput";
            this.PowerInput.Size = new System.Drawing.Size(100, 20);
            this.PowerInput.TabIndex = 0;
            // 
            // RingPowerInput
            // 
            this.RingPowerInput.Location = new System.Drawing.Point(0, 0);
            this.RingPowerInput.Name = "RingPowerInput";
            this.RingPowerInput.Size = new System.Drawing.Size(100, 20);
            this.RingPowerInput.TabIndex = 0;
            // 
            // CardPowerInput
            // 
            this.CardPowerInput.Location = new System.Drawing.Point(0, 0);
            this.CardPowerInput.Name = "CardPowerInput";
            this.CardPowerInput.Size = new System.Drawing.Size(100, 20);
            this.CardPowerInput.TabIndex = 0;
            // 
            // MascotPowerInput
            // 
            this.MascotPowerInput.Location = new System.Drawing.Point(0, 0);
            this.MascotPowerInput.Name = "MascotPowerInput";
            this.MascotPowerInput.Size = new System.Drawing.Size(100, 20);
            this.MascotPowerInput.TabIndex = 0;
            // 
            // CardPowerShotPowerInput
            // 
            this.CardPowerShotPowerInput.Location = new System.Drawing.Point(0, 0);
            this.CardPowerShotPowerInput.Name = "CardPowerShotPowerInput";
            this.CardPowerShotPowerInput.Size = new System.Drawing.Size(100, 20);
            this.CardPowerShotPowerInput.TabIndex = 0;
            // 
            // AIM
            // 
            this.AIM.AutoSize = true;
            this.AIM.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AIM.Location = new System.Drawing.Point(15, 477);
            this.AIM.Name = "AIM";
            this.AIM.Size = new System.Drawing.Size(43, 24);
            this.AIM.TabIndex = 23;
            this.AIM.Text = "AIM";
            // 
            // Caliper
            // 
            this.Caliper.AutoSize = true;
            this.Caliper.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Caliper.Location = new System.Drawing.Point(263, 477);
            this.Caliper.Name = "Caliper";
            this.Caliper.Size = new System.Drawing.Size(69, 24);
            this.Caliper.TabIndex = 24;
            this.Caliper.Text = "Caliper";
            // 
            // calculationWebView
            // 
            this.calculationWebView.AllowExternalDrop = true;
            this.calculationWebView.CreationProperties = null;
            this.calculationWebView.DefaultBackgroundColor = System.Drawing.Color.White;
            this.calculationWebView.Location = new System.Drawing.Point(168, 11);
            this.calculationWebView.Margin = new System.Windows.Forms.Padding(2);
            this.calculationWebView.Name = "calculationWebView";
            this.calculationWebView.Size = new System.Drawing.Size(88, 36);
            this.calculationWebView.TabIndex = 23;
            this.calculationWebView.Visible = false;
            this.calculationWebView.ZoomFactor = 1D;
            // 
            // Reset
            // 
            this.Reset.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Reset.Location = new System.Drawing.Point(518, 470);
            this.Reset.Name = "Reset";
            this.Reset.Size = new System.Drawing.Size(84, 39);
            this.Reset.TabIndex = 25;
            this.Reset.Text = "Reset";
            this.Reset.UseVisualStyleBackColor = true;
            this.Reset.Click += new System.EventHandler(this.Reset_Click);
            // 
            // WindCapture
            // 
            this.WindCapture.Location = new System.Drawing.Point(250, 12);
            this.WindCapture.Name = "WindCapture";
            this.WindCapture.Size = new System.Drawing.Size(400, 400);
            this.WindCapture.TabIndex = 26;
            this.WindCapture.TabStop = false;
            this.WindCapture.Visible = false;
            // 
            // F11
            // 
            this.F11.AutoSize = true;
            this.F11.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.F11.Location = new System.Drawing.Point(337, 415);
            this.F11.Name = "F11";
            this.F11.Size = new System.Drawing.Size(213, 24);
            this.F11.TabIndex = 27;
            this.F11.Text = "F11 here for Screenshot";
            // 
            // StayOnTop
            // 
            this.StayOnTop.AutoSize = true;
            this.StayOnTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StayOnTop.Location = new System.Drawing.Point(162, 432);
            this.StayOnTop.Name = "StayOnTop";
            this.StayOnTop.Size = new System.Drawing.Size(113, 24);
            this.StayOnTop.TabIndex = 28;
            this.StayOnTop.Text = "Stay on Top";
            this.StayOnTop.UseVisualStyleBackColor = true;
            this.StayOnTop.CheckedChanged += new System.EventHandler(this.StayOnTop_CheckedChanged);
            // 
            // pinSelectionButton
            // 
            this.pinSelectionButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pinSelectionButton.Location = new System.Drawing.Point(18, 15);
            this.pinSelectionButton.Name = "pinSelectionButton";
            this.pinSelectionButton.Size = new System.Drawing.Size(145, 33);
            this.pinSelectionButton.TabIndex = 29;
            this.pinSelectionButton.Text = "Select Courses";
            this.pinSelectionButton.UseVisualStyleBackColor = true;
            this.pinSelectionButton.Click += new System.EventHandler(this.pinSelectionBotton_Click);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(657, 12);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(43, 25);
            this.btnUp.TabIndex = 30;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(657, 44);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(43, 22);
            this.btnDown.TabIndex = 31;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(651, 196);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(49, 34);
            this.Save.TabIndex = 32;
            this.Save.Text = "Save SS";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(657, 72);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(43, 23);
            this.btnLeft.TabIndex = 33;
            this.btnLeft.Text = "<-";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(657, 102);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(43, 23);
            this.btnRight.TabIndex = 34;
            this.btnRight.Text = "->";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // zoomIn
            // 
            this.zoomIn.Location = new System.Drawing.Point(657, 131);
            this.zoomIn.Name = "zoomIn";
            this.zoomIn.Size = new System.Drawing.Size(43, 23);
            this.zoomIn.TabIndex = 35;
            this.zoomIn.Text = "+";
            this.zoomIn.UseVisualStyleBackColor = true;
            this.zoomIn.Click += new System.EventHandler(this.zoomIn_Click);
            // 
            // zoomOut
            // 
            this.zoomOut.Location = new System.Drawing.Point(657, 155);
            this.zoomOut.Name = "zoomOut";
            this.zoomOut.Size = new System.Drawing.Size(43, 23);
            this.zoomOut.TabIndex = 36;
            this.zoomOut.Text = "-";
            this.zoomOut.UseVisualStyleBackColor = true;
            this.zoomOut.Click += new System.EventHandler(this.zoomOut_Click);
            // 
            // CalculateOptimalSpin
            // 
            this.CalculateOptimalSpin.AutoSize = true;
            this.CalculateOptimalSpin.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CalculateOptimalSpin.Location = new System.Drawing.Point(21, 432);
            this.CalculateOptimalSpin.Name = "CalculateOptimalSpin";
            this.CalculateOptimalSpin.Size = new System.Drawing.Size(118, 24);
            this.CalculateOptimalSpin.TabIndex = 37;
            this.CalculateOptimalSpin.Text = "Optimal Spin";
            this.CalculateOptimalSpin.UseVisualStyleBackColor = true;
            
            // 
            // PangYaC
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(703, 510);
            this.Controls.Add(this.CalculateOptimalSpin);
            this.Controls.Add(this.zoomOut);
            this.Controls.Add(this.zoomIn);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.pinSelectionButton);
            this.Controls.Add(this.StayOnTop);
            this.Controls.Add(this.WindCapture);
            this.Controls.Add(this.Reset);
            this.Controls.Add(this.Caliper);
            this.Controls.Add(this.AIM);
            this.Controls.Add(this.ClubLabel);
            this.Controls.Add(this.ClubDropdown);
            this.Controls.Add(this.ShotLabel);
            this.Controls.Add(this.ShotDropdown);
            this.Controls.Add(this.PowerShotLabel);
            this.Controls.Add(this.PowerShotDropdown);
            this.Controls.Add(this.DistanceLabel);
            this.Controls.Add(this.DistanceInput);
            this.Controls.Add(this.HeightLabel);
            this.Controls.Add(this.HeightInput);
            this.Controls.Add(this.WindLabel);
            this.Controls.Add(this.WindInput);
            this.Controls.Add(this.DegreeLabel);
            this.Controls.Add(this.DegreeInput);
            this.Controls.Add(this.GroundLabel);
            this.Controls.Add(this.GroundInput);
            this.Controls.Add(this.SpinLabel);
            this.Controls.Add(this.SpinInput);
            this.Controls.Add(this.CurveLabel);
            this.Controls.Add(this.CurveInput);
            this.Controls.Add(this.SlopeBreakLabel);
            this.Controls.Add(this.SlopeBreakInput);
            this.Controls.Add(this.PangYa);
            this.Controls.Add(this.calculationWebView);
            this.Controls.Add(this.F11);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "PangYaC";
            this.Text = "PangYaC";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.calculationWebView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WindCapture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        //Listener
        private void StartPipeListener()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("AnglePipe", PipeDirection.In))
                    {
                        Console.WriteLine("Waiting for a connection...");
                        pipeServer.WaitForConnection();

                        try
                        {
                            using (StreamReader reader = new StreamReader(pipeServer, Encoding.UTF8))
                            {
                                string receivedData = reader.ReadToEnd();
                                

                                // Update the degree input and ensure WindInput gets focus
                                UpdateDegreeInput(receivedData);

                                // Add a small delay before focusing
                                Thread.Sleep(100);
                                
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                    }
                }
            });
        }



        private void UpdateDegreeInput(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    DegreeInput.Text = text;

                    // Bring the window to the front
                    this.BringToFront();
                    this.Activate();

                    // Focus the WindInput
                    WindInput.Focus();
                    WindInput.SelectAll();
                    this.ActiveControl = WindInput;
                }));
            }
            else
            {
                DegreeInput.Text = text;
                this.BringToFront();
                this.Activate();
                WindInput.Focus();
                WindInput.SelectAll();
                this.ActiveControl = WindInput;
            }
        }


        //Closes the WindWindow
        private void CloseWindWindow()
        {
            foreach (var process in Process.GetProcessesByName("WindWindow"))
            {
                process.Kill();
            }
        }

        // Open the WindWindow application
        private void OpenWindWindow()
        {
            // Construct the path to the resource folder
            string resourceFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
            string windWindowPath = System.IO.Path.Combine(resourceFolderPath, "WindWindow.exe");

            try
            {
                Process windWindowProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = windWindowPath,
                        UseShellExecute = false
                    }
                };

                windWindowProcess.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to launch WindWindow: {ex.Message}");
            }
        }

        // Helper method to check if a process is running
        private bool IsProcessRunning(string processName)
        {
            // Get all processes with the specified name
            var processes = System.Diagnostics.Process.GetProcessesByName(processName);

            // Return true if any instances are found
            return processes.Length > 0;
        }



        //Captures F11 keystroke
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F11)
            {

                // Make the PictureBox visible
                this.WindCapture.Visible = true;
                CaptureScreenArea("Pangya Reborn!");


                // Check if WindWindow.exe is already running
                if (!IsProcessRunning("WindWindow"))
                {
                    OpenWindWindow();
                }
            }
        }

        private void StayOnTop_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = StayOnTop.Checked;
        }

       

        
        //Captures the screen area
        public void CaptureScreenArea(string windowTitle)
        {
            // Define a dictionary for capture settings based on resolution
            var resolutionSettings = new Dictionary<(int width, int height), (int x, int y, int captureWidth, int captureHeight)>
            {
                // Add more resolutions and their capture settings here
                { (2576, 1460), (2295, 1159, 255, 255) },// Example for 2560x1440, 255x255 capture area, top-left corner at x=2287, y=1159
                { (2560, 1440), (2288, 1144, 255, 255) },
                { (1280, 960), (1720, 880, 200, 200) },
                {(1296, 999), (1720, 880, 200, 200) }

            };

            try
            {
                // Find the game window
                IntPtr hWnd = FindWindow(null, windowTitle);
                if (hWnd == IntPtr.Zero) throw new Exception("Game window not found.");

                // Get the window dimensions
                GetWindowRect(hWnd, out RECT windowRect);
                int windowWidth = windowRect.Right - windowRect.Left;
                int windowHeight = windowRect.Bottom - windowRect.Top;

                // Calculate the center point for capture (93.95% of width, 87.95% of height)
                int centerX = (int)(windowWidth * 0.9395);
                int centerY = (int)(windowHeight * 0.8795);

                // Define the capture size (1/7th of the smaller window dimension)
                int baseSize = Math.Min(windowWidth, windowHeight) / 7;
                int captureSize = baseSize + zoom;  // Adjust size based on zoom

                // Apply both offsets
                int captureX = (centerX - (captureSize / 2)) + offsetX;
                int captureY = (centerY - (captureSize / 2)) + offsetY;

                captureX = Math.Max(0, Math.Min(captureX, windowWidth - captureSize));
                captureY = Math.Max(0, Math.Min(captureY, windowHeight - captureSize));

                // Dispose of previous image
                if (WindCapture.Image != null)
                {
                    var oldImage = WindCapture.Image;
                    WindCapture.Image = null;
                    oldImage.Dispose();
                }

                // Take the screenshot
                using (Bitmap fullScreenshot = new Bitmap(windowWidth, windowHeight))
                {
                    using (Graphics g = Graphics.FromImage(fullScreenshot))
                    {
                        g.CopyFromScreen(
                            windowRect.Left, windowRect.Top,
                            0, 0,
                            new Size(windowWidth, windowHeight)
                        );

                    }
                    // Crop to our target area
                    Rectangle cropArea = new Rectangle(captureX, captureY, captureSize, captureSize);



                    WindCapture.Image = fullScreenshot.Clone(cropArea, fullScreenshot.PixelFormat);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error capturing screen area: {ex.Message}");
            }
        }

  

        // Start Overlay
        private void StartLineOverlay()
        {
            try
            {
                // Check if the process is already running
                Process[] processes = Process.GetProcessesByName("LineOverlay");
                if (processes.Length > 0)
                {
                    // Optional: Bring existing window to front
                    // processes[0].MainWindowHandle can be used here if needed
                    return;
                }

                string overlayPath = @"Resources\LineOverlay.exe"; // Adjust the path if needed
                Process.Start(overlayPath); // Launch the WPF application
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start LineOverlay.exe: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (!isExiting) // Check if already exiting
            {
                calculationWebView.Dispose();
                calculationWebView = null;
                isExiting = true; // Prevent re-entry

                Process[] processes = Process.GetProcessesByName("LineOverlay");
                if (processes.Length > 0)
                {
                    SignalWPFAppToClose(); // Notify WPF app to shut down
                    CloseWindWindow();

                }

                // Allow the application to close without further execution
                System.Windows.Forms.Application.ExitThread(); // Closes Forms application gracefully
            }


        }

        
        //Closes the WPF app
        private void SignalWPFAppToClose()
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "LineOverlayPipe", PipeDirection.Out))
            {
                pipeClient.Connect();
                using (StreamWriter writer = new StreamWriter(pipeClient))
                {
                    writer.WriteLine("shutdown");
                }
            }

        }

        // This method is called when the form is loaded
        private async void InputValues_Load(object sender, EventArgs e)
        {

            StartLineOverlay();

            WindCapture.SizeMode = PictureBoxSizeMode.Zoom;

            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Safely populate the UI fields after the form has loaded
            if (_inputData != null)
            {
                // Populate fields or logic with the received data
                WidthInput.Text = _inputData.Width;
                HeightResolutionInput.Text = _inputData.HeightResolution;
                PowerInput.Text = _inputData.Power;
                RingPowerInput.Text = _inputData.RingPower;
                CardPowerInput.Text = _inputData.CardPower;
                MascotPowerInput.Text = _inputData.MascotPower;
                CardPowerShotPowerInput.Text = _inputData.CardPowerShotPower;
                // Set default inputData for combo boxes or text boxes
                ClubDropdown.SelectedIndex = 0; // Set default selection for Club dropdown
                ShotDropdown.SelectedIndex = 0;  // Set default selection for Shot dropdown
                PowerShotDropdown.SelectedIndex = 0;  // Set default selection for Power Shot dropdown
                //DistanceInput.Text = "0";
                HeightInput.Text = "0";
                WindInput.Text = "0";
                DegreeInput.Text = "0";
                GroundInput.Text = "100";
                SlopeBreakInput.Text = "0";
                SpinInput.Text = "30";
                CurveInput.Text = "0";


            }
            else
            {
                MessageBox.Show("No data was passed to _inputData", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            // Save default values of all TextBoxes
            foreach (Control control in this.Controls)
            {
                if (control is TextBox textBox)
                {
                    defaultValues[textBox.Name] = textBox.Text;
                }
                else if (control is ComboBox comboBox)
                {
                    defaultValues[comboBox.Name] = comboBox.SelectedItem?.ToString() ?? "";
                }
            }


            LoadOffsets();

            await InitializeWebView();

        }


        // This method Initializes WevView2
        private async Task InitializeWebView()
        {
            try
            {
                // Then initialize CoreWebView2
                await calculationWebView.EnsureCoreWebView2Async();

                // Setting the source to a local HTML file
                string htmlPath = Path.Combine(System.Windows.Forms.Application.StartupPath, "index.htm");
                if (File.Exists(htmlPath))
                {
                    calculationWebView.Source = new Uri(htmlPath);

                }
                else
                {
                    MessageBox.Show("HTML file not found!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing WebView2: {ex.Message}");
            }
        }


        private async Task UpdateValues(dynamic values)
        {
            string ScriptResult = "";
            try
            {
                string script = $"document.getElementById('rel-width').value = {values.WidthInput}; " +
                                $"document.getElementById('rel-height').value = {values.HeightResolutionInput}; " +
                                $"document.getElementById('power').value = {values.PowerInput}; " +
                                $"document.getElementById('auxpart_pwr').value = {values.RingPowerInput}; " +
                                $"document.getElementById('card_pwr').value = {values.CardPowerInput}; " +
                                $"document.getElementById('mascot_pwr').value = {values.MascotPowerInput}; " +
                                $"document.getElementById('card_ps_pwr').value = {values.CardPowerShotPowerInput}; " +
                                $"document.getElementById('club').value = {values.ClubDropdown}; " +
                                $"document.getElementById('shot').value = {values.ShotDropdown}; " +
                                $"document.getElementById('power_shot').value = {values.PowerShotDropdown}; " +
                                $"document.getElementById('distance').value = {values.DistanceInput}; " +
                                $"document.getElementById('height').value = {values.HeightInput}; " +
                                $"document.getElementById('wind').value = {values.WindInput}; " +
                                $"document.getElementById('degree').value = {values.DegreeInput}; " +
                                $"document.getElementById('ground').value = {values.GroundInput}; " +
                                $"document.getElementById('spin').value = {values.SpinInput}; " +
                                $"document.getElementById('curve').value = {values.CurveInput}; " +
                                $"document.getElementById('slope_break').value = {values.SlopeBreakInput}; " +
                                $"calc();";

                // Execute the script in the WebView
                ScriptResult = await calculationWebView.ExecuteScriptAsync(script);

                //Clean up and Parse the ScriptResult
                ScriptResult = ScriptResult.Trim('"'); // Remove potential JSON quotes around the ScriptResult




            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating values: {ex.Message}");

            }

            

            Finalresult = ScriptResult;
            Console.WriteLine($"Finalresult is: {Finalresult}");
            SetCaliper(Finalresult, values);

            // IAM Value example would be 1.2 or -1.2 
            string AIMValue = ReturnAimValue(Finalresult);

            SendToPipe(AIMValue, values);
        }


        // This method is sends the AIM value to the named pipe
        private void SendToPipe(string aimValue, dynamic values)
        {


            try
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "AIMPipe", PipeDirection.Out))
                {
                    pipeClient.Connect();
                    using (StreamWriter writer = new StreamWriter(pipeClient))
                    {

                        if (aimValue != "reset")
                        {
                            writer.WriteLine(aimValue.ToString());
                            writer.Write($"{values.SpinInput},{values.CurveInput}");
                            writer.Write("\n");
                            writer.Write(Caliper.Text);
                            writer.Write("\n");
                            writer.Write(values.ShotDropdown);
                            writer.Write("\n");
                            writer.Write(values.PowerShotDropdown);
                            writer.Write("\n");
                            writer.Write(values.ClubDropdown);
                            writer.Write("\n");
                        }
                        else
                        {
                            writer.WriteLine(aimValue.ToString());
                        }


                        writer.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send AIM value: {ex.Message}");
            }
            CloseWindWindow();
        }


        // This method is called when the 'PangYa' button is clicked
        private async void PangYaButton_Click(object sender, EventArgs e)
        {

            try
            {
                PangYa.Enabled = false; // Disable button during execution

                // Validate inputs before proceeding
                if (!ValidateInputs())
                {
                    MessageBox.Show("Please check your inputs and try again.", "Validation Error");
                    return;
                }

                var inputData = new
                {
                    WidthInput = WidthInput.Text.Trim(),
                    HeightResolutionInput = HeightResolutionInput.Text.Trim(),
                    PowerInput = PowerInput.Text.Trim(),
                    RingPowerInput = RingPowerInput.Text.Trim(),
                    CardPowerInput = CardPowerInput.Text.Trim(),
                    MascotPowerInput = MascotPowerInput.Text.Trim(),
                    CardPowerShotPowerInput = CardPowerShotPowerInput.Text.Trim(),
                    ShotDropdown = ShotDropdown.SelectedIndex.ToString(),
                    PowerShotDropdown = PowerShotDropdown.SelectedIndex.ToString(),
                    DistanceInput = DistanceInput.Text.Trim(),
                    HeightInput = HeightInput.Text.Trim(),
                    WindInput = WindInput.Text.Trim(),
                    DegreeInput = DegreeInput.Text.Trim(),
                    GroundInput = GroundInput.Text.Trim(),
                    SpinInput = SpinInput.Text.Trim(),
                    CurveInput = CurveInput.Text.Trim(),
                    SlopeBreakInput = SlopeBreakInput.Text.Trim(),
                    ClubDropdown = ClubDropdown.SelectedIndex.ToString()
                };

                if (CalculateOptimalSpin.Checked)
                {
                    await FindOptimalSpin(inputData);
                }
                else
                {
                    await UpdateValues(inputData);
                }

                // Initialize WebView if not already done
                if (calculationWebView == null)
                {
                    await InitializeWebView();
                }

                SetupWebViewMessageHandler();

                // Refresh the page after it loads
                await Task.Delay(100); // Optional: Delay to ensure loading completes

                Process[] processes = Process.GetProcessesByName("WindWindow");
                if (processes.Length > 0)
                {
                    CloseWindWindow();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during calculation: {ex.Message}", "Error");
                System.Diagnostics.Debug.WriteLine($"Error: {ex}");
            }
            finally
            {
                PangYa.Enabled = true; // Re-enable button after execution
            }

        }

        

        // This method updates the Caliper label
        private void SetCaliper(string finalResult, dynamic values)
        {
            if (string.IsNullOrWhiteSpace(finalResult))
            {
                Caliper.Text = "No valid data found";
                return;
            }

            // Split the string by commas and trim any whitespace
            var parts = finalResult.Split(',')
                                   .Select(part => part.Trim())
                                   .ToList();

            // Ensure there are at least two parts to extract
            if (parts.Count >= 2)
            {
                string percentageValue = parts[0]; // First value (e.g., "18.7%")
                string FinalCaliper = GetCaliper(parts[1], values); ;  // Second value (e.g., "83.9y")


                // Update the RawCaliper label with the extracted values
                Caliper.Text = $"{FinalCaliper}y, {percentageValue}";

            }
            else
            {
                Caliper.Text = "Power is too weak!";
            }
        }



        // This method updates the ScriptResult
        private bool ValidateInputs()
        {
            // Add validation logic for your inputs
            if (string.IsNullOrWhiteSpace(WidthInput.Text) ||
                string.IsNullOrWhiteSpace(HeightInput.Text))
            {
                return false;
            }

            // Add more validation as needed
            return true;
        }

        // This set up the WebView message handler
        private void SetupWebViewMessageHandler()
        {


            calculationWebView.CoreWebView2.WebMessageReceived += (sender, args) =>
            {
                var resultObject = JsonConvert.DeserializeObject<dynamic>(args.WebMessageAsJson);
                var calculationResult = resultObject.result;
                AIM.Text = calculationResult?.ToString() ?? "No Result";
                Console.WriteLine($"Calculation DOES THIS EVER WORK? ScriptResult: {calculationResult}");
            };
        }

        // This method sends the AIM value to the main form
        private string ReturnAimValue(string Aim)
        {
            // Ensure you are working with the latest value of the Finalresult string
            string originalText = Aim;

            // Split the text into parts and look for the part containing "AIM"
            var parts = originalText.Split(',')
                                    .Select(part => part.Trim())
                                    .ToList();

            var aimPart = parts.FirstOrDefault(part => part.Contains("AIM"));

            // Update the label with the extracted value or a default message
            AIM.Text = aimPart ?? "AIM data not found";

            Match match = Regex.Match(aimPart, @"\(([^)]*)\)");
            if (match.Success)
            {
                string extractedNumber = Regex.Match(aimPart, @"\((-?[0-9.]+)").Groups[1].Value; // Returns both "2.1" and "-2.1"

                return extractedNumber;
            }

            return Aim = "0"; // Return 0 as it was not found
        }

        // This method generates a list of percentages
        public static string GetCaliper(string ReferenceCaliper, dynamic values)
        {

            string returnCaliper = "";

            // Define constants
            double maxDrive = CalculateDrive(values); // Maximum drive or player max distance
            double step = 0.27777777777778; // Percentage decrement step

            // Generate the list of percentages from 100.00 to 0.00
            List<double> percentages = GeneratePercentages(100.00, 0.00, step);
            List<double> CaliperList = new List<double>();

            foreach (var percentage in percentages)
            {
                double RawCaliper = GetNearCaliper(maxDrive, percentage);
                double decimalPart = RawCaliper - Math.Floor(RawCaliper);

                var ranges = new (double Min, double Max, Func<double, double> RoundingFunction)[]
                {
                   (0.0499, 0.05, x => Math.Ceiling(x * 10) / 10),
                   (0.15, 0.1500000006, x => Math.Floor(x * 10) / 10),
                   (0.2499, 0.25, x => Math.Ceiling(x * 10) / 10),
                   (0.3499, 0.35, x => Math.Ceiling(x * 10) / 10),
                   (0.4499, 0.45, x => Math.Ceiling(x * 10) / 10),
                   (0.5499, 0.55, x => Math.Ceiling(x * 10) / 10),
                   (0.6499, 0.65, x => Math.Ceiling(x * 10) / 10),
                   (0.7499, 0.75, x => Math.Ceiling(x * 10) / 10),
                   (0.8499, 0.85, x => Math.Ceiling(x * 10) / 10),
                   (0.9499, 0.95, x => Math.Ceiling(x * 10) / 10)
                };



                // Check if the decimalPart falls into any range
                bool rangeMatched = false;
                foreach (var range in ranges)
                {
                    if (decimalPart > range.Min && decimalPart < range.Max)
                    {
                        double fixedCaliper = range.RoundingFunction(RawCaliper);
                        CaliperList.Add(fixedCaliper);
                        rangeMatched = true;//adding this

                        break;
                    }

                }

                // If no range matched, apply default logic //Almost all cases will fall into this
                if (!rangeMatched)
                {
                    RawCaliper = Math.Round(RawCaliper, 2, MidpointRounding.AwayFromZero);
                    //remember to use :F1 later 
                    CaliperList.Add(RawCaliper);
                }
            }

            // Clean up reference caliper and find closest match
            ReferenceCaliper = ReferenceCaliper.TrimEnd('y');
            Console.WriteLine($"ReferenceCaliper is:{ReferenceCaliper}");

            // Final validation check
            if (double.TryParse(ReferenceCaliper, out double refCaliper))
            {
                if (refCaliper > maxDrive)
                {
                    return "Short!";
                }
                
            }



            double unfixedCaliper = CaliperList.OrderBy(n => Math.Abs(n - Convert.ToDouble(ReferenceCaliper))).First();
            returnCaliper = $"{unfixedCaliper:F1}";
            Console.WriteLine($"ReturnCaliper is: {returnCaliper}");
            return returnCaliper;


        }


        static double GetNearCaliper(double maxDrive, double percentage)
        {
            // Calculate the caliper value based on percentage
            return maxDrive * (percentage / 100);
        }

        static List<double> GeneratePercentages(double start, double end, double step)
        {
            List<double> percentages = new List<double>();
            for (double value = start; value >= end; value -= step)
            {
                percentages.Add(value);
            }
            return percentages;
        }


        private static double CalculateDrive(dynamic values)
        {
            int basepower;
            double drive = 0;

            //safely parse the input values
            double ringPower = double.TryParse(values.RingPowerInput, out double rp) ? rp : 0;
            double cardPower = double.TryParse(values.CardPowerInput, out double cp) ? cp : 0;
            double mascotPower = double.TryParse(values.MascotPowerInput, out double mp) ? mp : 0;
            double PowerShotGauge = double.TryParse(values.PowerShotDropdown, out double ap) ? ap : 0;
            double cardPowerShotPower = double.TryParse(values.CardPowerShotPowerInput, out double cpsp) ? cpsp : 0;

            if (PowerShotGauge == 0)
            {
                PowerShotGauge = 0;
                cardPowerShotPower = 0;
            }
            else if (PowerShotGauge == 1) { PowerShotGauge = 10; }
            else if (PowerShotGauge == 2) { PowerShotGauge = 20; }
            else if (PowerShotGauge == 1.5)
            {
                PowerShotGauge = 15;
            }

            // Calculate the base power

            //No PowerShot
            switch (values.ClubDropdown)
            {
                case "0":
                    //1W
                    basepower = 200;
                    drive = basepower + PowerShotGauge + ringPower + cardPower + mascotPower + cardPowerShotPower + (Convert.ToDouble(values.PowerInput) * 2);
                    Console.WriteLine($"Drive is: {drive}");

                    break;
                case "1":
                    //2W
                    basepower = 180;
                    drive = basepower + PowerShotGauge + ringPower + cardPower + mascotPower + cardPowerShotPower + (Convert.ToDouble(values.PowerInput) * 2);
                    Console.WriteLine($"Drive is: {drive}");
                    break;
                case "2":
                    //3W
                    basepower = 160;
                    drive = basepower + PowerShotGauge + ringPower + cardPower + mascotPower + cardPowerShotPower + (Convert.ToDouble(values.PowerInput) * 2);
                    Console.WriteLine($"Drive is: {drive}");
                    break;
                case "3":
                    //2i
                    basepower = 180;
                    drive = basepower + ringPower + cardPower + mascotPower + PowerShotGauge + cardPowerShotPower;
                    break;
                case "4":
                    //3i
                    basepower = 170;
                    drive = basepower + PowerShotGauge + ringPower + cardPower + mascotPower + cardPowerShotPower;
                    Console.WriteLine($"Drive is: {drive}");
                    break;
                case "5":
                    //4i
                    basepower = 160;
                    drive = basepower + PowerShotGauge + ringPower + cardPower + mascotPower + cardPowerShotPower;
                    break;
                case "6":
                    //5i
                    basepower = 150;
                    drive = basepower + PowerShotGauge + ringPower + cardPower + mascotPower + cardPowerShotPower;
                    break;
                case "7":
                    //6i
                    basepower = 140;
                    drive = basepower + PowerShotGauge + ringPower + cardPower + mascotPower + cardPowerShotPower;
                    break;
                case "8":
                    //7i
                    basepower = 130;
                    drive = basepower + PowerShotGauge + ringPower + cardPower + mascotPower + cardPowerShotPower;
                    break;
                case "9":
                    //8i
                    basepower = 120;
                    drive = basepower + PowerShotGauge + ringPower + cardPower + mascotPower + cardPowerShotPower;
                    break;
                case "10":
                    //9i
                    basepower = 110;
                    drive = basepower + PowerShotGauge + ringPower + cardPower + mascotPower + cardPowerShotPower;
                    break;
                case "11":
                    //PW
                    basepower = 100;
                    drive = basepower + PowerShotGauge + ringPower + cardPower + mascotPower + cardPowerShotPower;
                    break;
                case "12":
                    //SW
                    basepower = 90;
                    drive = basepower + PowerShotGauge + ringPower + cardPower + mascotPower + cardPowerShotPower;
                    break;
                default:

                    break;
            }
            return drive;

        }


        private async Task FindOptimalSpin(dynamic inputData)
        {
            // Calculate drive power for this shot
            double drive = CalculateDrive(inputData);

            // Get all possible calipers that exist in the game for this drive
            List<double> validGameCalipers = GenerateAllPossibleCalipers(drive);

            // Start from highest spin
            int maxSpin = int.Parse(SpinInput.Text);

            // Iterate from highest to lowest spin
            for (int spin = maxSpin; spin >= 0; spin--)
            {
                // Construct the full script with all values
                string script = $"document.getElementById('rel-width').value = {inputData.WidthInput}; " +
                               $"document.getElementById('rel-height').value = {inputData.HeightResolutionInput}; " +
                               $"document.getElementById('power').value = {inputData.PowerInput}; " +
                               $"document.getElementById('auxpart_pwr').value = {inputData.RingPowerInput}; " +
                               $"document.getElementById('card_pwr').value = {inputData.CardPowerInput}; " +
                               $"document.getElementById('mascot_pwr').value = {inputData.MascotPowerInput}; " +
                               $"document.getElementById('card_ps_pwr').value = {inputData.CardPowerShotPowerInput}; " +
                               $"document.getElementById('club').value = {inputData.ClubDropdown}; " +
                               $"document.getElementById('shot').value = {inputData.ShotDropdown}; " +
                               $"document.getElementById('power_shot').value = {inputData.PowerShotDropdown}; " +
                               $"document.getElementById('distance').value = {inputData.DistanceInput}; " +
                               $"document.getElementById('height').value = {inputData.HeightInput}; " +
                               $"document.getElementById('wind').value = {inputData.WindInput}; " +
                               $"document.getElementById('degree').value = {inputData.DegreeInput}; " +
                               $"document.getElementById('ground').value = {inputData.GroundInput}; " +
                               $"document.getElementById('spin').value = {spin}; " +
                               $"document.getElementById('curve').value = {inputData.CurveInput}; " +
                               $"document.getElementById('slope_break').value = {inputData.SlopeBreakInput}; " +
                               $"calc();";

                string result = await calculationWebView.ExecuteScriptAsync(script);

                result = result.Trim('"');
                var testParts = result.Split(',').Select(p => p.Trim()).ToList();
                if (testParts.Count < 2) continue;

                string testCaliper = testParts[1].TrimEnd('y');
                if (!double.TryParse(testCaliper, out double calculatedCaliper))
                    continue;


                Console.WriteLine($"\nTesting spin {spin}, calculated caliper: {calculatedCaliper:F2}");
                
                // Check each valid game caliper
                foreach (double gameCaliper in validGameCalipers)
                {
                    double difference = Math.Abs(gameCaliper - calculatedCaliper);
                    Console.WriteLine($"Comparing with game caliper {gameCaliper:F2}, difference: {difference:F3}");

                    // If we find a close enough match, stop checking other calipers
                    if (difference <= 0.1)
                    {
                        Console.WriteLine($"Found match: Spin {spin}, Calculated {calculatedCaliper:F2}, Game Caliper {gameCaliper:F2}, Difference {difference:F3}");

                        // Update the spin input
                        SpinInput.Text = spin.ToString();

                        // Create new input data with the updated spin
                        var updatedInputData = new
                        {
                            WidthInput = inputData.WidthInput,
                            HeightResolutionInput = inputData.HeightResolutionInput,
                            PowerInput = inputData.PowerInput,
                            RingPowerInput = inputData.RingPowerInput,
                            CardPowerInput = inputData.CardPowerInput,
                            MascotPowerInput = inputData.MascotPowerInput,
                            CardPowerShotPowerInput = inputData.CardPowerShotPowerInput,
                            ClubDropdown = inputData.ClubDropdown,
                            ShotDropdown = inputData.ShotDropdown,
                            PowerShotDropdown = inputData.PowerShotDropdown,
                            DistanceInput = inputData.DistanceInput,
                            HeightInput = inputData.HeightInput,
                            WindInput = inputData.WindInput,
                            DegreeInput = inputData.DegreeInput,
                            GroundInput = inputData.GroundInput,
                            SpinInput = spin.ToString(), // Use the new spin value
                            CurveInput = inputData.CurveInput,
                            SlopeBreakInput = inputData.SlopeBreakInput
                        };

                        // Do final update with all UI elements using the updated input data
                        await UpdateValues(updatedInputData);
                        return; // Exit the entire method
                    }
                }


            }
        }

        private List<double> GenerateAllPossibleCalipers(double drive)
        {
            List<double> CaliperList = new List<double>();
            double step = 0.27777777777778;
            List<double> percentages = GeneratePercentages(100.00, 0.00, step);

            foreach (var percentage in percentages)
            {
                double RawCaliper = drive * (percentage / 100);
                double decimalPart = RawCaliper - Math.Floor(RawCaliper);

                var ranges = new (double Min, double Max, Func<double, double> RoundingFunction)[]
                {
                    (0.0499, 0.05, x => Math.Ceiling(x * 10) / 10),
                    (0.15, 0.1500000006, x => Math.Floor(x * 10) / 10),
                    (0.2499, 0.25, x => Math.Ceiling(x * 10) / 10),
                    (0.3499, 0.35, x => Math.Ceiling(x * 10) / 10),
                    (0.4499, 0.45, x => Math.Ceiling(x * 10) / 10),
                    (0.5499, 0.55, x => Math.Ceiling(x * 10) / 10),
                    (0.6499, 0.65, x => Math.Ceiling(x * 10) / 10),
                    (0.7499, 0.75, x => Math.Ceiling(x * 10) / 10),
                    (0.8499, 0.85, x => Math.Ceiling(x * 10) / 10),
                    (0.9499, 0.95, x => Math.Ceiling(x * 10) / 10)
                };

                bool rangeMatched = false;
                foreach (var range in ranges)
                {
                    if (decimalPart > range.Min && decimalPart < range.Max)
                    {
                        CaliperList.Add(range.RoundingFunction(RawCaliper));
                        rangeMatched = true;
                        break;
                    }
                }

                if (!rangeMatched)
                {
                    CaliperList.Add(Math.Round(RawCaliper, 2, MidpointRounding.AwayFromZero));
                }
            }

            return CaliperList;
        }

        // working area

        private void comboBoxShot_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected item from the Shot dropdown
            string selectedShot = ShotDropdown.SelectedItem.ToString();

            // Check the condition for specific shots
            if (selectedShot == "Tomahawk" || selectedShot == "Spike" || selectedShot == "Cobra")
            {
                // Set Power Shot dropdown to "1"
                PowerShotDropdown.SelectedItem = "1 Power Shot";
            }
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            foreach (Control control in this.Controls)
            {
                // Skip the SpinInput control
                if (control.Name == "pinSelectionButton" || control.Name == "CalculateOptimalSpin" || control.Name == "StayOnTop")
                    continue;

                if (control is TextBox textBox)
                {
                    textBox.Text = defaultValues[textBox.Name];
                }
                else if (control is ComboBox comboBox)
                {
                    comboBox.SelectedItem = defaultValues[comboBox.Name];
                }
                else if (control is CheckBox checkBox)
                {
                    checkBox.Checked = false; // Uncheck all checkboxes
                }
                else if (control is RadioButton radioButton)
                {
                    radioButton.Checked = false; // Uncheck all radio buttons
                }
            }


            SendToPipe("reset", null);


        }


        private void pinSelectionBotton_Click(object sender, EventArgs e)
        {
            PinSelectionForm pinSelectionForm = new PinSelectionForm
            {
                Owner = this // Set this (PangYaC) as the owner
            };
            pinSelectionForm.ShowDialog(); // Use ShowDialog if you want the PinSelectionForm to block until it is closed

            this.BeginInvoke(new Action(() =>
            {
                pinSelectionButton.NotifyDefault(false);
                // Force blur on the button
                pinSelectionButton.Enabled = false;
                pinSelectionButton.Enabled = true;

                // Clear any active control
                this.ActiveControl = null;

                // Focus WindInput
                WindInput.Focus();
                WindInput.SelectAll();
                this.ActiveControl = WindInput;
            }));
        }
      

        public void UpdateDistanceHeightAndSlope(string distance, string height, string slope)
        {
            /// Update the Distance and Height fields
            DistanceInput.Text = distance;
            HeightInput.Text = height;
            SlopeBreakInput.Text = slope;

        }

        //saves the offsets
        private void SaveOffsets()
        {
            try
            {
                string directory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "PangYaC"
                );
                Directory.CreateDirectory(directory);
                string settingsFile = Path.Combine(directory, "capture_settings.txt");

                // Save X, Y offsets and zoom
                File.WriteAllText(settingsFile, $"{offsetX},{offsetY},{zoom}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving offsets: {ex.Message}");
            }
        }

        //loads the offsets
        private void LoadOffsets()
        {
            try
            {
                string settingsFile = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "PangYaC",
                    "capture_settings.txt"
                );

                if (File.Exists(settingsFile))
                {
                    string[] savedSettings = File.ReadAllText(settingsFile).Split(',');
                    if (savedSettings.Length == 3)
                    {
                        if (int.TryParse(savedSettings[0], out int loadedOffsetX))
                            offsetX = loadedOffsetX;
                        if (int.TryParse(savedSettings[1], out int loadedOffsetY))
                            offsetY = loadedOffsetY;
                        if (int.TryParse(savedSettings[2], out int loadedZoom))
                            zoom = loadedZoom;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading offsets: {ex.Message}");
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {   
            offsetY -= 1;
            CaptureScreenArea("Pangya Reborn!");
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            offsetY += 1;
            CaptureScreenArea("Pangya Reborn!");
        }

        private void Save_Click(object sender, EventArgs e)
        {
            SaveOffsets();
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            offsetX -= 1;
            CaptureScreenArea("Pangya Reborn!");
        }
        private void btnRight_Click(object sender, EventArgs e)
        {
            offsetX += 1;
            CaptureScreenArea("Pangya Reborn!");
        }
        private void zoomOut_Click(object sender, EventArgs e)
        {
            zoom += 1;
            CaptureScreenArea("Pangya Reborn!");

        }

        private void zoomIn_Click(object sender, EventArgs e)
        {
            
            zoom -= 1;
            CaptureScreenArea("Pangya Reborn!");
        }
    }


}