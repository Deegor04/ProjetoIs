namespace SomiodSubscriber
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
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.pictureBoxState = new System.Windows.Forms.PictureBox();
            this.lblStateTitle = new System.Windows.Forms.Label();
            this.lblConnection = new System.Windows.Forms.Label();
            this.lblLastCommand = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxState)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(23, 71);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(348, 243);
            this.txtLog.TabIndex = 1;
            this.txtLog.Text = "";
            // 
            // pictureBoxState
            // 
            this.pictureBoxState.Location = new System.Drawing.Point(420, 71);
            this.pictureBoxState.Name = "pictureBoxState";
            this.pictureBoxState.Size = new System.Drawing.Size(315, 243);
            this.pictureBoxState.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxState.TabIndex = 5;
            this.pictureBoxState.TabStop = false;
            // 
            // lblStateTitle
            // 
            this.lblStateTitle.AutoSize = true;
            this.lblStateTitle.Location = new System.Drawing.Point(420, 54);
            this.lblStateTitle.Name = "lblStateTitle";
            this.lblStateTitle.Size = new System.Drawing.Size(63, 13);
            this.lblStateTitle.TabIndex = 6;
            this.lblStateTitle.Text = "Visual State";
            // 
            // lblConnection
            // 
            this.lblConnection.AutoSize = true;
            this.lblConnection.Location = new System.Drawing.Point(11, 16);
            this.lblConnection.Name = "lblConnection";
            this.lblConnection.Size = new System.Drawing.Size(73, 13);
            this.lblConnection.TabIndex = 2;
            this.lblConnection.Text = "Disconnected";
            // 
            // lblLastCommand
            // 
            this.lblLastCommand.AutoSize = true;
            this.lblLastCommand.Location = new System.Drawing.Point(11, 43);
            this.lblLastCommand.Name = "lblLastCommand";
            this.lblLastCommand.Size = new System.Drawing.Size(113, 13);
            this.lblLastCommand.TabIndex = 3;
            this.lblLastCommand.Text = "Last Command: (none)";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblLastCommand);
            this.groupBox1.Controls.Add(this.lblConnection);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(374, 311);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Status";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(769, 326);
            this.Controls.Add(this.lblStateTitle);
            this.Controls.Add(this.pictureBoxState);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "SOMIOD - Subscriber App";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxState)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.PictureBox pictureBoxState;
        private System.Windows.Forms.Label lblStateTitle;
        private System.Windows.Forms.Label lblConnection;
        private System.Windows.Forms.Label lblLastCommand;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

