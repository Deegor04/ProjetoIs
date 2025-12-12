namespace TestApplications
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
            this.SuspendLayout();
            // 
            // textBoxCreateApp
            // 
            this.textBoxCreateApp.Location = new System.Drawing.Point(28, 70);
            this.textBoxCreateApp.Name = "textBoxCreateApp";
            this.textBoxCreateApp.Size = new System.Drawing.Size(383, 26);
            this.textBoxCreateApp.TabIndex = 0;
            // 
            // buttonCreateApp
            // 
            this.buttonCreateApp.Location = new System.Drawing.Point(457, 69);
            this.buttonCreateApp.Name = "buttonCreateApp";
            this.buttonCreateApp.Size = new System.Drawing.Size(154, 28);
            this.buttonCreateApp.TabIndex = 1;
            this.buttonCreateApp.Text = "Add App";
            this.buttonCreateApp.UseVisualStyleBackColor = true;
            // 
            // CreateApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 163);
            this.Controls.Add(this.buttonCreateApp);
            this.Controls.Add(this.textBoxCreateApp);
            this.Name = "CreateApp";
            this.Text = "CreateApp";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxCreateApp;
        private System.Windows.Forms.Button buttonCreateApp;
    }
}