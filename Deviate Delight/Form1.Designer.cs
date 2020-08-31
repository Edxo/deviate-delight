namespace Deviate_Delight
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.process1_search_button = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.process1_pid = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.process1_spam_key = new System.Windows.Forms.Label();
            this.process1_spam_button = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.process_1_toggle_key_checkbox = new System.Windows.Forms.CheckBox();
            this.process1_toggle_key = new System.Windows.Forms.Label();
            this.process1_toggle_key_button = new System.Windows.Forms.Button();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // process1_search_button
            // 
            this.process1_search_button.Location = new System.Drawing.Point(6, 19);
            this.process1_search_button.Name = "process1_search_button";
            this.process1_search_button.Size = new System.Drawing.Size(96, 32);
            this.process1_search_button.TabIndex = 0;
            this.process1_search_button.Text = "Search";
            this.process1_search_button.UseVisualStyleBackColor = true;
            this.process1_search_button.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.process1_pid);
            this.groupBox2.Controls.Add(this.process1_search_button);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(220, 57);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Process Selection";
            // 
            // process1_pid
            // 
            this.process1_pid.AutoSize = true;
            this.process1_pid.Location = new System.Drawing.Point(108, 29);
            this.process1_pid.Name = "process1_pid";
            this.process1_pid.Size = new System.Drawing.Size(92, 13);
            this.process1_pid.TabIndex = 3;
            this.process1_pid.Text = "Process PID: N/A";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.process1_spam_key);
            this.groupBox1.Controls.Add(this.process1_spam_button);
            this.groupBox1.Location = new System.Drawing.Point(12, 161);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(220, 57);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Key";
            // 
            // process1_spam_key
            // 
            this.process1_spam_key.AutoSize = true;
            this.process1_spam_key.Location = new System.Drawing.Point(108, 29);
            this.process1_spam_key.Name = "process1_spam_key";
            this.process1_spam_key.Size = new System.Drawing.Size(27, 13);
            this.process1_spam_key.TabIndex = 3;
            this.process1_spam_key.Text = "N/A";
            // 
            // process1_spam_button
            // 
            this.process1_spam_button.Location = new System.Drawing.Point(6, 19);
            this.process1_spam_button.Name = "process1_spam_button";
            this.process1_spam_button.Size = new System.Drawing.Size(96, 32);
            this.process1_spam_button.TabIndex = 0;
            this.process1_spam_button.Text = "Scan";
            this.process1_spam_button.UseVisualStyleBackColor = true;
            this.process1_spam_button.Click += new System.EventHandler(this.process1_spam_button_Click);
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(12, 250);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(220, 36);
            this.button1.TabIndex = 5;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.background_thread);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(91, 224);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            -1486618625,
            232830643,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(137, 20);
            this.numericUpDown1.TabIndex = 0;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 226);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Key Delay(ms):";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.process_1_toggle_key_checkbox);
            this.groupBox3.Controls.Add(this.process1_toggle_key);
            this.groupBox3.Controls.Add(this.process1_toggle_key_button);
            this.groupBox3.Location = new System.Drawing.Point(12, 75);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(220, 80);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Toggle Key";
            // 
            // process_1_toggle_key_checkbox
            // 
            this.process_1_toggle_key_checkbox.AutoSize = true;
            this.process_1_toggle_key_checkbox.Location = new System.Drawing.Point(6, 19);
            this.process_1_toggle_key_checkbox.Name = "process_1_toggle_key_checkbox";
            this.process_1_toggle_key_checkbox.Size = new System.Drawing.Size(116, 17);
            this.process_1_toggle_key_checkbox.TabIndex = 8;
            this.process_1_toggle_key_checkbox.Text = "Enable Toggle Key";
            this.process_1_toggle_key_checkbox.UseVisualStyleBackColor = true;
            this.process_1_toggle_key_checkbox.CheckedChanged += new System.EventHandler(this.process1_toggle_key_check_changed);
            // 
            // process1_toggle_key
            // 
            this.process1_toggle_key.AutoSize = true;
            this.process1_toggle_key.Location = new System.Drawing.Point(108, 52);
            this.process1_toggle_key.Name = "process1_toggle_key";
            this.process1_toggle_key.Size = new System.Drawing.Size(27, 13);
            this.process1_toggle_key.TabIndex = 3;
            this.process1_toggle_key.Text = "N/A";
            // 
            // process1_toggle_key_button
            // 
            this.process1_toggle_key_button.Enabled = false;
            this.process1_toggle_key_button.Location = new System.Drawing.Point(6, 42);
            this.process1_toggle_key_button.Name = "process1_toggle_key_button";
            this.process1_toggle_key_button.Size = new System.Drawing.Size(96, 32);
            this.process1_toggle_key_button.TabIndex = 0;
            this.process1_toggle_key_button.Text = "Scan";
            this.process1_toggle_key_button.UseVisualStyleBackColor = true;
            this.process1_toggle_key_button.Click += new System.EventHandler(this.process1_toggle_key_button_Click);
            // 
            // backgroundWorker2
            // 
            this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.toggle_thread);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(246, 293);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "Deviate Delight";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.form1_KeyDown);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button process1_search_button;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label process1_pid;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label process1_spam_key;
        private System.Windows.Forms.Button process1_spam_button;
        private System.Windows.Forms.Button button1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox process_1_toggle_key_checkbox;
        private System.Windows.Forms.Label process1_toggle_key;
        private System.Windows.Forms.Button process1_toggle_key_button;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
    }
}

