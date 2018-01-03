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
    public partial class Dialog : Form
    {
        Form2 p;
        public Dialog(Form2 parent)
        {
            p = parent;
            InitializeComponent();
            p.messageFromMyClass += delegate(object sender, MyEventArgs e)
            {
                textBox1.Invoke((Action)delegate
                {
                    textBox1.AppendText(e.Message);
                    textBox1.AppendText(Environment.NewLine);
                });
            };
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogEventArgs args = new DialogEventArgs();
            args.mess = textBox2.Text;
            textBox1.AppendText("From me: " + textBox2.Text);
            textBox2.Text = "";
            p.button2_Click(this, args);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
        }
    }

    public class DialogEventArgs : EventArgs
    {
        public string mess { get; set; }
    }
}
