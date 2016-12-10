namespace DeskLamp_WinClient
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.cbDeskLamp = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbIntensity = new System.Windows.Forms.TrackBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tbMenuItem = new DeskLamp_WinClient.TrackBarMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.tbIntensity)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbDeskLamp
            // 
            this.cbDeskLamp.FormattingEnabled = true;
            this.cbDeskLamp.Location = new System.Drawing.Point(76, 12);
            this.cbDeskLamp.Name = "cbDeskLamp";
            this.cbDeskLamp.Size = new System.Drawing.Size(196, 21);
            this.cbDeskLamp.TabIndex = 0;
            this.cbDeskLamp.SelectedValueChanged += new System.EventHandler(this.cbDeskLamp_SelectedValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "DeskLamp:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Intensity:";
            // 
            // tbIntensity
            // 
            this.tbIntensity.LargeChange = 20;
            this.tbIntensity.Location = new System.Drawing.Point(76, 39);
            this.tbIntensity.Maximum = 100;
            this.tbIntensity.Name = "tbIntensity";
            this.tbIntensity.Size = new System.Drawing.Size(196, 45);
            this.tbIntensity.SmallChange = 5;
            this.tbIntensity.TabIndex = 3;
            this.tbIntensity.TickFrequency = 10;
            this.tbIntensity.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.tbIntensity.ValueChanged += new System.EventHandler(this.tbIntensity_ValueChanged);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipText = "DeskLamp is minimized\r\nUse ALT+ and ALT- to change Intensity";
            this.notifyIcon1.BalloonTipTitle = "DeskLamp";
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "DeskLamp";
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(106, 111);
            // 
            // tbMenuItem
            // 
            this.tbMenuItem.LargeChange = 20;
            this.tbMenuItem.Maximum = 100;
            this.tbMenuItem.Minimum = 0;
            this.tbMenuItem.Name = "tbMenuItem";
            this.tbMenuItem.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbMenuItem.Size = new System.Drawing.Size(45, 104);
            this.tbMenuItem.SmallChange = 5;
            this.tbMenuItem.Text = "Intensity";
            this.tbMenuItem.TickFrequency = 10;
            this.tbMenuItem.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.tbMenuItem.Value = 0;
            this.tbMenuItem.ValueChanged += new System.EventHandler(this.tbMenuItem_ValueChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 98);
            this.Controls.Add(this.tbIntensity);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbDeskLamp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "DeskLamp";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.tbIntensity)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbDeskLamp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar tbIntensity;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private TrackBarMenuItem tbMenuItem;
    }
}

