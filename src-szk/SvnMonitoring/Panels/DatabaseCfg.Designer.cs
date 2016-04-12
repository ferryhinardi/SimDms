namespace SCHEMON.Panels
{
    partial class DatabaseCfg
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCheck = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.lbSmsDBStatus = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSMSDB = new System.Windows.Forms.TextBox();
            this.txtSMSSrv = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnEmpDbConnect = new System.Windows.Forms.Button();
            this.lbEmpDbStatus = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtEmpDB = new System.Windows.Forms.TextBox();
            this.txtEmpSrv = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnPrjDbConnect = new System.Windows.Forms.Button();
            this.lbPrjDbStatus = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtPrjDB = new System.Windows.Forms.TextBox();
            this.txtPrjSrv = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.dgSynch = new System.Windows.Forms.DataGridView();
            this.pgImport = new System.Windows.Forms.ProgressBar();
            this.lbProgress = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgSynch)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCheck);
            this.groupBox1.Controls.Add(this.btnImport);
            this.groupBox1.Controls.Add(this.lbSmsDBStatus);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtSMSDB);
            this.groupBox1.Controls.Add(this.txtSMSSrv);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(10, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(653, 83);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SMS Centre Database";
            // 
            // btnCheck
            // 
            this.btnCheck.Enabled = false;
            this.btnCheck.Location = new System.Drawing.Point(480, 52);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(74, 23);
            this.btnCheck.TabIndex = 7;
            this.btnCheck.Text = "Check";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // btnImport
            // 
            this.btnImport.Enabled = false;
            this.btnImport.Location = new System.Drawing.Point(556, 52);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(74, 23);
            this.btnImport.TabIndex = 6;
            this.btnImport.Text = "&Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // lbSmsDBStatus
            // 
            this.lbSmsDBStatus.AutoSize = true;
            this.lbSmsDBStatus.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSmsDBStatus.ForeColor = System.Drawing.Color.Green;
            this.lbSmsDBStatus.Location = new System.Drawing.Point(533, 24);
            this.lbSmsDBStatus.Name = "lbSmsDBStatus";
            this.lbSmsDBStatus.Size = new System.Drawing.Size(95, 16);
            this.lbSmsDBStatus.TabIndex = 5;
            this.lbSmsDBStatus.Text = "CONNECTED";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(478, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Status:";
            // 
            // txtSMSDB
            // 
            this.txtSMSDB.Location = new System.Drawing.Point(133, 53);
            this.txtSMSDB.Name = "txtSMSDB";
            this.txtSMSDB.ReadOnly = true;
            this.txtSMSDB.Size = new System.Drawing.Size(270, 21);
            this.txtSMSDB.TabIndex = 3;
            this.txtSMSDB.Text = "test";
            // 
            // txtSMSSrv
            // 
            this.txtSMSSrv.Location = new System.Drawing.Point(133, 24);
            this.txtSMSSrv.Name = "txtSMSSrv";
            this.txtSMSSrv.ReadOnly = true;
            this.txtSMSSrv.Size = new System.Drawing.Size(270, 21);
            this.txtSMSSrv.TabIndex = 2;
            this.txtSMSSrv.Text = "(local)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Database Name:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server Name:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnEmpDbConnect);
            this.groupBox2.Controls.Add(this.lbEmpDbStatus);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtEmpDB);
            this.groupBox2.Controls.Add(this.txtEmpSrv);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(10, 95);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(653, 83);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Employee Database";
            // 
            // btnEmpDbConnect
            // 
            this.btnEmpDbConnect.Location = new System.Drawing.Point(481, 50);
            this.btnEmpDbConnect.Name = "btnEmpDbConnect";
            this.btnEmpDbConnect.Size = new System.Drawing.Size(94, 25);
            this.btnEmpDbConnect.TabIndex = 6;
            this.btnEmpDbConnect.Text = "Connect";
            this.btnEmpDbConnect.UseVisualStyleBackColor = true;
            this.btnEmpDbConnect.Click += new System.EventHandler(this.btnEmpDbConnect_Click);
            // 
            // lbEmpDbStatus
            // 
            this.lbEmpDbStatus.AutoSize = true;
            this.lbEmpDbStatus.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbEmpDbStatus.ForeColor = System.Drawing.Color.Red;
            this.lbEmpDbStatus.Location = new System.Drawing.Point(533, 24);
            this.lbEmpDbStatus.Name = "lbEmpDbStatus";
            this.lbEmpDbStatus.Size = new System.Drawing.Size(55, 16);
            this.lbEmpDbStatus.TabIndex = 5;
            this.lbEmpDbStatus.Text = "CLOSE";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(478, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Status:";
            // 
            // txtEmpDB
            // 
            this.txtEmpDB.Location = new System.Drawing.Point(133, 53);
            this.txtEmpDB.Name = "txtEmpDB";
            this.txtEmpDB.ReadOnly = true;
            this.txtEmpDB.Size = new System.Drawing.Size(270, 21);
            this.txtEmpDB.TabIndex = 3;
            this.txtEmpDB.Text = "test";
            // 
            // txtEmpSrv
            // 
            this.txtEmpSrv.Location = new System.Drawing.Point(133, 24);
            this.txtEmpSrv.Name = "txtEmpSrv";
            this.txtEmpSrv.ReadOnly = true;
            this.txtEmpSrv.Size = new System.Drawing.Size(270, 21);
            this.txtEmpSrv.TabIndex = 2;
            this.txtEmpSrv.Text = "(local)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(25, 56);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(103, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Database Name:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(40, 27);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Server Name:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnPrjDbConnect);
            this.groupBox3.Controls.Add(this.lbPrjDbStatus);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.txtPrjDB);
            this.groupBox3.Controls.Add(this.txtPrjSrv);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(10, 184);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(653, 83);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Project Database";
            // 
            // btnPrjDbConnect
            // 
            this.btnPrjDbConnect.Location = new System.Drawing.Point(481, 49);
            this.btnPrjDbConnect.Name = "btnPrjDbConnect";
            this.btnPrjDbConnect.Size = new System.Drawing.Size(94, 25);
            this.btnPrjDbConnect.TabIndex = 7;
            this.btnPrjDbConnect.Text = "Connect";
            this.btnPrjDbConnect.UseVisualStyleBackColor = true;
            this.btnPrjDbConnect.Click += new System.EventHandler(this.btnPrjDbConnect_Click);
            // 
            // lbPrjDbStatus
            // 
            this.lbPrjDbStatus.AutoSize = true;
            this.lbPrjDbStatus.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPrjDbStatus.ForeColor = System.Drawing.Color.Red;
            this.lbPrjDbStatus.Location = new System.Drawing.Point(533, 24);
            this.lbPrjDbStatus.Name = "lbPrjDbStatus";
            this.lbPrjDbStatus.Size = new System.Drawing.Size(55, 16);
            this.lbPrjDbStatus.TabIndex = 5;
            this.lbPrjDbStatus.Text = "CLOSE";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(478, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(48, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Status:";
            // 
            // txtPrjDB
            // 
            this.txtPrjDB.Location = new System.Drawing.Point(133, 53);
            this.txtPrjDB.Name = "txtPrjDB";
            this.txtPrjDB.ReadOnly = true;
            this.txtPrjDB.Size = new System.Drawing.Size(270, 21);
            this.txtPrjDB.TabIndex = 3;
            this.txtPrjDB.Text = "test";
            // 
            // txtPrjSrv
            // 
            this.txtPrjSrv.Location = new System.Drawing.Point(133, 24);
            this.txtPrjSrv.Name = "txtPrjSrv";
            this.txtPrjSrv.ReadOnly = true;
            this.txtPrjSrv.Size = new System.Drawing.Size(270, 21);
            this.txtPrjSrv.TabIndex = 2;
            this.txtPrjSrv.Text = "(local)";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(25, 56);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(103, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Database Name:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(40, 27);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(88, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "Server Name:";
            // 
            // dgSynch
            // 
            this.dgSynch.AllowUserToAddRows = false;
            this.dgSynch.AllowUserToDeleteRows = false;
            this.dgSynch.AllowUserToResizeRows = false;
            this.dgSynch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgSynch.Location = new System.Drawing.Point(10, 282);
            this.dgSynch.Name = "dgSynch";
            this.dgSynch.Size = new System.Drawing.Size(653, 202);
            this.dgSynch.TabIndex = 5;
            // 
            // pgImport
            // 
            this.pgImport.Location = new System.Drawing.Point(143, 368);
            this.pgImport.Name = "pgImport";
            this.pgImport.Size = new System.Drawing.Size(404, 22);
            this.pgImport.TabIndex = 6;
            this.pgImport.Value = 75;
            this.pgImport.Visible = false;
            // 
            // lbProgress
            // 
            this.lbProgress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lbProgress.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProgress.Location = new System.Drawing.Point(145, 393);
            this.lbProgress.Name = "lbProgress";
            this.lbProgress.Size = new System.Drawing.Size(402, 23);
            this.lbProgress.TabIndex = 7;
            this.lbProgress.Text = "sample import data employee: 234 / 1250";
            this.lbProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbProgress.Visible = false;
            // 
            // DatabaseCfg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.lbProgress);
            this.Controls.Add(this.pgImport);
            this.Controls.Add(this.dgSynch);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "DatabaseCfg";
            this.Size = new System.Drawing.Size(671, 487);
            this.Load += new System.EventHandler(this.DatabaseCfg_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgSynch)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbSmsDBStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSMSDB;
        private System.Windows.Forms.TextBox txtSMSSrv;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lbEmpDbStatus;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtEmpDB;
        private System.Windows.Forms.TextBox txtEmpSrv;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lbPrjDbStatus;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtPrjDB;
        private System.Windows.Forms.TextBox txtPrjSrv;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnEmpDbConnect;
        private System.Windows.Forms.Button btnPrjDbConnect;
        private System.Windows.Forms.DataGridView dgSynch;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.ProgressBar pgImport;
        private System.Windows.Forms.Label lbProgress;
        private System.Windows.Forms.Button btnCheck;

    }
}
