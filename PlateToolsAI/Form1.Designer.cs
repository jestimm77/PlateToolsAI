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
        ///  the contents of this method.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnProcessJob = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.btnClearAll = new System.Windows.Forms.ToolStripButton();
            this.pnlJobInfo = new System.Windows.Forms.Panel();
            this.lblJobNumberValue = new System.Windows.Forms.Label();
            this.lblJobNumberLabel = new System.Windows.Forms.Label();
            this.lblGroupValue = new System.Windows.Forms.Label();
            this.lblGroupLabel = new System.Windows.Forms.Label();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.pnlParts = new System.Windows.Forms.Panel();
            this.dgvParts = new System.Windows.Forms.DataGridView();
            this.lblAvailableParts = new System.Windows.Forms.Label();
            this.pnlSequences = new System.Windows.Forms.Panel();
            this.lstSequences = new System.Windows.Forms.ListBox();
            this.lblAvailableSequences = new System.Windows.Forms.Label();
            this.pnlLots = new System.Windows.Forms.Panel();
            this.lstLots = new System.Windows.Forms.ListBox();
            this.lblAvailableLots = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            this.pnlJobInfo.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.pnlParts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParts)).BeginInit();
            this.pnlSequences.SuspendLayout();
            this.pnlLots.SuspendLayout();
            this.SuspendLayout();
            
            // toolStrip1
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnProcessJob,
            this.toolStripSeparator1,
            this.btnRefresh,
            this.btnClearAll});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1200, 27);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            
            // btnProcessJob
            this.btnProcessJob.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnProcessJob.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnProcessJob.Name = "btnProcessJob";
            this.btnProcessJob.Size = new System.Drawing.Size(84, 24);
            this.btnProcessJob.Text = "Process Job";
            this.btnProcessJob.Click += new System.EventHandler(this.btnProcessJob_Click);
            
            // toolStripSeparator1
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            
            // btnRefresh
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(50, 24);
            this.btnRefresh.Text = "Refresh";
            
            // btnClearAll
            this.btnClearAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnClearAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(62, 24);
            this.btnClearAll.Text = "Clear All";
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            
            // pnlJobInfo
            this.pnlJobInfo.BackColor = System.Drawing.SystemColors.Control;
            this.pnlJobInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlJobInfo.Controls.Add(this.lblJobNumberValue);
            this.pnlJobInfo.Controls.Add(this.lblJobNumberLabel);
            this.pnlJobInfo.Controls.Add(this.lblGroupValue);
            this.pnlJobInfo.Controls.Add(this.lblGroupLabel);
            this.pnlJobInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlJobInfo.Location = new System.Drawing.Point(0, 27);
            this.pnlJobInfo.Name = "pnlJobInfo";
            this.pnlJobInfo.Size = new System.Drawing.Size(1200, 60);
            this.pnlJobInfo.TabIndex = 1;
            
            // lblJobNumberValue
            this.lblJobNumberValue.AutoSize = true;
            this.lblJobNumberValue.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblJobNumberValue.Location = new System.Drawing.Point(108, 12);
            this.lblJobNumberValue.Name = "lblJobNumberValue";
            this.lblJobNumberValue.Size = new System.Drawing.Size(48, 20);
            this.lblJobNumberValue.TabIndex = 3;
            this.lblJobNumberValue.Text = "(none)";
            
            // lblJobNumberLabel
            this.lblJobNumberLabel.AutoSize = true;
            this.lblJobNumberLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblJobNumberLabel.Location = new System.Drawing.Point(12, 12);
            this.lblJobNumberLabel.Name = "lblJobNumberLabel";
            this.lblJobNumberLabel.Size = new System.Drawing.Size(90, 19);
            this.lblJobNumberLabel.TabIndex = 2;
            this.lblJobNumberLabel.Text = "Job Number:";
            
            // lblGroupValue
            this.lblGroupValue.AutoSize = true;
            this.lblGroupValue.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblGroupValue.Location = new System.Drawing.Point(361, 12);
            this.lblGroupValue.Name = "lblGroupValue";
            this.lblGroupValue.Size = new System.Drawing.Size(48, 20);
            this.lblGroupValue.TabIndex = 1;
            this.lblGroupValue.Text = "(none)";
            
            // lblGroupLabel
            this.lblGroupLabel.AutoSize = true;
            this.lblGroupLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblGroupLabel.Location = new System.Drawing.Point(309, 12);
            this.lblGroupLabel.Name = "lblGroupLabel";
            this.lblGroupLabel.Size = new System.Drawing.Size(46, 19);
            this.lblGroupLabel.TabIndex = 0;
            this.lblGroupLabel.Text = "Group:";
            
            // pnlContent
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 87);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(5);
            this.pnlContent.Size = new System.Drawing.Size(1200, 565);
            this.pnlContent.TabIndex = 2;
            
            // pnlLots
            this.pnlLots.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlLots.Controls.Add(this.lstLots);
            this.pnlLots.Controls.Add(this.lblAvailableLots);
            this.pnlLots.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLots.Location = new System.Drawing.Point(5, 5);
            this.pnlLots.Name = "pnlLots";
            this.pnlLots.Size = new System.Drawing.Size(250, 555);
            this.pnlLots.TabIndex = 0;
            
            // lblAvailableLots
            this.lblAvailableLots.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.lblAvailableLots.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAvailableLots.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblAvailableLots.Location = new System.Drawing.Point(0, 0);
            this.lblAvailableLots.Name = "lblAvailableLots";
            this.lblAvailableLots.Size = new System.Drawing.Size(248, 25);
            this.lblAvailableLots.TabIndex = 0;
            this.lblAvailableLots.Text = "Available Lots";
            this.lblAvailableLots.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            
            // lstLots
            this.lstLots.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstLots.FormattingEnabled = true;
            this.lstLots.ItemHeight = 17;
            this.lstLots.Location = new System.Drawing.Point(0, 25);
            this.lstLots.Name = "lstLots";
            this.lstLots.Size = new System.Drawing.Size(248, 528);
            this.lstLots.TabIndex = 1;
            this.lstLots.SelectedIndexChanged += new System.EventHandler(this.lstLots_SelectedIndexChanged);
            
            // pnlSequences
            this.pnlSequences.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSequences.Controls.Add(this.lstSequences);
            this.pnlSequences.Controls.Add(this.lblAvailableSequences);
            this.pnlSequences.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSequences.Location = new System.Drawing.Point(255, 5);
            this.pnlSequences.Name = "pnlSequences";
            this.pnlSequences.Size = new System.Drawing.Size(250, 555);
            this.pnlSequences.TabIndex = 1;
            
            // lblAvailableSequences
            this.lblAvailableSequences.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.lblAvailableSequences.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAvailableSequences.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblAvailableSequences.Location = new System.Drawing.Point(0, 0);
            this.lblAvailableSequences.Name = "lblAvailableSequences";
            this.lblAvailableSequences.Size = new System.Drawing.Size(248, 25);
            this.lblAvailableSequences.TabIndex = 0;
            this.lblAvailableSequences.Text = "Available Sequences";
            this.lblAvailableSequences.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            
            // lstSequences
            this.lstSequences.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstSequences.FormattingEnabled = true;
            this.lstSequences.ItemHeight = 17;
            this.lstSequences.Location = new System.Drawing.Point(0, 25);
            this.lstSequences.Name = "lstSequences";
            this.lstSequences.Size = new System.Drawing.Size(248, 528);
            this.lstSequences.TabIndex = 1;
            this.lstSequences.SelectedIndexChanged += new System.EventHandler(this.lstSequences_SelectedIndexChanged);
            
            // pnlParts
            this.pnlParts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlParts.Controls.Add(this.dgvParts);
            this.pnlParts.Controls.Add(this.lblAvailableParts);
            this.pnlParts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlParts.Location = new System.Drawing.Point(505, 5);
            this.pnlParts.Name = "pnlParts";
            this.pnlParts.Size = new System.Drawing.Size(690, 555);
            this.pnlParts.TabIndex = 2;
            
            // lblAvailableParts
            this.lblAvailableParts.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.lblAvailableParts.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAvailableParts.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblAvailableParts.Location = new System.Drawing.Point(0, 0);
            this.lblAvailableParts.Name = "lblAvailableParts";
            this.lblAvailableParts.Size = new System.Drawing.Size(688, 25);
            this.lblAvailableParts.TabIndex = 0;
            this.lblAvailableParts.Text = "Available Parts";
            this.lblAvailableParts.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            
            // dgvParts
            this.dgvParts.AllowUserToAddRows = false;
            this.dgvParts.AllowUserToDeleteRows = false;
            this.dgvParts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvParts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvParts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvParts.Location = new System.Drawing.Point(0, 25);
            this.dgvParts.Name = "dgvParts";
            this.dgvParts.ReadOnly = true;
            this.dgvParts.RowHeadersWidth = 51;
            this.dgvParts.RowTemplate.Height = 25;
            this.dgvParts.Size = new System.Drawing.Size(688, 528);
            this.dgvParts.TabIndex = 1;
            
            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 652);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlJobInfo);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Form1";
            this.Text = "PlateTools AI";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.pnlJobInfo.ResumeLayout(false);
            this.pnlJobInfo.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.pnlParts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvParts)).EndInit();
            this.pnlSequences.ResumeLayout(false);
            this.pnlLots.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnProcessJob;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.ToolStripButton btnClearAll;
        private System.Windows.Forms.Panel pnlJobInfo;
        private System.Windows.Forms.Label lblJobNumberValue;
        private System.Windows.Forms.Label lblJobNumberLabel;
        private System.Windows.Forms.Label lblGroupValue;
        private System.Windows.Forms.Label lblGroupLabel;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Panel pnlLots;
        private System.Windows.Forms.ListBox lstLots;
        private System.Windows.Forms.Label lblAvailableLots;
        private System.Windows.Forms.Panel pnlSequences;
        private System.Windows.Forms.ListBox lstSequences;
        private System.Windows.Forms.Label lblAvailableSequences;
        private System.Windows.Forms.Panel pnlParts;
        private System.Windows.Forms.DataGridView dgvParts;
        private System.Windows.Forms.Label lblAvailableParts;
    }
}
