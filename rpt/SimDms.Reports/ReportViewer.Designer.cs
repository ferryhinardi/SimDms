namespace SimDms.Reports
{
    partial class ReportViewer
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
            this.ribbonPanel2 = new System.Windows.Forms.Button();
            this.ribbonPanel3 = new System.Windows.Forms.Button();
            this.ribbonPanel4 = new System.Windows.Forms.Button();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.ribbonPanel5 = new System.Windows.Forms.Button();
            this.ribbonPanel6 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ribbonPanel2
            // 
            this.ribbonPanel2.Location = new System.Drawing.Point(218, 12);
            this.ribbonPanel2.Name = "ribbonPanel2";
            this.ribbonPanel2.Size = new System.Drawing.Size(75, 23);
            this.ribbonPanel2.TabIndex = 0;
            this.ribbonPanel2.Text = "Birthday Call";
            this.ribbonPanel2.Click += new System.EventHandler(this.ribbonPanel2_Click);
            // 
            // ribbonPanel3
            // 
            this.ribbonPanel3.Location = new System.Drawing.Point(11, 41);
            this.ribbonPanel3.Name = "ribbonPanel3";
            this.ribbonPanel3.Size = new System.Drawing.Size(75, 23);
            this.ribbonPanel3.TabIndex = 1;
            this.ribbonPanel3.Text = "Holiday Call";
            this.ribbonPanel3.Click += new System.EventHandler(this.ribbonPanel3_Click);
            // 
            // ribbonPanel4
            // 
            this.ribbonPanel4.Location = new System.Drawing.Point(115, 41);
            this.ribbonPanel4.Name = "ribbonPanel4";
            this.ribbonPanel4.Size = new System.Drawing.Size(75, 23);
            this.ribbonPanel4.TabIndex = 2;
            this.ribbonPanel4.Text = "STNK EXT CALL";
            this.ribbonPanel4.Click += new System.EventHandler(this.ribbonPanel4_Click);
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.reportViewer1.Location = new System.Drawing.Point(0, 134);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(886, 524);
            this.reportViewer1.TabIndex = 2;
            // 
            // ribbonPanel5
            // 
            this.ribbonPanel5.Location = new System.Drawing.Point(115, 12);
            this.ribbonPanel5.Name = "ribbonPanel5";
            this.ribbonPanel5.Size = new System.Drawing.Size(75, 23);
            this.ribbonPanel5.TabIndex = 3;
            this.ribbonPanel5.Text = "BPKB NOTIFICATION";
            this.ribbonPanel5.Click += new System.EventHandler(this.ribbonPanel5_Click);
            // 
            // ribbonPanel6
            // 
            this.ribbonPanel6.Location = new System.Drawing.Point(11, 12);
            this.ribbonPanel6.Name = "ribbonPanel6";
            this.ribbonPanel6.Size = new System.Drawing.Size(75, 23);
            this.ribbonPanel6.TabIndex = 4;
            this.ribbonPanel6.Text = "3 DAYS CALL";
            this.ribbonPanel6.Click += new System.EventHandler(this.ribbonPanel6_Click);
            // 
            // ReportViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 658);
            this.Controls.Add(this.ribbonPanel2);
            this.Controls.Add(this.ribbonPanel3);
            this.Controls.Add(this.ribbonPanel4);
            this.Controls.Add(this.ribbonPanel5);
            this.Controls.Add(this.ribbonPanel6);
            this.Controls.Add(this.reportViewer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ReportViewer";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.ReportViewer_Load);
            this.ResumeLayout(false);

        }

        #endregion

        //private System.Windows.Forms.Ribbon ribbon1;
        //private System.Windows.Forms.RibbonTab ribbonTab1;
        //private System.Windows.Forms.RibbonPanel ribbonPanel1;
        //private System.Windows.Forms.RibbonButton bdaybtn;
        //private System.Windows.Forms.RibbonButton holidaybtn;
        //private System.Windows.Forms.Ribbon ribbon2;
        //private System.Windows.Forms.RibbonTab ribbonTab2;
        private System.Windows.Forms.Button ribbonPanel2;
        private System.Windows.Forms.Button ribbonPanel3;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.Button ribbonPanel4;
        private System.Windows.Forms.Button ribbonPanel5;
        private System.Windows.Forms.Button ribbonPanel6;
    }
}

