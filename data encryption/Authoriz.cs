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
    public partial class Authoriz : Form
    {
        public Form2 p;
        public Authoriz(Form2 parent)
        {
            p = parent;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            p.login = textBox1.Text;
            p.pass = textBox2.Text;
            Close();
        }
    }
}
