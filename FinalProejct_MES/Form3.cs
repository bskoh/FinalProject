using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MySql.Data.MySqlClient;
using System.Data;

namespace FinalProejct_MES
{
    public partial class Form3 : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ShowScrollBar(System.IntPtr hWnd, int wBar, bool bShow);
        private const uint SB_HORZ = 0;
        private const uint SB_VERT = 1;
        private const uint ESB_ENABLE_BOTH = 0x3;

        public Form3()
        {
            InitializeComponent();

            //스크롤바 세로만 나오게 하기
            this.lst_AGV.Scrollable = false;
            this.lst_stage.Scrollable = false;
            this.lst_inspection.Scrollable = false;
            ShowScrollBar(this.lst_AGV.Handle, (int)SB_VERT, true);
            ShowScrollBar(this.lst_stage.Handle, (int)SB_VERT, true);
            ShowScrollBar(this.lst_inspection.Handle, (int)SB_VERT, true);
        }

        //DB 관련
        MySqlConnection conn;
        MySqlCommand cmd;
        MySqlDataReader reader;
        MySqlDataAdapter da; public string strConn { get; set; }

        public string server, user, pwd;

        public string[] Col_AGV = new string[] { "NowTime", "Num_AGV", "NowLocation", "Exist", "Num_Product" };
        public string[] Col_Stage = new string[] { "NowTime", "Exist_Pickstage", "Exist_Dropstage", "Status_Conv1", "Status_Conv2" };
        public string[] Col_Inspection = new string[] { "Inspection_time", "Num_Product", "Image", "Result" };

        int Num_Product = 0;

        //Form1 frm1 = new Form1();

