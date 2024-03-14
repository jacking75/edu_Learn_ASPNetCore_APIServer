using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Dapper;
using MySqlConnector;
using System.Data.OleDb;

namespace Excel2DbTool
{
    public partial class Form1 : Form
    {
        public Dictionary<String, DataTable> dictExcel = new Dictionary<String, DataTable>();
        private MySqlConnection _sqlConnection;

        public Form1()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string GameDBConnectionString = ConfigurationManager.AppSettings["DBConnection"];
                _sqlConnection = new MySqlConnection(GameDBConnectionString);
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.ToString();
                MessageBox.Show($"Failed to connect to database: {ErrorMessage}");
            }
        }

        String trimSheetName(String sheetName)
        {
            if (sheetName.LastIndexOf('$') == sheetName.Length - 1)
            {
                sheetName = sheetName.Substring(0, sheetName.Length - 1);
            }

            return sheetName;
        }

        public string getConnection(string fileName, string fileExt)
        {
            var connectionString = string.Empty;

            if (fileExt.CompareTo(".xls") == 0)
            {
                connectionString =
                    $@"provider=Microsoft.Jet.OLEDB.4.0;Data Source={fileName};Extended Properties='Excel 8.0;HDR=YES;IMEX=1;';";
            }
            else
            {
                connectionString =
                    $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fileName};Extended Properties='Excel 12.0;HDR=YES;';";
            }

            return connectionString;
        }

        public DataTable ReadSheet(String sheetName, OleDbConnection connection)
        {
            sheetName = trimSheetName(sheetName);

            var dtexcel = new DataTable();
            var oleAdpt = new OleDbDataAdapter($"select * from [{sheetName}$]", connection);
            oleAdpt.Fill(dtexcel);
            return dtexcel;
        }

        public String[] GetSheetName(OleDbConnection connection)
        {
            var infoTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            var sheet = new String[infoTable.Rows.Count];

            var idx = 0;
            foreach (DataRow row in infoTable.Rows)
            {
                var sheetName = row["TABLE_NAME"].ToString();
                sheet[idx++] = sheetName;
            }
            return sheet;
        }

        void ReadData(String[] sheetNameList, OleDbConnection connection)
        {

            foreach (String sheetName in sheetNameList)
            {
                var sheet = ReadSheet(sheetName, connection);
                dictExcel.Add(sheetName, sheet);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var file = new OpenFileDialog();

            if (file.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            var filePath = file.FileName;
            var fileExt = Path.GetExtension(filePath);

            if (fileExt.CompareTo(".xls") != 0 && fileExt.CompareTo(".xlsx") != 0)
            {
                MessageBox.Show("Please choose .xls or .xlsx file only.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var connectionString = getConnection(filePath, fileExt);
                var connection = new OleDbConnection(connectionString);
                connection.Open();

                var sheetNameList = GetSheetName(connection);
                dictExcel.Clear();
                ReadData(sheetNameList, connection);

                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(sheetNameList);
                if (sheetNameList.Length >= 1)
                {
                    comboBox1.SelectedIndex = 0;
                    dataGridView1.Visible = true;
                    dataGridView1.DataSource = ReadSheet(sheetNameList[0], connection);
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _sqlConnection.Open();

            foreach (KeyValuePair<string, DataTable> entry in dictExcel)
            {
                var sheetName = entry.Key.Substring(0, entry.Key.Length - 1);
                var paramList = "";
                var valueList = "";
                string[] columnNames = entry.Value.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();

                for (int i = 0; i < columnNames.Length; i++)
                {
                    paramList += $"{columnNames[i]},";
                    valueList += $"@{columnNames[i]},";
                }

                paramList = paramList.TrimEnd(',');
                valueList = valueList.TrimEnd(',');

                try
                {
                    var rowLength = entry.Value.Rows.Count;
                    for (int i = 0; i < rowLength; i++)
                    {
                        var parameter = new DynamicParameters();
                        for (int j = 0; j < columnNames.Length; j++)
                        {
                            parameter.Add($"@{columnNames[j]}", entry.Value.Rows[i][j], direction: ParameterDirection.Input);
                        }

                        var sqlQuery = $"INSERT {sheetName} ({paramList}) VALUES({valueList})";
                        var result = _sqlConnection.Execute(sqlQuery, parameter);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"시트 에러 발생 {sheetName}: {ex.ToString()}");
                    continue;
                }

                MessageBox.Show($"{sheetName} 마스터 데이터가 성공적으로 입력됨");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = dictExcel[comboBox1.Text.Trim()];
        }
    }
}