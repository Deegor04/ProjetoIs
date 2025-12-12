namespace TestApplications
{
    partial class Form1
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
            this.buttonNewApplication = new System.Windows.Forms.Button();
            this.buttonNewContainer = new System.Windows.Forms.Button();
            this.buttonNewSubsContentInstance = new System.Windows.Forms.Button();
            this.richTextBoxShowAll = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // buttonNewApplication
            // 
            this.buttonNewApplication.Location = new System.Drawing.Point(54, 41);
            this.buttonNewApplication.Name = "buttonNewApplication";
            this.buttonNewApplication.Size = new System.Drawing.Size(218, 54);
            this.buttonNewApplication.TabIndex = 0;
            this.buttonNewApplication.Text = "New Application";
            this.buttonNewApplication.UseVisualStyleBackColor = true;
            // 
            // buttonNewContainer
            // 
            this.buttonNewContainer.Location = new System.Drawing.Point(365, 41);
            this.buttonNewContainer.Name = "buttonNewContainer";
            this.buttonNewContainer.Size = new System.Drawing.Size(225, 54);
            this.buttonNewContainer.TabIndex = 1;
            this.buttonNewContainer.Text = "New Container";
            this.buttonNewContainer.UseVisualStyleBackColor = true;
            // 
            // buttonNewSubsContentInstance
            // 
            this.buttonNewSubsContentInstance.Location = new System.Drawing.Point(663, 41);
            this.buttonNewSubsContentInstance.Name = "buttonNewSubsContentInstance";
            this.buttonNewSubsContentInstance.Size = new System.Drawing.Size(281, 54);
            this.buttonNewSubsContentInstance.TabIndex = 2;
            this.buttonNewSubsContentInstance.Text = "New Subscription / Content Instance";
            this.buttonNewSubsContentInstance.UseVisualStyleBackColor = true;
            // 
            // richTextBoxShowAll
            // 
            this.richTextBoxShowAll.Location = new System.Drawing.Point(54, 134);
            this.richTextBoxShowAll.Name = "richTextBoxShowAll";
            this.richTextBoxShowAll.Size = new System.Drawing.Size(885, 364);
            this.richTextBoxShowAll.TabIndex = 3;
            this.richTextBoxShowAll.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(971, 510);
            this.Controls.Add(this.richTextBoxShowAll);
            this.Controls.Add(this.buttonNewSubsContentInstance);
            this.Controls.Add(this.buttonNewContainer);
            this.Controls.Add(this.buttonNewApplication);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonNewApplication;
        private System.Windows.Forms.Button buttonNewContainer;
        private System.Windows.Forms.Button buttonNewSubsContentInstance;
        private System.Windows.Forms.RichTextBox richTextBoxShowAll;
    }
}

