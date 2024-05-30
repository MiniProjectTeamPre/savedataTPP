using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace savedataTPP
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            try
            {
                string[] config = File.ReadAllText(Application.StartupPath + "\\config.txt").Split(',');
                if (config.Length < 4) return;
                textBox1.Text = config[0];
                textBox2.Text = config[1];
                textBox3.Text = config[2];
                textBox4.Text = config[3];
            }
            catch
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("ต้องการบันทึกหรือไม่", "Save Config", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result.ToString() == "Yes")
            {
                string config = textBox1.Text.ToLower() + "," + textBox2.Text.ToLower() + "," + textBox3.Text.ToLower() + "," + textBox4.Text.ToLower() + ",";
                File.WriteAllText(Application.StartupPath + "\\config.txt", config);
                this.Close();
            }
            else
            {
                return;
            }
           
        }
    }
}