        public bool DB_insert(string table, string[] column, string[] data)
        {
            if (column.Length == data.Length)
            {
                string sql = "Insert into " + table + "(";

                for (int i = 0; i < column.Length; i++)
                {
                    if (i == column.Length - 1)
                        sql += column[i] + ") ";
                    else
                        sql += column[i] + ", ";
                }
                sql += "values(";

                for (int i = 0; i < column.Length; i++)
                {
                    if (i == column.Length - 1)
                        sql += "@" + column[i] + ")";
                    else
                        sql += "@" + column[i] + ", ";
                }

                cmd.Parameters.Clear();

                //desc로 data 타입 받아오기
                string[] strFieldType = DB_getFieldType(table, column);

                for (int i = 0; i < column.Length; i++)
                {
                    //datatype 없으면 추가하면됨
                    if (strFieldType[i].Contains("datetime"))
                    {
                        cmd.Parameters.Add("@" + column[i], MySqlDbType.DateTime);
                        cmd.Parameters["@" + column[i]].Value = data[i];
                    }
                    else if (strFieldType[i].Contains("tinyint"))
                    {
                        if (strFieldType[i].Contains("unsigned"))
                        {
                            cmd.Parameters.Add("@" + column[i], MySqlDbType.UInt16);
                            cmd.Parameters["@" + column[i]].Value = uint.Parse(data[i]);
                        }
                        else
                        {
                            cmd.Parameters.Add("@" + column[i], MySqlDbType.Int16);
                            cmd.Parameters["@" + column[i]].Value = int.Parse(data[i]);
                        }
                    }
                    else if (strFieldType[i].Contains("int"))
                    {
                        if (strFieldType[i].Contains("unsigned"))
                        {
                            cmd.Parameters.Add("@" + column[i], MySqlDbType.UInt64);
                            cmd.Parameters["@" + column[i]].Value = uint.Parse(data[i]);
                        }
                        else
                        {
                            cmd.Parameters.Add("@" + column[i], MySqlDbType.Int64);
                            cmd.Parameters["@" + column[i]].Value = int.Parse(data[i]);
                        }
                    }
                    else if (strFieldType[i].Contains("char"))
                    {
                        int temp = int.Parse(strFieldType[i].Substring(5, strFieldType[i].IndexOf(")") - 5));
                        cmd.Parameters.Add("@" + column[i], MySqlDbType.VarChar, temp);
                        cmd.Parameters["@" + column[i]].Value = data[i];
                    }
                    else if (strFieldType[i].Contains("blob"))
                    {
                        //TODO: Image DB에 넣기..
                        /*
                        cmd.Parameters.Add("@" + column[i], MySqlDbType.LongBlob);
                        frm1.Blob_image = frm1.ms.ToArray();
                        cmd.Parameters["@" + column[i]].Value = frm1.Blob_image;
                        */
                    }
                    else
                    {
                        cmd.Parameters.Add("@" + column[i], MySqlDbType.Text);
                        cmd.Parameters["@" + column[i]].Value = data[i];
                    }
                }
                cmd.CommandText = sql;
                if (cmd.ExecuteNonQuery() == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private string[] DB_getFieldType(string TableName, string[] columns)
        {
            string[] arr = { };

            for (int i = 0; i < columns.Length; i++)
            {
                cmd.CommandText = "show columns from " +
                    TableName +
                    " where field = " +
                    "'" + columns[i] +"'";

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Array.Resize(ref arr, arr.Length + 1);
                    arr[arr.Length - 1] = reader["Type"].ToString();
                }

                reader.Close();
            }
            return arr;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            //DB 접속
            server = "127.0.0.1";
            user = "root";
            pwd = "1234";

            strConn = "Server=" + server + ";uid=" + user + ";pwd=" + pwd + ";CHARSET=UTF8" + ";DATABASE=SmartFactory";
            conn = new MySqlConnection(strConn);
            try
            {
                conn.Open();
                cmd = new MySqlCommand("", conn);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DB접속 실패 (예외 발생)");
            }

            if (conn.State == ConnectionState.Open)
            {
                MessageBox.Show("DB Login이 되었습니다.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Text += "  // Server : " + server + " // User : " + user;
                //tmr_DBSelect.Enabled = true;
            }
            else if (conn.State == ConnectionState.Closed)
            {
                MessageBox.Show("DB Login을 실패했습니다..", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
        private void tmr_DBSelect_Tick(object sender, EventArgs e)
        {
            try
            {
                DB_select("AGV", lst_AGV);
                DB_select("Stage", lst_stage);
                DB_select("Inspection", lst_inspection);
            }
            catch
            {
                
            }
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmr_DBSelect.Enabled = false;
        }

        private string[] DB_MakeColNameArray(string TableName)
        {
            string[] arr = { };
            cmd.CommandText = "DESC " + TableName;
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Array.Resize(ref arr, arr.Length + 1);
                arr[arr.Length - 1] = reader["Field"].ToString();

            }
            reader.Close();
            return arr;
        }
        public void DB_select(string table, ListView lst)
        {

            //열 이름 목록 만들기
            string[] ColNames = { };
            cmd.CommandText = "DESC " + table;

            //listview 설정
            lst.Clear();
            
            //lst.BeginUpdate();

            lst.View = View.Details;

            string[] column_name = DB_MakeColNameArray(table);

            foreach (string col in column_name)
            {
                lst.Columns.Add(col, 150, HorizontalAlignment.Center);
            }

            //lst.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            //lst.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            lst.Columns[0].Width = 200;



            //lst.EndUpdate();

            lst.BeginUpdate();

            //데이터 조회 후 화면에 출력
            if (table.ToUpper() == "AGV" || table.ToUpper() == "STAGE")
            {
                cmd.CommandText = "select * from " + table + " ORDER BY NowTime DESC";
            }
            else
            {
                cmd.CommandText = "select * from " + table + " ORDER BY Inspection_time DESC";
            }
            reader = cmd.ExecuteReader();


            ListViewItem item;

            while (reader.Read())
            {
                string[] datastr = { };

                foreach (string col in column_name)
                {
                    Array.Resize(ref datastr, datastr.Length + 1);
                    datastr[datastr.Length - 1] = reader[col].ToString();
                }

                for (int i = 0; i < datastr.Length; i = i + datastr.Length)
                {
                    item = new ListViewItem(datastr[i]);
                    for (int j = 1; j < datastr.Length; j++)
                    {
                        item.SubItems.Add(datastr[j]);
                    }

                    //listview에 넣는다.
                    lst.Items.Add(item);
                }
                lst.EndUpdate();
            }
            reader.Close();
        }

        private void btn_Read_log_Click(object sender, EventArgs e)
        {
            DB_select("AGV", lst_AGV);
            DB_select("Stage", lst_stage);
            DB_select("Inspection", lst_inspection);

        }

        private void chkbox_Log_CheckedChanged(object sender, EventArgs e)
        {
            if (chkbox_Log.Checked == true)
            {
                tmr_DBSelect.Enabled = true;
            }
            else
            {
                tmr_DBSelect.Enabled = false;
            }
        }

        private void lst_AGV_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView lst = sender as ListView;
            try
            {
               if(lst_stage.Items[0].Text == lst.FocusedItem.Text)
                {
                    lst_stage.SelectedItems[lst.FocusedItem.Index].Selected = true;
                }
            }
            catch
            {

            }
        }

        public void ADD_AGVComm(string str)
        {
            //lst_AGVComm.EndUpdate();
            //lst_AGVComm.BeginUpdate();
            lst_AGVComm.Items.Add(DateTime.Now.ToString("HH:mm:ss.fff ") + str); //tt는 AMPM
            lst_AGVComm.SelectedIndex = lst_AGVComm.Items.Count - 1;
            if (lst_AGVComm.Items.Count > 1000)
            {
                lst_AGVComm.Items.Clear();
            }

            //lst_AGVComm.EndUpdate();
        }
        public void ADD_StageComm(string str)
        {
            //lst_StageComm.EndUpdate();
            //lst_StageComm.BeginUpdate();
            lst_StageComm.Items.Add(DateTime.Now.ToString("HH:mm:ss.fff ") + str);
            lst_StageComm.SelectedIndex = lst_StageComm.Items.Count - 1;
            if (lst_StageComm.Items.Count > 1000)
            {
                lst_StageComm.Items.Clear();
            }
            //lst_StageComm.EndUpdate();
        }
    }
}
