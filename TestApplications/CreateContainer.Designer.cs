namespace TestApplications
{
    partial class CreateContainer
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
            this.comboBoxSelectApp = new System.Windows.Forms.ComboBox();
            this.labelSelectApp = new System.Windows.Forms.Label();
            this.labelContainerName = new System.Windows.Forms.Label();
            this.textBoxContainerName = new System.Windows.Forms.TextBox();
            this.buttonAddContainer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboBoxSelectApp
            // 
            this.comboBoxSelectApp.FormattingEnabled = true;
            this.comboBoxSelectApp.Location = new System.Drawing.Point(273, 57);
            this.comboBoxSelectApp.Name = "comboBoxSelectApp";
            this.comboBoxSelectApp.Size = new System.Drawing.Size(232, 28);
            this.comboBoxSelectApp.TabIndex = 0;
            // 
            // labelSelectApp
            // 
            this.labelSelectApp.AutoSize = true;
            this.labelSelectApp.Location = new System.Drawing.Point(47, 60);
            this.labelSelectApp.Name = "labelSelectApp";
            this.labelSelectApp.Size = new System.Drawing.Size(87, 20);
            this.labelSelectApp.TabIndex = 1;
            this.labelSelectApp.Text = "Select App";
            // 
            // labelContainerName
            // 
            this.labelContainerName.AutoSize = true;
            this.labelContainerName.Location = new System.Drawing.Point(46, 143);
            this.labelContainerName.Name = "labelContainerName";
            this.labelContainerName.Size = new System.Drawing.Size(124, 20);
            this.labelContainerName.TabIndex = 2;
            this.labelContainerName.Text = "Container Name";
            // 
            // textBoxContainerName
            // 
            this.textBoxContainerName.Location = new System.Drawing.Point(268, 143);
            this.textBoxContainerName.Name = "textBoxContainerName";
            this.textBoxContainerName.Size = new System.Drawing.Size(316, 26);
            this.textBoxContainerName.TabIndex = 3;
            // 
            // buttonAddContainer
            // 
            this.buttonAddContainer.Location = new System.Drawing.Point(68, 217);
            this.buttonAddContainer.Name = "buttonAddContainer";
            this.buttonAddContainer.Size = new System.Drawing.Size(200, 27);
            this.buttonAddContainer.TabIndex = 4;
            this.buttonAddContainer.Text = "Add Container";
            this.buttonAddContainer.UseVisualStyleBackColor = true;
            // 
            // CreateContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 338);
            this.Controls.Add(this.buttonAddContainer);
            this.Controls.Add(this.textBoxContainerName);
            this.Controls.Add(this.labelContainerName);
            this.Controls.Add(this.labelSelectApp);
            this.Controls.Add(this.comboBoxSelectApp);
            this.Name = "CreateContainer";
            this.Text = "CreateContainer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxSelectApp;
        private System.Windows.Forms.Label labelSelectApp;
        private System.Windows.Forms.Label labelContainerName;
        private System.Windows.Forms.TextBox textBoxContainerName;
        private System.Windows.Forms.Button buttonAddContainer;
    }
}