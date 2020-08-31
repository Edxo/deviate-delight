using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace Deviate_Delight
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int processId);
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        static extern int MapVirtualKey(uint uCode, uint uMapType);
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Keys vKey);

        const int WM_KEYDOWN = 0x0100;
        const int WM_KEYUP = 0x0101;

        int m_pid;
        KeyEventArgs m_key;
        KeyEventArgs m_toggle;

        public Form1()
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            m_pid = 0;
            m_key = null;
            m_toggle = null;
            InitializeComponent();
            this.KeyPreview = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker2.WorkerSupportsCancellation = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            process1_search_button.Enabled = false;
            IntPtr our_handle;
            IntPtr handle = GetForegroundWindow();
            our_handle = handle;

            while (our_handle == handle || handle == (IntPtr)0)
            {
                handle = GetForegroundWindow();
                Thread.Sleep(1);
            }

            int pid = 0;
            GetWindowThreadProcessId(handle, out pid);
            if (pid <= 0)
            {
                process1_search_button.Enabled = true;
                return;
            }

            m_pid = pid;
            if (m_key != null)
                button1.Enabled = true;
            process1_pid.Text = "Process PID: " + pid;
            process1_search_button.Enabled = true;

        }

        private void form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (process1_spam_button.Enabled && (process_1_toggle_key_checkbox.Checked && process1_toggle_key_button.Enabled))
                return;
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
            
            if (process1_spam_button.Enabled == false)
            {
                if (m_pid != 0)
                    button1.Enabled = true;

                m_key = e;
                process1_spam_key.Text = msg;
                process1_spam_button.Enabled = true;
            }
            if(process_1_toggle_key_checkbox.Checked && process1_toggle_key_button.Enabled == false)
            {
                process1_toggle_key.Text = msg;
                process1_toggle_key_button.Enabled = true;
                while (IsKeyComboPushedDown(e))
                {
                    Thread.Sleep(1);
                };

                m_toggle = e;
            }
        }

        private void process1_spam_button_Click(object sender, EventArgs e)
        {
            process1_spam_button.Enabled = false;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            toggle_background_thread();
        }

        private void toggle_background_thread()
        {
            if (m_pid == 0)
                return;

            if (button1.Text == "Start")
            {
                if (backgroundWorker1.IsBusy == true)
                    return;

                backgroundWorker1.RunWorkerAsync();
                button1.BeginInvoke((MethodInvoker)delegate () { button1.Text = "Stop"; });
            }
            else
            {
                if (backgroundWorker1.WorkerSupportsCancellation == false)
                    return;

                backgroundWorker1.CancelAsync();
                button1.BeginInvoke((MethodInvoker)delegate () { button1.Text = "Start"; });
            }
        }
        
        private void background_thread(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            Process proc;
            try
            {
                proc = Process.GetProcessById(m_pid);
            } catch (Exception except)
            {
                process1_pid.BeginInvoke((MethodInvoker)delegate () { process1_pid.Text = "Process PID: N/A"; });
                m_pid = 0;
                button1.BeginInvoke((MethodInvoker)delegate () { button1.Text = "Start"; button1.Enabled = false; });
                MessageBox.Show(except.Message, "Error", MessageBoxButtons.OK);
                e.Cancel = true;
                return;
            }

            if(m_key == null)
            {
                button1.BeginInvoke((MethodInvoker)delegate () { button1.Text = "Start"; button1.Enabled = false; });
                e.Cancel = true;
                return;
            }

            try
            {
                while (true)
                {
                    if(m_key.Shift)
                        PostMessage(proc.MainWindowHandle, WM_KEYDOWN, (int)Keys.ShiftKey, 0);
                    if (m_key.Control)
                        PostMessage(proc.MainWindowHandle, WM_KEYDOWN, (int)Keys.ControlKey, 0);
                    if (m_key.Alt)
                        PostMessage(proc.MainWindowHandle, WM_KEYDOWN, (int)Keys.Menu, 0);

                    PostMessage(proc.MainWindowHandle, WM_KEYDOWN, (int)m_key.KeyCode, 0);

                    Thread.Sleep(1);

                    PostMessage(proc.MainWindowHandle, WM_KEYUP, (int)m_key.KeyCode, 0);
                    
                    if (m_key.Alt)
                        PostMessage(proc.MainWindowHandle, WM_KEYUP, (int)Keys.Menu, 0);
                    if (m_key.Control)
                        PostMessage(proc.MainWindowHandle, WM_KEYUP, (int)Keys.ControlKey, 0);
                    if (m_key.Shift)
                        PostMessage(proc.MainWindowHandle, WM_KEYUP, (int)Keys.ShiftKey, 0);

                    Thread.Sleep(Decimal.ToInt32(numericUpDown1.Value));

                    if (worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
            catch (Exception except)
            {
                process1_pid.BeginInvoke((MethodInvoker)delegate () { process1_pid.Text = "Process PID: N/A"; });
                m_pid = 0;
                button1.BeginInvoke((MethodInvoker)delegate () { button1.Text = "Start"; button1.Enabled = false; });
                MessageBox.Show(except.Message, "Error", MessageBoxButtons.OK);
                e.Cancel = true;
                return;
            }
        }

        public static bool IsKeyPushedDown(Keys vKey)
        {
            return 0 != (GetAsyncKeyState(vKey) & 0x8000);
        }

        public static bool IsKeyComboPushedDown(KeyEventArgs vkey)
        {
            if ((vkey.Shift && !IsKeyPushedDown(Keys.ShiftKey)) || (!vkey.Shift && IsKeyPushedDown(Keys.ShiftKey)))
                return false;
            if (vkey.Control && !IsKeyPushedDown(Keys.ControlKey) || (!vkey.Control && IsKeyPushedDown(Keys.ControlKey)))
                return false;
            if (vkey.Alt && !IsKeyPushedDown(Keys.Menu) || (!vkey.Alt && IsKeyPushedDown(Keys.Menu)))
                return false;
            if (!IsKeyPushedDown(vkey.KeyCode))
                return false;
            return true;
        }

        private void toggle_thread(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            if (m_toggle == null)
            {
                while(m_toggle == null)
                {
                    Thread.Sleep(50);
                }
            }

            while (true)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }

                Thread.Sleep(1);

                if (!IsKeyComboPushedDown(m_toggle))
                    continue;

                toggle_background_thread();

                while (IsKeyPushedDown(m_toggle.KeyCode))
                {
                    Thread.Sleep(1);
                };
            }
        }

        private void process1_toggle_key_check_changed(object sender, EventArgs e)
        {
            if (process_1_toggle_key_checkbox.Checked == true)
            {
                if (backgroundWorker2.IsBusy == true)
                    return;
                backgroundWorker2.RunWorkerAsync();
                process1_toggle_key_button.Enabled = true;
            } else
            {
                if (backgroundWorker2.WorkerSupportsCancellation == false)
                    return;
                backgroundWorker2.CancelAsync();
                m_toggle = null;
                process1_toggle_key.Text = "N/A";
                process1_toggle_key_button.Enabled = false;
            }
        }

        private void process1_toggle_key_button_Click(object sender, EventArgs e)
        {
            process1_toggle_key_button.Enabled = false;
        }


    }
}
