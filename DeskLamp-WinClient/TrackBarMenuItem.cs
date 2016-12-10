using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace DeskLamp_WinClient
{
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip |
                                   ToolStripItemDesignerAvailability.ContextMenuStrip)]
    public class TrackBarMenuItem : ToolStripControlHost
    {
        public event EventHandler ValueChanged;

        private readonly TrackBar trackBar;

        public TrackBarMenuItem()
            : base(new TrackBar())
        {
            this.trackBar = this.Control as TrackBar;
            this.trackBar.ValueChanged += (s, args) =>
                {
                    if (ValueChanged != null)
                        ValueChanged(this, args);
                };
        }

        public int Maximum
        {
            get { return this.trackBar.Maximum; }
            set { this.trackBar.Maximum = value; }
        }

        public int Minimum
        {
            get { return this.trackBar.Minimum; }
            set { this.trackBar.Minimum = value; }
        }

        public int Value
        {
            get { return this.trackBar.Value; }
            set { this.trackBar.Value = value; }
        }

        public int SmallChange
        {
            get { return this.trackBar.SmallChange; }
            set { this.trackBar.SmallChange = value; }
        }

        public int LargeChange
        {
            get { return this.trackBar.LargeChange; }
            set { this.trackBar.LargeChange = value; }
        }

        public int TickFrequency
        {
            get { return this.trackBar.TickFrequency; }
            set { this.trackBar.TickFrequency = value; }
        }

        public Orientation Orientation
        {
            get { return this.trackBar.Orientation; }
            set { this.trackBar.Orientation = value; }
        }

        public TickStyle TickStyle
        {
            get { return this.trackBar.TickStyle; }
            set { this.trackBar.TickStyle = value; }
        }
    }
}
