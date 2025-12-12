namespace TestApplications
{
    partial class CreateSubsContInst
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
            this.labelSelectContainer = new System.Windows.Forms.Label();
            this.comboBoxSelectContainer = new System.Windows.Forms.ComboBox();
            this.buttonCreateSubscription = new System.Windows.Forms.Button();
            this.buttonCreateContentInstance = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelSelectContainer
            // 
            this.labelSelectContainer.AutoSize = true;
            this.labelSelectContainer.Location = new System.Drawing.Point(36, 37);
            this.labelSelectContainer.Name = "labelSelectContainer";
            this.labelSelectContainer.Size = new System.Drawing.Size(127, 20);
            this.labelSelectContainer.TabIndex = 0;
            this.labelSelectContainer.Text = "Select Container";
            // 
            // comboBoxSelectContainer
            // 
            this.comboBoxSelectContainer.FormattingEnabled = true;
            this.comboBoxSelectContainer.Location = new System.Drawing.Point(272, 34);
            this.comboBoxSelectContainer.Name = "comboBoxSelectContainer";
            this.comboBoxSelectContainer.Size = new System.Drawing.Size(314, 28);
            this.comboBoxSelectContainer.TabIndex = 1;
            // 
            // buttonCreateSubscription
            // 
            this.buttonCreateSubscription.Location = new System.Drawing.Point(41, 130);
            this.buttonCreateSubscription.Name = "buttonCreateSubscription";
            this.buttonCreateSubscription.Size = new System.Drawing.Size(195, 38);
            this.buttonCreateSubscription.TabIndex = 2;
            this.buttonCreateSubscription.Text = "Create Subscription";
            this.buttonCreateSubscription.UseVisualStyleBackColor = true;
            // 
            // buttonCreateContentInstance
            // 
            this.buttonCreateContentInstance.Location = new System.Drawing.Point(370, 130);
            this.buttonCreateContentInstance.Name = "buttonCreateContentInstance";
            this.buttonCreateContentInstance.Size = new System.Drawing.Size(195, 38);
            this.buttonCreateContentInstance.TabIndex = 3;
            this.buttonCreateContentInstance.Text = "Create Content Instance";
            this.buttonCreateContentInstance.UseVisualStyleBackColor = true;
            // 
            // CreateSubsContInst
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 246);
            this.Controls.Add(this.buttonCreateContentInstance);
            this.Controls.Add(this.buttonCreateSubscription);
            this.Controls.Add(this.comboBoxSelectContainer);
            this.Controls.Add(this.labelSelectContainer);
            this.Name = "CreateSubsContInst";
            this.Text = "CreateSubsContInst";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelSelectContainer;
        private System.Windows.Forms.ComboBox comboBoxSelectContainer;
        private System.Windows.Forms.Button buttonCreateSubscription;
        private System.Windows.Forms.Button buttonCreateContentInstance;
    }
}