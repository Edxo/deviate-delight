using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Deviate_Delight
{
    public partial class Keyfinder : Form
    {
        [DllImport("user32.dll")]
        static extern int MapVirtualKey(uint uCode, uint uMapType);

        public KeyEventArgs m_key;
        public string m_key_readable;

        public Keyfinder()
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            InitializeComponent();
            this.KeyPreview = true;
            m_key = null;
        }

        private void Keyfinder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Menu)
                return;

            string msg = "";

            if (e.Modifiers != Keys.None)
                msg = e.Modifiers.ToString();

            int nonVirtualKey = MapVirtualKey((uint)e.KeyCode, 2);

            string map_key = "";

            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter || nonVirtualKey == 0)
            {
                map_key = e.KeyCode.ToString();
            }
            else
            {
                try
                {
                    map_key = Convert.ToChar(nonVirtualKey).ToString();
                }
                catch { return; }
            }

            if (map_key.Length == 0)
            {
                map_key = e.KeyCode.ToString();
            }

            if (msg.Length > 0)
                msg += ", " + map_key;
            else
                msg += map_key;
            
            key.Text = msg;
            m_key_readable = msg;
            button1.Enabled = true;
            m_key = e;
        }

        // Cancel
        private void button2_Click(object sender, EventArgs e)
        {
            m_key = null;
            this.Close();
        }

        // Confirm
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
