namespace SomiodPublisher
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
            this.treeViewSomiod = new System.Windows.Forms.TreeView();
            this.btnRefreshTree = new System.Windows.Forms.Button();
            this.richTextBoxDetails = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonSubscription = new System.Windows.Forms.Button();
            this.buttonContentInstance = new System.Windows.Forms.Button();
            this.panelActions = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonNewApplication
            // 
            this.buttonNewApplication.Location = new System.Drawing.Point(24, 15);
            this.buttonNewApplication.Margin = new System.Windows.Forms.Padding(2);
            this.buttonNewApplication.Name = "buttonNewApplication";
            this.buttonNewApplication.Size = new System.Drawing.Size(145, 35);
            this.buttonNewApplication.TabIndex = 0;
            this.buttonNewApplication.Text = "Application";
            this.buttonNewApplication.UseVisualStyleBackColor = true;
            this.buttonNewApplication.Click += new System.EventHandler(this.buttonNewApplication_Click);
            // 
            // buttonNewContainer
            // 
            this.buttonNewContainer.Location = new System.Drawing.Point(274, 15);
            this.buttonNewContainer.Margin = new System.Windows.Forms.Padding(2);
            this.buttonNewContainer.Name = "buttonNewContainer";
            this.buttonNewContainer.Size = new System.Drawing.Size(150, 35);
            this.buttonNewContainer.TabIndex = 1;
            this.buttonNewContainer.Text = "Container";
            this.buttonNewContainer.UseVisualStyleBackColor = true;
            this.buttonNewContainer.Click += new System.EventHandler(this.buttonNewContainer_Click);
            // 
            // treeViewSomiod
            // 
            this.treeViewSomiod.Location = new System.Drawing.Point(36, 96);
            this.treeViewSomiod.Name = "treeViewSomiod";
            this.treeViewSomiod.Size = new System.Drawing.Size(357, 210);
            this.treeViewSomiod.TabIndex = 4;
            this.treeViewSomiod.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewSomiod_AfterSelect);
            // 
            // btnRefreshTree
            // 
            this.btnRefreshTree.Location = new System.Drawing.Point(318, 312);
            this.btnRefreshTree.Name = "btnRefreshTree";
            this.btnRefreshTree.Size = new System.Drawing.Size(75, 23);
            this.btnRefreshTree.TabIndex = 5;
            this.btnRefreshTree.Text = "Refresh";
            this.btnRefreshTree.UseVisualStyleBackColor = true;
            this.btnRefreshTree.Click += new System.EventHandler(this.btnRefreshTree_Click);
            // 
            // richTextBoxDetails
            // 
            this.richTextBoxDetails.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxDetails.Location = new System.Drawing.Point(422, 96);
            this.richTextBoxDetails.Name = "richTextBoxDetails";
            this.richTextBoxDetails.ReadOnly = true;
            this.richTextBoxDetails.Size = new System.Drawing.Size(270, 210);
            this.richTextBoxDetails.TabIndex = 6;
            this.richTextBoxDetails.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonSubscription);
            this.groupBox1.Controls.Add(this.buttonContentInstance);
            this.groupBox1.Controls.Add(this.buttonNewApplication);
            this.groupBox1.Controls.Add(this.buttonNewContainer);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1014, 63);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Menus";
            // 
            // buttonSubscription
            // 
            this.buttonSubscription.Location = new System.Drawing.Point(838, 15);
            this.buttonSubscription.Name = "buttonSubscription";
            this.buttonSubscription.Size = new System.Drawing.Size(150, 35);
            this.buttonSubscription.TabIndex = 4;
            this.buttonSubscription.Text = "Subscription";
            this.buttonSubscription.UseVisualStyleBackColor = true;
            this.buttonSubscription.Click += new System.EventHandler(this.buttonSubscription_Click);
            // 
            // buttonContentInstance
            // 
            this.buttonContentInstance.Location = new System.Drawing.Point(564, 15);
            this.buttonContentInstance.Name = "buttonContentInstance";
            this.buttonContentInstance.Size = new System.Drawing.Size(150, 35);
            this.buttonContentInstance.TabIndex = 3;
            this.buttonContentInstance.Text = "Content Instance";
            this.buttonContentInstance.UseVisualStyleBackColor = true;
            this.buttonContentInstance.Click += new System.EventHandler(this.buttonContentInstance_Click);
            // 
            // panelActions
            // 
            this.panelActions.Location = new System.Drawing.Point(737, 96);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(263, 210);
            this.panelActions.TabIndex = 8;
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(12, 81);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(689, 264);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Informations";
            // 
            // groupBox3
            // 
            this.groupBox3.Location = new System.Drawing.Point(724, 81);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(302, 264);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Actions";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1038, 357);
            this.Controls.Add(this.panelActions);
            this.Controls.Add(this.richTextBoxDetails);
            this.Controls.Add(this.btnRefreshTree);
            this.Controls.Add(this.treeViewSomiod);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "SOMIOD - Publisher App";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonNewApplication;
        private System.Windows.Forms.Button buttonNewContainer;
        private System.Windows.Forms.TreeView treeViewSomiod;
        private System.Windows.Forms.Button btnRefreshTree;
        private System.Windows.Forms.RichTextBox richTextBoxDetails;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panelActions;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button buttonSubscription;
        private System.Windows.Forms.Button buttonContentInstance;
    }
}

