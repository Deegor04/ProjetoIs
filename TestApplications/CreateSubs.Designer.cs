namespace SomiodPublisher
{
    partial class CreateSubs
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
            this.groupBoxAddSubscription = new System.Windows.Forms.GroupBox();
            this.textBoxEndpoint = new System.Windows.Forms.TextBox();
            this.labelEndpoint = new System.Windows.Forms.Label();
            this.comboBoxEvent = new System.Windows.Forms.ComboBox();
            this.labelEvent = new System.Windows.Forms.Label();
            this.buttonAddSubscription = new System.Windows.Forms.Button();
            this.textBoxSubscriptionName = new System.Windows.Forms.TextBox();
            this.labelNameSubscription = new System.Windows.Forms.Label();
            this.comboBoxSelectContainerSubs = new System.Windows.Forms.ComboBox();
            this.labelSelectContainerSubs = new System.Windows.Forms.Label();
            this.groupBoxDeleteSubscription = new System.Windows.Forms.GroupBox();
            this.buttonDeleteSubscription = new System.Windows.Forms.Button();
            this.comboBoxSelectSubscription = new System.Windows.Forms.ComboBox();
            this.labelSelectSubscription = new System.Windows.Forms.Label();
            this.groupBoxAddSubscription.SuspendLayout();
            this.groupBoxDeleteSubscription.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxAddSubscription
            // 
            this.groupBoxAddSubscription.Controls.Add(this.textBoxEndpoint);
            this.groupBoxAddSubscription.Controls.Add(this.labelEndpoint);
            this.groupBoxAddSubscription.Controls.Add(this.comboBoxEvent);
            this.groupBoxAddSubscription.Controls.Add(this.labelEvent);
            this.groupBoxAddSubscription.Controls.Add(this.buttonAddSubscription);
            this.groupBoxAddSubscription.Controls.Add(this.textBoxSubscriptionName);
            this.groupBoxAddSubscription.Controls.Add(this.labelNameSubscription);
            this.groupBoxAddSubscription.Controls.Add(this.comboBoxSelectContainerSubs);
            this.groupBoxAddSubscription.Controls.Add(this.labelSelectContainerSubs);
            this.groupBoxAddSubscription.Location = new System.Drawing.Point(23, 18);
            this.groupBoxAddSubscription.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxAddSubscription.Name = "groupBoxAddSubscription";
            this.groupBoxAddSubscription.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxAddSubscription.Size = new System.Drawing.Size(409, 173);
            this.groupBoxAddSubscription.TabIndex = 0;
            this.groupBoxAddSubscription.TabStop = false;
            this.groupBoxAddSubscription.Text = "Add Subscription";
            // 
            // textBoxEndpoint
            // 
            this.textBoxEndpoint.Location = new System.Drawing.Point(212, 122);
            this.textBoxEndpoint.Name = "textBoxEndpoint";
            this.textBoxEndpoint.Size = new System.Drawing.Size(175, 20);
            this.textBoxEndpoint.TabIndex = 8;
            this.textBoxEndpoint.TextChanged += new System.EventHandler(this.textBoxEndpoint_TextChanged);
            // 
            // labelEndpoint
            // 
            this.labelEndpoint.AutoSize = true;
            this.labelEndpoint.Location = new System.Drawing.Point(12, 122);
            this.labelEndpoint.Name = "labelEndpoint";
            this.labelEndpoint.Size = new System.Drawing.Size(133, 13);
            this.labelEndpoint.TabIndex = 7;
            this.labelEndpoint.Text = "Endpoint (HTTP or MQTT)";
            // 
            // comboBoxEvent
            // 
            this.comboBoxEvent.FormattingEnabled = true;
            this.comboBoxEvent.Location = new System.Drawing.Point(212, 85);
            this.comboBoxEvent.Name = "comboBoxEvent";
            this.comboBoxEvent.Size = new System.Drawing.Size(175, 21);
            this.comboBoxEvent.TabIndex = 6;
            this.comboBoxEvent.SelectedIndexChanged += new System.EventHandler(this.comboBoxEvent_SelectedIndexChanged);
            // 
            // labelEvent
            // 
            this.labelEvent.AutoSize = true;
            this.labelEvent.Location = new System.Drawing.Point(12, 88);
            this.labelEvent.Name = "labelEvent";
            this.labelEvent.Size = new System.Drawing.Size(35, 13);
            this.labelEvent.TabIndex = 5;
            this.labelEvent.Text = "Event";
            // 
            // buttonAddSubscription
            // 
            this.buttonAddSubscription.Location = new System.Drawing.Point(301, 148);
            this.buttonAddSubscription.Margin = new System.Windows.Forms.Padding(2);
            this.buttonAddSubscription.Name = "buttonAddSubscription";
            this.buttonAddSubscription.Size = new System.Drawing.Size(86, 21);
            this.buttonAddSubscription.TabIndex = 4;
            this.buttonAddSubscription.Text = "Add";
            this.buttonAddSubscription.UseVisualStyleBackColor = true;
            this.buttonAddSubscription.Click += new System.EventHandler(this.buttonAddSubscription_Click);
            // 
            // textBoxSubscriptionName
            // 
            this.textBoxSubscriptionName.Location = new System.Drawing.Point(212, 51);
            this.textBoxSubscriptionName.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxSubscriptionName.Name = "textBoxSubscriptionName";
            this.textBoxSubscriptionName.Size = new System.Drawing.Size(175, 20);
            this.textBoxSubscriptionName.TabIndex = 3;
            this.textBoxSubscriptionName.TextChanged += new System.EventHandler(this.textBoxSubscriptionName_TextChanged);
            // 
            // labelNameSubscription
            // 
            this.labelNameSubscription.AutoSize = true;
            this.labelNameSubscription.Location = new System.Drawing.Point(12, 51);
            this.labelNameSubscription.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelNameSubscription.Name = "labelNameSubscription";
            this.labelNameSubscription.Size = new System.Drawing.Size(96, 13);
            this.labelNameSubscription.TabIndex = 2;
            this.labelNameSubscription.Text = "Subscription Name";
            // 
            // comboBoxSelectContainerSubs
            // 
            this.comboBoxSelectContainerSubs.FormattingEnabled = true;
            this.comboBoxSelectContainerSubs.Location = new System.Drawing.Point(212, 20);
            this.comboBoxSelectContainerSubs.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxSelectContainerSubs.Name = "comboBoxSelectContainerSubs";
            this.comboBoxSelectContainerSubs.Size = new System.Drawing.Size(175, 21);
            this.comboBoxSelectContainerSubs.TabIndex = 1;
            this.comboBoxSelectContainerSubs.SelectedIndexChanged += new System.EventHandler(this.comboBoxSelectContainerSubs_SelectedIndexChanged);
            // 
            // labelSelectContainerSubs
            // 
            this.labelSelectContainerSubs.AutoSize = true;
            this.labelSelectContainerSubs.Location = new System.Drawing.Point(9, 21);
            this.labelSelectContainerSubs.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelSelectContainerSubs.Name = "labelSelectContainerSubs";
            this.labelSelectContainerSubs.Size = new System.Drawing.Size(85, 13);
            this.labelSelectContainerSubs.TabIndex = 0;
            this.labelSelectContainerSubs.Text = "Select Container";
            // 
            // groupBoxDeleteSubscription
            // 
            this.groupBoxDeleteSubscription.Controls.Add(this.buttonDeleteSubscription);
            this.groupBoxDeleteSubscription.Controls.Add(this.comboBoxSelectSubscription);
            this.groupBoxDeleteSubscription.Controls.Add(this.labelSelectSubscription);
            this.groupBoxDeleteSubscription.Location = new System.Drawing.Point(23, 195);
            this.groupBoxDeleteSubscription.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxDeleteSubscription.Name = "groupBoxDeleteSubscription";
            this.groupBoxDeleteSubscription.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxDeleteSubscription.Size = new System.Drawing.Size(409, 81);
            this.groupBoxDeleteSubscription.TabIndex = 1;
            this.groupBoxDeleteSubscription.TabStop = false;
            this.groupBoxDeleteSubscription.Text = "Delete Subscription";
            // 
            // buttonDeleteSubscription
            // 
            this.buttonDeleteSubscription.Location = new System.Drawing.Point(301, 51);
            this.buttonDeleteSubscription.Margin = new System.Windows.Forms.Padding(2);
            this.buttonDeleteSubscription.Name = "buttonDeleteSubscription";
            this.buttonDeleteSubscription.Size = new System.Drawing.Size(86, 25);
            this.buttonDeleteSubscription.TabIndex = 2;
            this.buttonDeleteSubscription.Text = "Delete";
            this.buttonDeleteSubscription.UseVisualStyleBackColor = true;
            this.buttonDeleteSubscription.Click += new System.EventHandler(this.buttonDeleteSubscription_Click);
            // 
            // comboBoxSelectSubscription
            // 
            this.comboBoxSelectSubscription.FormattingEnabled = true;
            this.comboBoxSelectSubscription.Location = new System.Drawing.Point(212, 17);
            this.comboBoxSelectSubscription.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxSelectSubscription.Name = "comboBoxSelectSubscription";
            this.comboBoxSelectSubscription.Size = new System.Drawing.Size(175, 21);
            this.comboBoxSelectSubscription.TabIndex = 1;
            this.comboBoxSelectSubscription.SelectedIndexChanged += new System.EventHandler(this.comboBoxSelectSubscription_SelectedIndexChanged);
            // 
            // labelSelectSubscription
            // 
            this.labelSelectSubscription.AutoSize = true;
            this.labelSelectSubscription.Location = new System.Drawing.Point(9, 25);
            this.labelSelectSubscription.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelSelectSubscription.Name = "labelSelectSubscription";
            this.labelSelectSubscription.Size = new System.Drawing.Size(98, 13);
            this.labelSelectSubscription.TabIndex = 0;
            this.labelSelectSubscription.Text = "Select Subscription";
            // 
            // CreateSubs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 287);
            this.Controls.Add(this.groupBoxDeleteSubscription);
            this.Controls.Add(this.groupBoxAddSubscription);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "CreateSubs";
            this.Text = "CreateSubs";
            this.groupBoxAddSubscription.ResumeLayout(false);
            this.groupBoxAddSubscription.PerformLayout();
            this.groupBoxDeleteSubscription.ResumeLayout(false);
            this.groupBoxDeleteSubscription.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxAddSubscription;
        private System.Windows.Forms.Label labelSelectContainerSubs;
        private System.Windows.Forms.Button buttonAddSubscription;
        private System.Windows.Forms.TextBox textBoxSubscriptionName;
        private System.Windows.Forms.Label labelNameSubscription;
        private System.Windows.Forms.ComboBox comboBoxSelectContainerSubs;
        private System.Windows.Forms.GroupBox groupBoxDeleteSubscription;
        private System.Windows.Forms.Button buttonDeleteSubscription;
        private System.Windows.Forms.ComboBox comboBoxSelectSubscription;
        private System.Windows.Forms.Label labelSelectSubscription;
        private System.Windows.Forms.Label labelEvent;
        private System.Windows.Forms.Label labelEndpoint;
        private System.Windows.Forms.ComboBox comboBoxEvent;
        private System.Windows.Forms.TextBox textBoxEndpoint;
    }
}