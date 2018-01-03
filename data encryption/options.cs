using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace data_encryption
{
    public partial class options : Form
    {
        string[] lines=new string[20];
    
        public options()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 49538.dyn.orsk.ufanet.ru
            // ivan-andreyev.zapto.org

            //int counter = 0;
            object[] controls = new object[8];
            controls[0] = label1;
            controls[1] = textBox1;
            controls[2] = label2;
            controls[3] = textBox2;
            controls[4] = label3;
            controls[5] = textBox3;
            controls[6] = label4;
            controls[7] = textBox4;


            SaveFileDialog f = new SaveFileDialog();
            f.CreatePrompt = false;
            f.DefaultExt = "ini";
            f.OverwritePrompt = false;
            int counter = 0;
            for (int i = 0; i < 8; i++)
            {
                if (i % 2 == 0)
                    lines[i] = ((Label)controls[i]).Text;
                else
                    lines[i] = ((TextBox)controls[i]).Text;
                counter = i + 1;
            }
            string[] savelines = new string[counter];
            counter =0;
            foreach(string s in lines)
                if ((s == "") || (s == null))
                { }
                else
                {
                    savelines[counter++] = s;
                }

            /*lines[1] = textBox1.Text;
            lines[2] = label2.Text;
            lines[3] = textBox2.Text;*/

            System.IO.File.WriteAllLines(@"../../data/options.ini", savelines);
        }

        private void options_Load(object sender, EventArgs e)
        {
            int counter = 0;

            System.IO.StreamReader file = new System.IO.StreamReader(@"../../data/options.ini");

            object[] controls = new object[8];
            controls[0] = label1;
            controls[1] = textBox1;
            controls[2] = label2;
            controls[3] = textBox2;
            controls[4] = label3;
            controls[5] = textBox3;
            controls[6] = label4;
            controls[7] = textBox4;

            do
            {
                //line = new string();
                lines[counter] = file.ReadLine();
                //MessageBox.Show(lines[counter]);
                //if (counter == 0) label1.Text = lines[counter];
                if (counter % 2 == 0) ((Label)controls[counter]).Text = lines[counter];
                else
                    ((TextBox)controls[counter]).Text = lines[counter];
                //if (counter == 1) textBox1.Text = lines[counter];
                //if (counter == 2) label2.Text = lines[counter];
                //if (counter == 3) textBox2.Text = lines[counter];
                counter++;
            }
            while (lines[counter - 1] != null);
            file.Close();

            //textBox2.Text;
        }
    }
}
