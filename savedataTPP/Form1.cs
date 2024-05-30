using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace savedataTPP
{

    public partial class Form1 : Form
    {
        [Obsolete]
        string ConnectionString = ConfigurationSettings.AppSettings["ConnectionString"];
        MySqlConnection Conn;
        MySqlCommand com;
        MySqlTransaction tr;
        StringBuilder sb;
        bool flagclose = false;
        bool flagsavedata = false;
        FolderBrowserDialog Browse = new FolderBrowserDialog();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Show();
            try {
                label1.Text = File.ReadAllText(Application.StartupPath + "\\path.txt");
            } catch {
                label1.Text = "";
            }
            this.Hide();
          /*  notifyIcon1.Visible = true;
            savedata();*/
        }

        private void Form1_Resize(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            if (!flagclose)
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    Hide();
                    notifyIcon1.Visible = true;
                }
            }
            else
            {

            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            DialogResult result = MessageBox.Show("ต้องการปิดโปรแกรมหรือไม่", "Save Data TPP", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result.ToString() == "Yes")
            {
                flagclose = true;
                this.Close();
            }
            else
            {
                timer1.Start();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            select:

            if (Browse.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                label1.Text = Browse.SelectedPath + @"\";
                File.WriteAllText(Application.StartupPath + "\\path.txt", label1.Text);
            }
            else
            {
                if (label1.Text == "")
                {
                    MessageBox.Show("Please select path file!!");
                    goto select;
                }
            }
            timer1.Start();
        }
        public static void DelaymS(int mS)
        {
            //if (GlobalTestingFlag == true)
            //{
            Stopwatch stopwatchDelaymS = new Stopwatch();

            stopwatchDelaymS.Restart();
            while (mS > stopwatchDelaymS.ElapsedMilliseconds)
            {
                if (!stopwatchDelaymS.IsRunning)
                    stopwatchDelaymS.Start();
                Application.DoEvents();
                //if (GlobalTestingFlag == false)
                //    return;
            }
            stopwatchDelaymS.Stop();
            //}
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            savedata();
            timer1.Start();
        }
        void savedata()
        {
            string namefile;
            namefile = "";
            string patch = System.IO.Directory.GetCurrentDirectory();
            string[] txtFiles;
            try
            {
                txtFiles = Directory.GetFiles(label1.Text.Replace(@"\", @"\\"), "*.txt").Select(Path.GetFileName).ToArray();
            }
            catch
            {
                timer1.Stop();
                timer1.Start();
                return;
            }
            if (txtFiles.Length < 1) return;
            for (int i = 0; i < txtFiles.Length; i++)
            {
                string patchfile = "";
                bool catchFlag = false;
                try
                {
                    namefile = txtFiles[i];
                    patchfile = label1.Text.Replace(@"\", @"\\") + namefile;
                    if (namefile.Length != 36)
                    {
                        File.Delete(patchfile);
                        continue;
                    }
                    try
                    {
                        string dataSup = File.ReadAllText(patchfile);
                        dataSup = dataSup.Replace("'", string.Empty);
                        string[] data = dataSup.Split(',');
                        if (data.Length != 11)
                        {
                            File.Delete(patchfile);
                            continue;
                        }
                        if(data[1].Length != 13) {
                            File.Delete(patchfile);
                            continue;
                        }
                        //sent to sql
                        string moDel = data[0];
                        string snNumber = data[1];
                        string dateTimeTest = data[2];
                        string testresult = data[3];
                        string operatorID = data[4];
                        string workOrder = data[5];
                        string testStn = data[6];
                        string failureSymptom = data[7];
                        string fvalue = data[8];
                        string headerNoX = data[9];

                        string[] config = File.ReadAllText(Application.StartupPath + "\\config.txt").Split(',');
                        if (config.Length < 4) return;

                        try
                        {
                            string MyConnection2 = "SERVER=" + config[0] + ";DATABASE=" + config[1] + ";UID=" + config[2] + ";PASSWORD=" + config[3] + ";Allow Zero Datetime=true;";

                            string QueryInsertFT = "insert firsttest(model,serialNumber,dateTime,testResult,operatorID,workOrder,station,failureSymptom,FailValue,headerNo)VALUES ('" + moDel + "','" + snNumber + "','" + dateTimeTest + "','" + testresult + "','" + operatorID + "','" + workOrder + "','" + testStn + "','" + failureSymptom + "','" + fvalue + "','" + headerNoX + "')";
                            string QueryInsertRT = "insert retest(model,serialNumber,dateTime,testResult,operatorID,workOrder,station,failureSymptom,FailValue,headerNo)VALUES ('" + moDel + "','" + snNumber + "','" + dateTimeTest + "','" + testresult + "','" + operatorID + "','" + workOrder + "','" + testStn + "','" + failureSymptom + "','" + fvalue + "','" + headerNoX + "')";
                            string QueryCheckTB = "select * from firsttest where serialNumber = '" + snNumber + "' ";
                            MySqlConnection MyConn2 = new MySqlConnection(MyConnection2);

                            MySqlCommand MyCommand1 = new MySqlCommand(QueryCheckTB, MyConn2);
                            MySqlCommand MyCommand2 = new MySqlCommand(QueryInsertFT, MyConn2);
                            MySqlCommand MyCommand3 = new MySqlCommand(QueryInsertRT, MyConn2);

                            try
                            {
                                MyConn2.Open();
                                MySqlDataReader cr = MyCommand1.ExecuteReader();

                                if (cr.HasRows == true)
                                {
                                    cr.Close();
                                    MyCommand3.ExecuteReader();
                                }
                                else if (cr.HasRows == false)
                                {
                                    cr.Close();
                                    MyCommand2.ExecuteReader();
                                }
                            }
                            catch
                            {
                                if (data_err.Contains(patchfile)) {
                                    string path_err = "err\\" + namefile;
                                    File.Move(patchfile, patchfile.Replace(namefile, path_err));
                                    data_err.Remove(patchfile);
                                } else data_err.Add(patchfile);
                                continue;
                            }
                            MyConn2.Close();
                        }
                        catch
                        {
                            if (data_err.Contains(patchfile)) {
                                string path_err = "err\\" + namefile;
                                File.Move(patchfile, patchfile.Replace(namefile, path_err));
                                data_err.Remove(patchfile);
                            } else data_err.Add(patchfile);
                            continue;
                        }
                    }
                    catch 
                    {
                        if (data_err.Contains(patchfile)) {
                            string path_err = "err\\" + namefile;
                            File.Move(patchfile, patchfile.Replace(namefile, path_err));
                            data_err.Remove(patchfile);
                        } else data_err.Add(patchfile);
                        continue;
                    }
                    File.Delete(patchfile);
                    try { data_err.Remove(patchfile); } catch { }
                }
                catch
                {
                    catchFlag = true;
                }
                if (catchFlag)
                {
                    while (true)
                    {
                        try
                        {
                            if (data_err.Contains(patchfile))
                            {
                                string path_err = "err\\" + namefile;
                                patchfile = patchfile.Replace("\\\\", "\\");
                                File.Move(patchfile, patchfile.Replace(namefile, path_err));
                                data_err.Remove(patchfile);
                            }
                            else
                            {
                                data_err.Add(patchfile);
                            }
                            break;
                        }
                        catch{
                            int hhhhhh = 0;
                        }
                    }

                    continue;

                }
            }
        }
        private List<string> data_err = new List<string>(); 

        private void settingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 x2 = new Form2();
            x2.ShowDialog();
        }
    }
}
