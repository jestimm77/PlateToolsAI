namespace PlateToolsAI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblJobNumber = new System.Windows.Forms.Label();
            this.txtJobNumber = new System.Windows.Forms.TextBox();
            this.lstLots = new System.Windows.Forms.ListBox();
            this.lstSequences = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lblJobNumber
            // 
            this.lblJobNumber.AutoSize = true;
            this.lblJobNumber.Location = new System.Drawing.Point(12, 9);
            this.lblJobNumber.Name = "lblJobNumber";
            this.lblJobNumber.Size = new System.Drawing.Size(81, 17);
            this.lblJobNumber.TabIndex = 1;
            this.lblJobNumber.Text = "Job Number";
            // 
            // txtJobNumber
            // 
            this.txtJobNumber.Location = new System.Drawing.Point(99, 6);
            this.txtJobNumber.Name = "txtJobNumber";
            this.txtJobNumber.Size = new System.Drawing.Size(150, 25);
            this.txtJobNumber.TabIndex = 2;
            // 
            // lstLots
            // 
            this.lstLots.FormattingEnabled = true;
            this.lstLots.ItemHeight = 17;
            this.lstLots.Location = new System.Drawing.Point(371, 6);
            this.lstLots.Name = "lstLots";
            this.lstLots.Size = new System.Drawing.Size(150, 55);
            this.lstLots.TabIndex = 3;
            // 
            // lstSequences
            // 
            this.lstSequences.FormattingEnabled = true;
            this.lstSequences.ItemHeight = 17;
            this.lstSequences.Location = new System.Drawing.Point(245, 6);
            this.lstSequences.Name = "lstSequences";
            this.lstSequences.Size = new System.Drawing.Size(120, 55);
            this.lstSequences.TabIndex = 5;
            this.lstSequences.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1898, 1052);
            this.Controls.Add(this.lstSequences);
            this.Controls.Add(this.lstLots);
            this.Controls.Add(this.txtJobNumber);
            this.Controls.Add(this.lblJobNumber);
            this.Name = "Form1";
            this.Text = "Available Sequences";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblJobNumber;
        private System.Windows.Forms.TextBox txtJobNumber;
        private System.Windows.Forms.ListBox lstLots;
        private System.Windows.Forms.ListBox lstSequences;
    }
}

