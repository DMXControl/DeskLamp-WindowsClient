﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace DeskLamp_WinClient
{
    public partial class Form1 : Form
    {
        private readonly DeskLamp.DeskLampInstance usedInstance;
        private readonly HotKey hk;

        public Form1()
        {
            InitializeComponent();

            usedInstance = new DeskLamp.DeskLampInstance();

            this.Disposed += new EventHandler(Form1_Disposed);

            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.hk = new HotKey(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.hk.RegisterHotKey(Keys.Alt | Keys.Add);
            this.hk.RegisterHotKey(Keys.Alt | Keys.Subtract);
        }

        private void Form1_Disposed(object sender, EventArgs e)
        {
            if (usedInstance != null && !usedInstance.IsDisposed)
            {
                usedInstance.Enabled = false;
                usedInstance.Dispose();
            }

            this.hk.Dispose();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == HotKey.WM_HOTKEY)
            {
                int id = (int)m.WParam;
                Keys k = (Keys)id;
                switch (k)
                {
                    case Keys.Alt | Keys.Add:
                        int add = Math.Min(this.tbIntensity.Value + this.tbIntensity.SmallChange, this.tbIntensity.Maximum);
                        this.tbIntensity.Value = add;
                        break;

                    case Keys.Alt | Keys.Subtract:
                        int sub = Math.Max(this.tbIntensity.Value - this.tbIntensity.SmallChange, this.tbIntensity.Minimum);
                        this.tbIntensity.Value = sub;
                        break;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs args)
        {
            var dl = DeskLamp.DeskLampInstance.GetAvailableDeskLamps();
            var e = dl.Except(cbDeskLamp.Items.Cast<string>());
            if (cbDeskLamp.Items.Count != dl.Count || e.Any())
            {
                string selected = cbDeskLamp.SelectedItem as string;

                cbDeskLamp.Items.Clear();
                cbDeskLamp.Items.AddRange(dl.ToArray());
                if (selected != null && cbDeskLamp.Items.Contains(selected))
                {
                    cbDeskLamp.SelectedItem = selected;
                }
                else if(cbDeskLamp.Items.Count > 0)
                {
                    cbDeskLamp.SelectedIndex = 0;
                }
            }
        }

        private void cbDeskLamp_SelectedValueChanged(object sender, EventArgs e)
        {
            string id = cbDeskLamp.SelectedItem as string;
            if (id != null)
            {
                usedInstance.ID = id;
                usedInstance.Enabled = true;
            }
            else
            {
                usedInstance.Enabled = false;
                usedInstance.ID = String.Empty;
            }
        }

        private void tbIntensity_ValueChanged(object sender, EventArgs e)
        {
            Update(tbIntensity.Value);
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.notifyIcon1.Visible = false;
            this.hk.ReRegisterAll();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(3000);
                this.ShowInTaskbar = false;
                this.hk.ReRegisterAll();
            }
            else
            {
                notifyIcon1.Visible = false;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            notifyIcon1.Visible = false;
        }

        private void tbMenuItem_ValueChanged(object sender, EventArgs e)
        {
            Update(tbMenuItem.Value);
        }

        private void Update(double value)
        {
            this.tbIntensity.ValueChanged -= tbIntensity_ValueChanged;
            this.tbMenuItem.ValueChanged -= tbMenuItem_ValueChanged;

            usedInstance.Brightness = (byte)((value / 100) * 255);

            if (tbIntensity.Value != value) tbIntensity.Value = (int)value;
            if (tbMenuItem.Value != value) tbMenuItem.Value = (int)value;

            this.tbIntensity.ValueChanged += tbIntensity_ValueChanged;
            this.tbMenuItem.ValueChanged += tbMenuItem_ValueChanged;
        }
    }

    public class HotKey : IDisposable
    {
        private List<Keys> idList = new List<Keys>();

        private readonly Form _parentForm;

        public HotKey(Form parent)
        {
            this._parentForm = parent;
        }

        public void ReRegisterAll()
        {
            //ToList to make a copy
            this.idList.ToList().ForEach(ReRegisterHotKey);
        }

        public void ReRegisterHotKey(Keys key)
        {
            UnregisterHotKey(key);
            RegisterHotKey(key);
        }

        public void RegisterHotKey(Keys key)
        {
            if (idList.Contains(key)) return;

            idList.Add(key);

            int modifiers = 0;

            if ((key & Keys.Alt) == Keys.Alt)
                modifiers |= MOD_ALT;

            if ((key & Keys.Control) == Keys.Control)
                modifiers |= MOD_CONTROL;

            if ((key & Keys.Shift) == Keys.Shift)
                modifiers |= MOD_SHIFT;

            Keys k = key & ~Keys.Control & ~Keys.Shift & ~Keys.Alt;
            RegisterHotKey(_parentForm.Handle, (int)key, (uint)modifiers, (uint)k);
        }

        public void UnregisterHotKey(Keys key)
        {
            if (!idList.Contains(key)) return;

            try
            {
                UnregisterHotKey(_parentForm.Handle, (int)key);
                idList.Remove(key);
            }
            catch (Exception ex)
            {
                
            }
        }

        #region IDisposable Member

        public void Dispose()
        {
            //ToList to make a copy
            this.idList.ToList().ForEach(UnregisterHotKey);
        }

        #endregion

        private static int MOD_ALT = 0x1;
        private static int MOD_CONTROL = 0x2;
        private static int MOD_SHIFT = 0x4;
        private static int MOD_WIN = 0x8;
        public static int WM_HOTKEY = 0x312;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
