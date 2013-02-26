using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace SkypeResearcher
{
    public partial class SkypeResearcher : Form
    {
        public SkypeResearcher()
        {
            InitializeComponent();

            _dirName = Environment.GetEnvironmentVariable("APPDATA") + @"\Skype";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            if (Directory.Exists(_dirName))
            {
                _RetrieveInfoFromSkypeFolder();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void _RetrieveInfoFromSkypeFolder()
        {
            try
            {
                if (Directory.Exists(_dirName))
                {
                    string[] folders = Directory.GetDirectories(_dirName);

                    foreach (string fname in folders)
                    {
                        string folder = fname.Replace(_dirName, string.Empty).Trim('\\');

                        if (folder != "Content" &&
                            folder != "My Skype Received Files" &&
                            folder != "Pictures" &&
                            folder != "DbTemp")
                        {
                            usernamecb.Items.Add(folder);
                        }
                    }

                    if (usernamecb.Items.Count > 0)
                    {
                        usernamecb.SelectedIndex = -1;
                        usernamecb.SelectedIndexChanged += usernamecb_SelectedIndexChanged;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void usernamecb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (usernamecb.SelectedIndex > -1)
            {
                string skypeUserName = usernamecb.Items[usernamecb.SelectedIndex].ToString();
                string database = _dirName + @"\" + skypeUserName;

                string main = Application.UserAppDataPath + @"\main.db";
                string bistats = Application.UserAppDataPath + @"\bistats.db";
                string dc = Application.UserAppDataPath + @"\dc.db";
                string eas = Application.UserAppDataPath + @"\eas.db";
                string keyval = Application.UserAppDataPath + @"\keyval.db";
                string msn = Application.UserAppDataPath + @"\msn.db";

                File.Copy(database + @"\main.db", main, true);
                File.Copy(database + @"\bistats.db", bistats, true);
                File.Copy(database + @"\dc.db", dc, true);
                File.Copy(database + @"\eas.db", eas, true);
                File.Copy(database + @"\keyval.db", keyval, true);
                File.Copy(database + @"\msn.db", msn, true);

                if (File.Exists(main))
                {
                    _FillContacts(main);

                    dataGridView1.DataSource = _GetTablesInfo(dataGridView1, main);
                    dataGridView3.DataSource = _GetTablesInfo(dataGridView1, bistats);
                    dataGridView4.DataSource = _GetTablesInfo(dataGridView1, dc);
                    dataGridView5.DataSource = _GetTablesInfo(dataGridView1, eas);
                    dataGridView6.DataSource = _GetTablesInfo(dataGridView1, keyval);
                    dataGridView7.DataSource = _GetTablesInfo(dataGridView1, msn);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void participants_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (usernamecb.SelectedIndex > -1 && participants.SelectedIndex > -1)
            {
                string skypeUserName = usernamecb.SelectedItem.ToString();
                string participant = participants.SelectedValue.ToString();
                string database = _dirName + @"\" + skypeUserName + @"\main.db";

                if (File.Exists(database))
                {
                    SQLiteConnection sqlite = new SQLiteConnection("data source=" + database);
                    SQLiteDataAdapter ad;
                    DataTable dt = new DataTable();
                    SQLiteCommand cmd;

                    try
                    {
                        sqlite.Open();
                        cmd = sqlite.CreateCommand();
                        // 'guid' field dropped since Exception
                        cmd.CommandText = @"select id,is_permanent,convo_id,chatname,author,from_dispname,author_was_live,dialog_partner,
                        timestamp,type,sending_status,consumption_status,edited_by,edited_timestamp,param_key,param_value,
                        body_xml,identities,reason,leavereason,participant_count,error_code,chatmsg_type,chatmsg_status,
                        body_is_rawxml,oldoptions,newoptions,newrole,pk_id,crc,remote_id,call_guid,extprop_contact_review_date,
                        extprop_contact_received_stamp,extprop_contact_reviewed
                        from Messages Where dialog_partner = '"
                            + participant + "'";
                        ad = new SQLiteDataAdapter(cmd);
                        ad.Fill(dt);

                        dataGridView2.DataSource = dt;
                        tabControl1.SelectedTab = tabControl1.TabPages["tabMessages"];
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        private void _FillContacts(string database)
        {
            SQLiteConnection sqlite = new SQLiteConnection("data source=" + database);

            try
            {
                sqlite.Open();

                //
                SQLiteCommand cmd = sqlite.CreateCommand();
                cmd.CommandText = "select DISTINCT dialog_partner from Messages;";

                SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd);

                DataTable dt = new DataTable();
                ad.Fill(dt);

                participants.DataSource = dt;
                participants.ValueMember = "dialog_partner";

                participants.SelectedIndex = -1;
                participants.SelectedIndexChanged += participants_SelectedIndexChanged;
            }
            catch (Exception)
            {
            }
        }

        void f()
        {
string path = "[PATH_TO_USERDATA]";
DataTable dt = new DataTable();
SQLiteConnection sqlite = new SQLiteConnection("data source=" + path);
sqlite.Open();

SQLiteDataAdapter ad;
SQLiteCommand cmd = sqlite.CreateCommand();
cmd.CommandText = @"select * from Messages";

ad = new SQLiteDataAdapter(cmd);
ad.Fill(dt);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        private DataTable _GetTablesInfo(DataGridView dataGridView, string database)
        {
            DataTable dt = new DataTable();
            SQLiteConnection sqlite = new SQLiteConnection("data source=" + database);

            try
            {
                sqlite.Open();

                //
                SQLiteCommand cmd = sqlite.CreateCommand();
                cmd.CommandText = "SELECT * FROM sqlite_master WHERE type='table';";

                SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd);

                ad.Fill(dt);
            }
            catch (Exception)
            {
            }

            return dt;
        }

        private string _dirName;
    }
}
