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
        static extern short GetAsyncKeyState(Keys vKey);
        [DllImport("user32.dll", EntryPoint = "VkKeyScanExW")]
        static extern short VkKeyScanExW(Char ch, IntPtr dwhkl);

        const int WM_KEYDOWN = 0x0100;
        const int WM_KEYUP = 0x0101;

        int m_pid;
        KeyEventArgs m_toggle;

        public Form1()
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            m_pid = 0;
            m_toggle = null;
            InitializeComponent();
            this.KeyPreview = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker2.WorkerSupportsCancellation = true;
            
            DataGridViewComboBoxColumn combo = new DataGridViewComboBoxColumn();
            combo.HeaderText = "Type";
            combo.Items.AddRange("Delay", "Spam", "Toggle", "Write");

            dataGridView1.Columns.Add(combo);
            dataGridView1.Columns.Add("Value", "Value");
            dataGridView1.Columns.Add("Duration", "Duration");

            dataGridView1.RowHeadersWidth = 15;

            dataGridView1.CellValueChanged += new DataGridViewCellEventHandler(dataGridView1_CellValueChanged);
            dataGridView1.CurrentCellDirtyStateChanged += new EventHandler(dataGridView1_CurrentCellDirtyStateChanged);

            
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
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
            button1.Enabled = true;
            process1_pid.Text = "Process PID: " + pid;
            process1_search_button.Enabled = true;

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            toggle_background_thread();
        }

        private void toggle_background_thread()
        {
            if (m_pid == 0)
                return;

            dataGridView1.ClearSelection();

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

            try
            {
                while (true)
                {
                    if (worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        return;
                    }

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (dataGridView1.Rows.Count <= 1)
                            Thread.Sleep(1);

                        if (row == null)
                            continue;
                        if (row.Cells[0] == null || row.Cells[2] == null)
                            continue;
                        if (row.Cells[0].Value == null || row.Cells[2].Value == null)
                            continue;

                        row.Selected = true;
                        string type = row.Cells[0].Value.ToString();
                        decimal duration;
                        bool ret = Decimal.TryParse(row.Cells[2].Value.ToString(), out duration);
                        if (!ret)
                            continue;
                        
                        if(type == "Spam")
                        {
                            if (row.Cells[1] == null || row.Cells[1].Value == null)
                            {
                                row.Selected = false;
                                continue;
                            }

                            Keybind_t obj = (Keybind_t)row.Cells[1].Value;

                            duration = duration / 1000;
                            DateTime now = DateTime.UtcNow;
                            DateTime end = now.AddSeconds(decimal.ToDouble(duration));

                            while (now < end)
                            {
                                ToggleKey(proc, obj.m_key);
                                if (worker.CancellationPending == true)
                                {
                                    e.Cancel = true;
                                    return;
                                }
                                now = DateTime.UtcNow;
                            }
                        } else if(type == "Toggle")
                        {
                            if (row.Cells[1] == null || row.Cells[1].Value == null)
                            {
                                row.Selected = false;
                                continue;
                            }

                            Keybind_t obj = (Keybind_t)row.Cells[1].Value;
                            ToggleKey(proc, obj.m_key);
                        }
                        else if(type == "Delay")
                        {
                            duration = duration / 1000;
                            DateTime now = DateTime.UtcNow;
                            DateTime end = now.AddSeconds(decimal.ToDouble(duration));

                            while(now < end)
                            {
                                Thread.Sleep(1);
                                if (worker.CancellationPending == true)
                                {
                                    e.Cancel = true;
                                    return;
                                }
                                now = DateTime.UtcNow;
                            }
                        } else if(type == "Write")
                        {
                            if(row.Cells[1] == null || row.Cells[1].Value == null)
                            {
                                row.Selected = false;
                                continue;
                            }

                            string value = row.Cells[1].Value.ToString();
                            int per_char = decimal.ToInt32(duration / value.Length);

                            ToggleKey(proc, new KeyEventArgs(Keys.Enter));
                            foreach (char key in value)
                            {
                                short vkKeyCode = VkKeyScanExW(key, InputLanguage.CurrentInputLanguage.Handle);
                                
                                if ((vkKeyCode & 0xFF00) != 0)
                                    PostMessage(proc.MainWindowHandle, WM_KEYDOWN, (int)Keys.ShiftKey, 0);

                                PostMessage(proc.MainWindowHandle, WM_KEYDOWN, (int)vkKeyCode, 0);

                                Thread.Sleep(1);

                                PostMessage(proc.MainWindowHandle, WM_KEYUP, (int)vkKeyCode, 0);

                                if ((vkKeyCode & 0xFF00) != 0)
                                    PostMessage(proc.MainWindowHandle, WM_KEYUP, (int)Keys.ShiftKey, 0);
                                
                                if (per_char > 0)
                                    Thread.Sleep(per_char);

                                if (worker.CancellationPending == true)
                                {
                                    e.Cancel = true;
                                    return;
                                }
                            }

                            if (per_char <= 0)
                                Thread.Sleep(per_char);
                            ToggleKey(proc, new KeyEventArgs(Keys.Enter));
                        }

                        row.Selected = false;
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

        public static void ToggleKey(Process proc, KeyEventArgs vkey)
        {
            if(vkey.Shift)
                PostMessage(proc.MainWindowHandle, WM_KEYDOWN, (int)Keys.ShiftKey, 0);
            if (vkey.Control)
                PostMessage(proc.MainWindowHandle, WM_KEYDOWN, (int)Keys.ControlKey, 0);
            if (vkey.Alt)
                PostMessage(proc.MainWindowHandle, WM_KEYDOWN, (int)Keys.Menu, 0);

            PostMessage(proc.MainWindowHandle, WM_KEYDOWN, (int)vkey.KeyCode, 0);

            Thread.Sleep(1);

            PostMessage(proc.MainWindowHandle, WM_KEYUP, (int)vkey.KeyCode, 0);

            if (vkey.Alt)
                PostMessage(proc.MainWindowHandle, WM_KEYUP, (int)Keys.Menu, 0);
            if (vkey.Control)
                PostMessage(proc.MainWindowHandle, WM_KEYUP, (int)Keys.ControlKey, 0);
            if (vkey.Shift)
                PostMessage(proc.MainWindowHandle, WM_KEYUP, (int)Keys.ShiftKey, 0);
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
                KeyEventArgs toggle = m_toggle;
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }

                Thread.Sleep(1);

                if (toggle == null)
                    continue;
                if (!IsKeyComboPushedDown(toggle))
                    continue;

                toggle_background_thread();

                while (IsKeyPushedDown(toggle.KeyCode))
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
            Keyfinder key_search = new Keyfinder();
            key_search.ShowDialog();

            KeyEventArgs key = key_search.m_key;
            string key_readable = key_search.m_key_readable;
            if (key == null)
                return;

            m_toggle = key;
            process1_toggle_key.Text = key_readable;
        }

        void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewComboBoxCell type = (DataGridViewComboBoxCell)dataGridView1.Rows[e.RowIndex].Cells[0];
            if (type.Value == null)
                return;

            DataGridViewTextBoxCell value = (DataGridViewTextBoxCell)dataGridView1.Rows[e.RowIndex].Cells[1];
            DataGridViewTextBoxCell duration = (DataGridViewTextBoxCell)dataGridView1.Rows[e.RowIndex].Cells[2];


            if ("Write" == (string)type.Value)
            {
                value.Style.BackColor = Color.White;
                value.ReadOnly = false;
                return;
            } else {
                if ("Delay" == (string)type.Value)
                {
                    value.Style.BackColor = Color.LightGray;
                    value.Value = "";
                }
                else
                    value.Style.BackColor = Color.White;
                value.ReadOnly = true;
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
            if (dataGridView1.CurrentCell.ColumnIndex == 2)
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
                }
            }
        }

        private void Column1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 1)
                return;

            string type = (string)dataGridView1.Rows[e.RowIndex].Cells[0].Value;
            if (type != "Spam" && type != "Toggle")
                return;

            Keyfinder key_search = new Keyfinder();
            key_search.ShowDialog();

            KeyEventArgs key = key_search.m_key;
            string key_readable = key_search.m_key_readable;
            if (key == null)
                return;

            Keybind_t obj = new Keybind_t();
            obj.m_key = key;
            obj.readable = key_readable;

            DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)dataGridView1.Rows[e.RowIndex].Cells[1];
            cell.Value = obj;
        }

        private void alwaysOnTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TopMost = !this.TopMost;
            alwaysOnTopToolStripMenuItem.Checked = this.TopMost;
        }
    }

    class Keybind_t
    {
        public string readable { get; set; }
        public KeyEventArgs m_key { get; set; }

        public override string ToString()
        {
            return readable;
        }
    }
}
