using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Deviate_Delight
{
    public partial class saveForm : Form
    {
        public string save_name;

        public saveForm(bool ontop)
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            InitializeComponent();
            this.TopMost = ontop;
            save_name = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            save_name = textBox1.Text;
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button2.Enabled = textBox1.Text.Length > 0;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button2.PerformClick();
        }
    }
}
