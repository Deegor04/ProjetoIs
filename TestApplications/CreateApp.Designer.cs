namespace SomiodPublisher
{
    partial class CreateApp
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
            this.textBoxCreateApp = new System.Windows.Forms.TextBox();
            this.buttonCreateApp = new System.Windows.Forms.Button();
            this.groupBoxAddApp = new System.Windows.Forms.GroupBox();
            this.labelName = new System.Windows.Forms.Label();
            this.groupBoxDeleteApp = new System.Windows.Forms.GroupBox();
            this.labelSelectAppDelete = new System.Windows.Forms.Label();
            this.comboBoxSelectAppDel = new System.Windows.Forms.ComboBox();
            this.buttonDeleteApp = new System.Windows.Forms.Button();
            this.groupBoxAddApp.SuspendLayout();
            this.groupBoxDeleteApp.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxCreateApp
            // 
            this.textBoxCreateApp.Location = new System.Drawing.Point(221, 21);
            this.textBoxCreateApp.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxCreateApp.Name = "textBoxCreateApp";
            this.textBoxCreateApp.Size = new System.Drawing.Size(341, 22);
            this.textBoxCreateApp.TabIndex = 0;
            // 
            // buttonCreateApp
            // 
            this.buttonCreateApp.Location = new System.Drawing.Point(435, 70);
            this.buttonCreateApp.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonCreateApp.Name = "buttonCreateApp";
            this.buttonCreateApp.Size = new System.Drawing.Size(129, 30);
            this.buttonCreateApp.TabIndex = 1;
            this.buttonCreateApp.Text = "Add";
            this.buttonCreateApp.UseVisualStyleBackColor = true;
            this.buttonCreateApp.Click += new System.EventHandler(this.buttonCreateApp_Click);
            // 
            // groupBoxAddApp
            // 
            this.groupBoxAddApp.Controls.Add(this.labelName);
            this.groupBoxAddApp.Controls.Add(this.buttonCreateApp);
            this.groupBoxAddApp.Controls.Add(this.textBoxCreateApp);
            this.groupBoxAddApp.Location = new System.Drawing.Point(41, 10);
            this.groupBoxAddApp.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBoxAddApp.Name = "groupBoxAddApp";
            this.groupBoxAddApp.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBoxAddApp.Size = new System.Drawing.Size(613, 119);
            this.groupBoxAddApp.TabIndex = 2;
            this.groupBoxAddApp.TabStop = false;
            this.groupBoxAddApp.Text = "Add ";
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(25, 30);
            this.labelName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(44, 16);
            this.labelName.TabIndex = 2;
            this.labelName.Text = "Name";
            // 
            // groupBoxDeleteApp
            // 
            this.groupBoxDeleteApp.Controls.Add(this.labelSelectAppDelete);
            this.groupBoxDeleteApp.Controls.Add(this.comboBoxSelectAppDel);
            this.groupBoxDeleteApp.Controls.Add(this.buttonDeleteApp);
            this.groupBoxDeleteApp.Location = new System.Drawing.Point(42, 162);
            this.groupBoxDeleteApp.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBoxDeleteApp.Name = "groupBoxDeleteApp";
            this.groupBoxDeleteApp.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBoxDeleteApp.Size = new System.Drawing.Size(612, 126);
            this.groupBoxDeleteApp.TabIndex = 4;
            this.groupBoxDeleteApp.TabStop = false;
            this.groupBoxDeleteApp.Text = "Delete";
            // 
            // labelSelectAppDelete
            // 
            this.labelSelectAppDelete.AutoSize = true;
            this.labelSelectAppDelete.Location = new System.Drawing.Point(25, 36);
            this.labelSelectAppDelete.Name = "labelSelectAppDelete";
            this.labelSelectAppDelete.Size = new System.Drawing.Size(73, 16);
            this.labelSelectAppDelete.TabIndex = 5;
            this.labelSelectAppDelete.Text = "Select App";
            // 
            // comboBoxSelectAppDel
            // 
            this.comboBoxSelectAppDel.FormattingEnabled = true;
            this.comboBoxSelectAppDel.Location = new System.Drawing.Point(317, 32);
            this.comboBoxSelectAppDel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBoxSelectAppDel.Name = "comboBoxSelectAppDel";
            this.comboBoxSelectAppDel.Size = new System.Drawing.Size(245, 24);
            this.comboBoxSelectAppDel.TabIndex = 5;
            // 
            // buttonDeleteApp
            // 
            this.buttonDeleteApp.Location = new System.Drawing.Point(435, 82);
            this.buttonDeleteApp.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonDeleteApp.Name = "buttonDeleteApp";
            this.buttonDeleteApp.Size = new System.Drawing.Size(129, 28);
            this.buttonDeleteApp.TabIndex = 5;
            this.buttonDeleteApp.Text = "Delete";
            this.buttonDeleteApp.UseVisualStyleBackColor = true;
            this.buttonDeleteApp.Click += new System.EventHandler(this.buttonDeleteApp_Click);
            // 
            // CreateApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(702, 339);
            this.Controls.Add(this.groupBoxDeleteApp);
            this.Controls.Add(this.groupBoxAddApp);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "CreateApp";
            this.Text = "Application";
            this.Load += new System.EventHandler(this.CreateApp_Load);
            this.groupBoxAddApp.ResumeLayout(false);
            this.groupBoxAddApp.PerformLayout();
            this.groupBoxDeleteApp.ResumeLayout(false);
            this.groupBoxDeleteApp.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxCreateApp;
        private System.Windows.Forms.Button buttonCreateApp;
        private System.Windows.Forms.GroupBox groupBoxAddApp;
        private System.Windows.Forms.GroupBox groupBoxDeleteApp;
        private System.Windows.Forms.Label labelSelectAppDelete;
        private System.Windows.Forms.ComboBox comboBoxSelectAppDel;
        private System.Windows.Forms.Button buttonDeleteApp;
        private System.Windows.Forms.Label labelName;
    }
}