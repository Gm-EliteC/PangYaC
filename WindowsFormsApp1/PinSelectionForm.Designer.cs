namespace WindowsFormsApp1
{
    partial class PinSelectionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.CoursesDropDown = new System.Windows.Forms.ComboBox();
            this.courseDataGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.courseDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // CoursesDropDown
            // 
            this.CoursesDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CoursesDropDown.FormattingEnabled = true;
            this.CoursesDropDown.Location = new System.Drawing.Point(13, 13);
            this.CoursesDropDown.Name = "CoursesDropDown";
            this.CoursesDropDown.Size = new System.Drawing.Size(121, 21);
            this.CoursesDropDown.TabIndex = 0;
            this.CoursesDropDown.SelectedIndexChanged += new System.EventHandler(this.Course_SelectedIndexChanged);
            // 
            // courseDataGridView
            // 
            this.courseDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.courseDataGridView.Location = new System.Drawing.Point(750, 419);
            this.courseDataGridView.Name = "courseDataGridView";
            this.courseDataGridView.Size = new System.Drawing.Size(38, 19);
            this.courseDataGridView.TabIndex = 1;
            this.courseDataGridView.Visible = false;
            // 
            // PinSelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.CoursesDropDown);
            this.Controls.Add(this.courseDataGridView);
            this.Name = "PinSelectionForm";
            this.Text = "PInSelectionForm";
            ((System.ComponentModel.ISupportInitialize)(this.courseDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox CoursesDropDown;
        private System.Windows.Forms.DataGridView courseDataGridView;
    }
}