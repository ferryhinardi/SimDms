namespace SQLtoModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    public class FrmMain : Form
    {
        private Button btnProcess;
        private ComboBox cbDatabase;
        private ComboBox cbServer;
        private IContainer components;
        private DataSet dsDatabases;
        private DataSet dsModel;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private RadioButton rbColumn;
        private RadioButton rbColumn2;
        private RadioButton rbModel;
        private TextBox txtPassword;
        private TextBox txtResult;
        private TextBox txtTable;
        private TextBox txtUserID;

        public FrmMain()
        {
            this.InitializeComponent();
            this.cbServer.SelectedIndex = 2;
            this.txtTable.Text = "@uspfn_sp_partinquiry_subsitusi '6006406', '6006401', '09119-10073-000', '0'";
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            this.txtResult.Clear();
            string text = string.Empty;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder {
                DataSource = this.cbServer.Text,
                InitialCatalog = this.cbDatabase.Text,
                UserID = this.txtUserID.Text,
                Password = this.txtPassword.Text
            };
            try
            {
                SqlConnection connection = new SqlConnection(builder.ConnectionString);
                SqlCommand command = new SqlCommand();
                SqlCommand command2 = new SqlCommand();

                if (this.txtTable.Text.StartsWith("@"))
                {
                    command = new SqlCommand(this.txtTable.Text.Substring(1), connection);
                    command.Connection.Open();
                    DataTable table = new DataTable();
                    table.Load(command.ExecuteReader());
                    if (table.Columns.Count > 0)
                    {
                        int n = table.Columns.Count;

                        if (this.rbModel.Checked)
                        {
                            this.txtResult.Text = "public class ModelName" + System.Environment.NewLine + "{" + System.Environment.NewLine;

                            for (int i = 0; i < n; i++)
                            {
                                this.txtResult.Text += '\t' + "public " + (table.Columns[i].DataType.Name == "String" ? "String  " : table.Columns[i].DataType.Name + "?  ") + table.Columns[i].ColumnName + "  { get; set; }" + System.Environment.NewLine;
                            }

                            this.txtResult.Text += "}";
                        }
                         else if (rbColumn2.Checked )
                        {
                            this.txtResult.Text = "";

                            for (int i = 0; i < n; i++)
                            {
                                this.txtResult.Text += "{ field:\"" + table.Columns[i].ColumnName + "\",  title:\"" + table.Columns[i].ColumnName + "\", width:100 }," + System.Environment.NewLine;
                            }

                        }
                        else
                        {
                            this.txtResult.Text = "";
                            n--;

                            for (int i = 0; i < n; i++)
                            {
                                this.txtResult.Text += "{ id:\"" + table.Columns[i].ColumnName + "\",  header:\"" + table.Columns[i].ColumnName + "\", width:100 }," + System.Environment.NewLine;
                            }

                            this.txtResult.Text += "{ id:\"" + table.Columns[n].ColumnName + "\",  header:\"" + table.Columns[n].ColumnName + "\", width:100, fillspace:true }" + System.Environment.NewLine;

                        }
                    }
                }
                else
                {
                    if (this.txtTable.Text.StartsWith("#"))
                    {
                        command = new SqlCommand(string.Format("SELECT COLUMN_NAME, IS_NULLABLE, DATA_TYPE, ORDINAL_POSITION\r\n\t                    FROM tempdb.INFORMATION_SCHEMA.COLUMNS\r\n                        WHERE TABLE_NAME LIKE '{0}%'", this.txtTable.Text), connection);
                        command2 = new SqlCommand(string.Format("SELECT b.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS a\r\n\t                    INNER JOIN tempdb.INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE b\r\n\t                    ON a.CONSTRAINT_NAME = b.CONSTRAINT_NAME\r\n\t                    WHERE a.CONSTRAINT_TYPE = 'PRIMARY KEY'\r\n                        AND a.TABLE_NAME LIKE '{0}%'", this.txtTable.Text), connection);
                    }
                    else
                    {
                        command = new SqlCommand(string.Format("SELECT COLUMN_NAME, IS_NULLABLE, DATA_TYPE, ORDINAL_POSITION\r\n\t                    FROM INFORMATION_SCHEMA.COLUMNS\r\n                        WHERE TABLE_NAME = '{0}'", this.txtTable.Text), connection);
                        command2 = new SqlCommand(string.Format("SELECT b.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS a\r\n\t                    INNER JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE b\r\n\t                    ON a.CONSTRAINT_NAME = b.CONSTRAINT_NAME\r\n\t                    WHERE a.CONSTRAINT_TYPE = 'PRIMARY KEY'\r\n                        AND a.TABLE_NAME = '{0}'", this.txtTable.Text), connection);
                    }
                    command.Connection.Open();
                    DataTable table = new DataTable();
                    table.Load(command.ExecuteReader());
                    this.dsModel.Tables.Clear();
                    this.dsModel.Tables.Add(table);
                    DataTable table2 = new DataTable();
                    table2.Load(command2.ExecuteReader());
                    this.dsModel.Tables.Add(table2);
                    command.Connection.Close();
                    DataRowCollection rows = this.dsModel.Tables[0].Rows;
                    DataRowCollection rows2 = this.dsModel.Tables[1].Rows;
                    List<string> list = new List<string>();
                    for (int i = 0; i < rows2.Count; i++)
                    {
                        list.Add(rows2[i][0].ToString());
                    }

                    if (this.rbModel.Checked)
                        this.txtResult.Text = "public class ModelName" + System.Environment.NewLine + "{" + System.Environment.NewLine;

                    for (int j = 0; j < rows.Count; j++)
                    {
                        string item = rows[j][0].ToString();
                        string isNull = rows[j][1].ToString();
                        string type = rows[j][2].ToString();
                        string str5 = rows[j][3].ToString();
                        bool flag = list.Contains(item);
                        if (this.rbModel.Checked)
                        {
                            string str6 = this.txtResult.Text;
                            this.txtResult.Text = str6 + (flag ? ("[Key]" + Environment.NewLine + string.Format("[Column(Order = {0})]", str5) + Environment.NewLine) : "") + "public " + this.GenerateType(type, isNull) + " " + item + " { get; set; }" + Environment.NewLine;
                        }
                        else if (rbColumn2.Checked)
                        {
                            this.txtResult.Text = this.txtResult.Text + "{" + string.Format( " header: '{0}', dataIndex: '{0}', width: 100, flex: 1", rows[j][0].ToString()) + " }," + Environment.NewLine;
                        }
                        else
                        {
                            this.txtResult.Text = this.txtResult.Text + "{ name: '" + rows[j][0].ToString() + "', type: 'auto' }, " + Environment.NewLine;
                        }
                    }

                    if (this.rbModel.Checked)
                        this.txtResult.Text += "}";

                }
            }
            catch (Exception exception)
            {
                text = exception.Message;
            }
            if (text != string.Empty)
            {
                MessageBox.Show(text);
            }
        }

        private void cbDatabase_Enter(object sender, EventArgs e)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder {
                    DataSource = this.cbServer.Text,
                    InitialCatalog = "master",
                    UserID = this.txtUserID.Text,
                    Password = this.txtPassword.Text
                };
                SqlConnection connection = new SqlConnection(builder.ConnectionString);
                SqlCommand command = new SqlCommand(string.Format("SELECT NAME FROM sys.databases", new object[0]), connection);
                command.Connection.Open();
                DataTable table = new DataTable();
                table.Load(command.ExecuteReader());
                this.dsDatabases.Tables.Clear();
                this.dsDatabases.Tables.Add(table);
                command.Connection.Close();
                DataRowCollection rows = this.dsDatabases.Tables[0].Rows;
                this.cbDatabase.Items.Clear();
                for (int i = 0; i < rows.Count; i++)
                {
                    this.cbDatabase.Items.Add(rows[i][0]);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private string GenerateType(string type, string isNull)
        {
            string str = (type == "uniqueidentifier") ? "Guid" : ((type == "varchar") ? "string" : ((type == "nvarchar") ? "string" : ((type == "numeric") ? "decimal" : ((type == "bit") ? "bool" : ((type == "datetime") ? "DateTime" : ((type == "char") ? "string" : ((type == "text") ? "string" : ((type == "int") ? "int" : ((type == "bigint") ? "long" : ((type == "smallint") ? "short" : string.Empty))))))))));
            if (!new string[] { "varchar", "nvarchar", "char", "text" }.Contains<string>(type))
            {
                str = str + ((isNull == "YES") ? "?" : "");
            }
            return str;
        }

        private void InitializeComponent()
        {
            this.txtUserID = new TextBox();
            this.txtPassword = new TextBox();
            this.label1 = new Label();
            this.label2 = new Label();
            this.label3 = new Label();
            this.label4 = new Label();
            this.label5 = new Label();
            this.txtTable = new TextBox();
            this.btnProcess = new Button();
            this.txtResult = new TextBox();
            this.dsModel = new DataSet();
            this.rbModel = new RadioButton();
            this.rbColumn = new RadioButton();
            this.rbColumn2 = new RadioButton();
            this.cbDatabase = new ComboBox();
            this.dsDatabases = new DataSet();
            this.cbServer = new ComboBox();
            this.dsModel.BeginInit();
            this.dsDatabases.BeginInit();
            base.SuspendLayout();
            this.txtUserID.Location = new Point(180, 0x41);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.Size = new Size(290, 20);
            this.txtUserID.TabIndex = 2;
            this.txtUserID.Text = "sa";
            this.txtPassword.Location = new Point(180, 0x5b);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new Size(290, 20);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.Text = "P4ssw0rd-01";
            this.label1.AutoSize = true;
            this.label1.Location = new Point(12, 0x10);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x6b, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Data Source (Server)";
            this.label2.AutoSize = true;
            this.label2.Location = new Point(12, 0x2a);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x7d, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Initial Catalog (Database)";
            this.label3.AutoSize = true;
            this.label3.Location = new Point(12, 0x44);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x2b, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "User ID";
            this.label4.AutoSize = true;
            this.label4.Location = new Point(12, 0x5e);
            this.label4.Name = "label4";
            this.label4.Size = new Size(0x35, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Password";
            this.label5.AutoSize = true;
            this.label5.Location = new Point(12, 0x79);
            this.label5.Name = "label5";
            this.label5.Size = new Size(0x22, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Table";
            this.txtTable.Location = new Point(60, 0x76);
            this.txtTable.Name = "txtTable";
            this.txtTable.Size = new Size(410, 20);
            this.txtTable.TabIndex = 4;
            this.btnProcess.Location = new Point(400, 0x90);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new Size(0x4b, 0x19);
            this.btnProcess.TabIndex = 7;
            this.btnProcess.Text = "Generate";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new EventHandler(this.btnProcess_Click);
            this.txtResult.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.txtResult.Location = new Point(15, 0xae);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ScrollBars = ScrollBars.Vertical;
            this.txtResult.Size = new Size(460, 480);
            this.txtResult.TabIndex = 11;
            this.txtResult.KeyDown += new KeyEventHandler(this.txtResult_KeyDown);
            this.dsModel.DataSetName = "dsModel1";
            this.rbModel.AutoSize = true;
            this.rbModel.Checked = true;
            this.rbModel.Location = new Point(15, 0x94);
            this.rbModel.Name = "rbModel";
            this.rbModel.Size = new Size(0x36, 0x11);
            this.rbModel.TabIndex = 5;
            this.rbModel.TabStop = true;
            this.rbModel.Text = "Model";
            this.rbModel.UseVisualStyleBackColor = true;
            this.rbColumn.AutoSize = true;
            this.rbColumn.Location = new Point(0x6a, 0x94);
            this.rbColumn.Name = "rbColumn";
            this.rbColumn.Size = new Size(0x41, 0x11);
            this.rbColumn.TabIndex = 6;
            this.rbColumn.Text = "Columns";
            this.rbColumn.UseVisualStyleBackColor = true;

            this.rbColumn2.AutoSize = true;
            this.rbColumn2.Location = new Point(200, 0x94);
            this.rbColumn2.Name = "rbColumn2";
            this.rbColumn2.Size = new Size(0x41, 0x11);
            this.rbColumn2.TabIndex = 6;
            this.rbColumn2.Text = "Columns k-Grid";
            this.rbColumn2.UseVisualStyleBackColor = true;

            this.cbDatabase.FormattingEnabled = true;
            this.cbDatabase.Location = new Point(180, 0x26);
            this.cbDatabase.Name = "cbDatabase";
            this.cbDatabase.Size = new Size(290, 0x15);
            this.cbDatabase.TabIndex = 1;
            this.cbDatabase.Text = "BIT201310";
            this.cbDatabase.Enter += new EventHandler(this.cbDatabase_Enter);
            this.dsDatabases.DataSetName = "dsDatabases1";
            this.cbServer.FormattingEnabled = true;
            this.cbServer.Items.AddRange(new object[] { "hqdmsdev02", "tbsdmsdb01", "tbsdmsap01", "tbdmsdev02" });
            this.cbServer.Location = new Point(180, 12);
            this.cbServer.Name = "cbServer";
            this.cbServer.Size = new Size(290, 0x15);
            this.cbServer.TabIndex = 0;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x149, 0x2ed);
            base.Controls.Add(this.cbServer);
            base.Controls.Add(this.cbDatabase);
            base.Controls.Add(this.rbColumn);
            base.Controls.Add(this.rbColumn2);
            base.Controls.Add(this.rbModel);
            base.Controls.Add(this.txtResult);
            base.Controls.Add(this.btnProcess);
            base.Controls.Add(this.txtTable);
            base.Controls.Add(this.label5);
            base.Controls.Add(this.label4);
            base.Controls.Add(this.label3);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.txtPassword);
            base.Controls.Add(this.txtUserID);
            this.Height = 500;
            this.MinimumSize = new Size(500, 700);
            base.Name = "FrmMain";
            base.SizeGripStyle = SizeGripStyle.Hide;
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "SQL to MVC4 Model";
            this.dsModel.EndInit();
            this.dsDatabases.EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void txtResult_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.A))
            {
                this.txtResult.SelectAll();
            }
        }
    }
}

