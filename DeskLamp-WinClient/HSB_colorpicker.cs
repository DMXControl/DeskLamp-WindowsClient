using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;  // ImageFormat
using System.Threading;
using System.Diagnostics;

namespace DeskLamp_WinClient
{
    public partial class HSB_colorpicker : UserControl
    {
        private const int BOARDER_WIDTH = 0;

        private static readonly Dictionary<int, Bitmap> enabledBackgrounds = new Dictionary<int, Bitmap>();
        private static readonly Dictionary<int, Bitmap> disabledBackgrounds = new Dictionary<int, Bitmap>();

        private Point mCoords;
        private Color mCurrentColor = Color.White;
        private Point? mCircleCoords;
        private int mCircleRadius = 5;

        private volatile bool updating = false;

        private readonly System.Windows.Forms.Timer _cacheTimer = new System.Windows.Forms.Timer();
        private Color? _cachedColor;
        private readonly object _cacheLock = new object();

        public event EventHandler SelectedColorChanged;

        public HSB_colorpicker()
        {
            InitializeComponent();
            this.BackgroundImageLayout = ImageLayout.Stretch;
            //this.BackColor = Color.Gray;
            this.DoubleBuffered = true;
            this.SetStyle(
                  ControlStyles.AllPaintingInWmPaint |
                  ControlStyles.UserPaint |
                  ControlStyles.DoubleBuffer, true);

            _cacheTimer.Interval = 50; // set caching time to 50ms for this control
            _cacheTimer.Tick += cacheTimer_Tick;

            mCoords = CreateCoordsFromColor(Color.White, this.Radius, this.Center);

            redraw();
            drawCircle();
        }

        private void DisposeControl()
        {
            this._cacheTimer.Dispose();
        }

        private void cacheTimer_Tick(object sender, EventArgs e)
        {
            lock (_cacheLock)
            {
                if (IsDisposed)
                    return;

                if (_cachedColor != null) // value cached, set value, time still runs
                {
                    SetCurrentColor((Color)_cachedColor, false, false);
                    _cachedColor = null;
                }
                else // no value cached, disable timer
                {
                    _cacheTimer.Enabled = false;
                }
            }
        }

        public int CircleDiameter
        {
            get { return this.mCircleRadius; }
            set
            {
                eraseCircle();
                this.mCircleRadius = value;
                drawCircle();
            }
        }

        private Point Center
        {
            get { return new Point(this.Width / 2, this.Height / 2); }
        }

        private int Radius
        {
            get { return Math.Min((this.Height - BOARDER_WIDTH) / 2, (this.Width - BOARDER_WIDTH) / 2); }
        }

        public Color CurrentColor
        {
            get { return this.mCurrentColor; }
            set { SetCurrentColor(value, true, true); }
        }

        //delete cache when setting internal
        private void SetCurrentColor(Color value, bool cache, bool fireEvent)
        {
            lock (_cacheLock)
            {
                if (cache && _cacheTimer.Enabled)
                {
                    _cachedColor = value;
                    return;
                }
                if (!cache)
                {
                    // reset cache, internal set value overrides cached one
                    _cachedColor = null;
                }
            }

            // TEST
            if (value.ColorsEqual(mCurrentColor))
                return;

            mCoords = CreateCoordsFromColor(value, this.Radius, this.Center);
            eraseCircle();
            /*
             * If we use this property to set a new color, the circle
             * will be removed at the old position and drawn at the new one,
             * but the paint function uses CurrentColor to determine the color
             * of the brush, that is drawn.
             * This is a fix for FS#496
             */
            drawCircle(value);
            ActualSetColor(value, fireEvent);

            if (cache)
                _cacheTimer.Enabled = true;
        }

        private void ActualSetColor(Color value, bool fireEvent)
        {
            if (!value.ColorsEqual(mCurrentColor))
            {
                this.mCurrentColor = value;
                Application.DoEvents();
                if (fireEvent)
                    OnSelectedColorChanged();
            }
        }

        /// <summary>
        /// creates a bitmap with color-circle and sets it as background image
        /// </summary>
        private void redraw()
        {
            if (this.Width - BOARDER_WIDTH < 2 || this.Height - BOARDER_WIDTH < 2)
                return;
            if (this.Enabled)
            {
                lock (enabledBackgrounds)
                {
                    if (!enabledBackgrounds.ContainsKey(this.BackColor.ToArgb()))
                        enabledBackgrounds[this.BackColor.ToArgb()] = ImageTools.DrawHSVCircle(this.BackColor);
                    this.BackgroundImage = enabledBackgrounds[this.BackColor.ToArgb()];
                }
            }
            else
            {
                lock (disabledBackgrounds)
                {
                    if (!disabledBackgrounds.ContainsKey(this.BackColor.ToArgb()))
                    {
                        lock (enabledBackgrounds)
                        {
                            if (!enabledBackgrounds.ContainsKey(this.BackColor.ToArgb()))
                                enabledBackgrounds[this.BackColor.ToArgb()] = ImageTools.DrawHSVCircle(this.BackColor);
                            disabledBackgrounds[this.BackColor.ToArgb()] = ImageTools.ConvertToGreyScale(enabledBackgrounds[this.BackColor.ToArgb()]);
                        }
                    }
                    this.BackgroundImage = disabledBackgrounds[this.BackColor.ToArgb()];
                }
            }
        }

        private static void GetAngleAndDistFromCoords(Point coords, int radius, Point center, out double dist, out double angle)
        {
            dist = Math.Sqrt(Math.Pow(coords.X - center.X, 2) + Math.Pow(coords.Y - center.Y, 2));
            if (coords.X - center.X > 0)
                angle = Math.Atan2(coords.X - center.X, coords.Y - center.Y);
            else
                angle = Math.Atan2(-(coords.X - center.X), -(coords.Y - center.Y)) + Math.PI;
        }

        private static Color CreateColorFromCoords(Point coords, int radius, Point center)
        {
            double dist, angle;
            GetAngleAndDistFromCoords(coords, radius, center, out dist, out angle);
            return CreateColorFromAngleAndDist(angle, dist, radius);
        }

        private static Color CreateColorFromAngleAndDist(double angle, double dist, int radius)
        {
            Color newColor = ColorTools.FromHSV(angle/(2*Math.PI), dist/radius, 1, _base:1)
                             ?? Color.Black;
            return newColor;
        }

        private static Point CreateCoordsFromColor(Color color, int radius, Point center)
        {
            double angle;

            Point myReturn = new Point(0, 0);
            double alpha = 90;
            double beta = 0;
            double gamma = 0;
            double a = 0;
            double b = 0;

            var hsv = HSVConverter.ColortoHSV(color);

            if (hsv.H.Between(0, 360))
                angle = (hsv.H/360)*Math.PI*2;
            else
                angle = 0;

            double Grad = angle * 57.40923650233096;

            // L = Entfernung zum Mittelpunkt
            // angle = Gradzahl auf dem Kreis

            // Modulo 90, um den Quadranten zu eliminieren
            beta = Grad % 90;
            // gamma ist der Restwinkel
            gamma = 180 - beta - alpha;

            // Winkel ins Bogenmaß umrechnen
            beta = beta * Math.PI / 180;
            gamma = gamma * Math.PI / 180;

            double width = hsv.S*radius;

            // Seitenlängen berechnen:
            // sin(x) = Gegenkathete / Hypothenuse
            // Gegenkathete = sin(x) * Hypothenuse
            b = Math.Round(Math.Sin(beta) * width, 0);
            a = Math.Round(Math.Sin(gamma) * width, 0);

            // Addition bzw. Subtraktion je nach Quadrant
            if (Grad >= 0 && Grad < 90)
            {
                myReturn.X = center.X + (int)b;
                myReturn.Y = center.Y + (int)a;
            }
            else if (Grad >= 90 && Grad < 180)
            {
                myReturn.X = center.X + (int)a;
                myReturn.Y = center.Y - (int)b;
            }
            else if (Grad >= 180 && Grad < 270)
            {
                myReturn.X = center.X - (int)b;
                myReturn.Y = center.Y - (int)a;
            }
            else if (Grad >= 270 && Grad < 360)
            {
                myReturn.X = center.X - (int)a;
                myReturn.Y = center.Y + (int)b;
            }
            return myReturn;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (e.ClipRectangle == this.ClientRectangle)
                drawCircle(e.Graphics);
        }

        private void drawCircle()
        {
            drawCircle(CurrentColor);
        }

        private void drawCircle(Color color)
        {
            using (Graphics gfx = this.CreateGraphics())
                drawCircle(gfx, color);
        }

        private void drawCircle(Graphics gfx)
        {
            drawCircle(gfx, CurrentColor);
        }

        private void drawCircle(Graphics gfx, Color color)
        {
            if (!this.Enabled)
                return;

            using (Pen pen = new Pen(Color.Black, 1))
            using (Brush brush = new SolidBrush(color))
            {
                gfx.FillEllipse(brush, mCoords.X - CircleDiameter, mCoords.Y - CircleDiameter, 2 * CircleDiameter, 2 * CircleDiameter);
                gfx.DrawEllipse(pen, mCoords.X - CircleDiameter, mCoords.Y - CircleDiameter, 2 * CircleDiameter, 2 * CircleDiameter);
                this.mCircleCoords = mCoords;
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            this.redraw();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            //We do this to get new coordinates
            mCoords = CreateCoordsFromColor(this.CurrentColor, this.Radius, this.Center);
            eraseCircle();
            drawCircle();
        }

        void HSB_colorpicker_MouseWheel(object sender, MouseEventArgs e)
        {
            int d = e.Delta / 120;
            HSV hsv = HSVConverter.ColortoHSV(this.CurrentColor);
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
            }
            else
            {
                hsv.H = ColorTools.TrimHue(hsv.H + d * 7.2);
            }

            SetCurrentColor(HSVConverter.HSVtoRGB(hsv).ToColor(), false, true);
        }

        private void HSB_colorpicker_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.Enabled)
                return;

            // we need this flag to prevent a stack-overflow because we 
            // would cause our own onMouseMove-Eents to be handled in Application.DoEvents()
            // what would cause a recursion if the mouse is moved continuous.
            if (updating)
                return;

            updating = true;

            if (e.Button == MouseButtons.Left)
            {
                int radius = this.Radius;
                Point center = this.Center;
                PointF mouseVector =  new PointF(e.X - center.X, e.Y - center.Y);
                float length = (float)Math.Sqrt(Math.Pow(mouseVector.X , 2) + Math.Pow(mouseVector.Y, 2));
                
                if (length <= radius){
                    mCoords = e.Location;
                }
                else{
                    PointF direction = new PointF(mouseVector.X / length, mouseVector.Y / length);
                    mCoords = new Point(center.X + (int)(direction.X * radius), center.Y + (int)(direction.Y * radius));
                }
                GetColorFromPosition();
            }
            updating = false;
        }

        private void HSB_colorpicker_MouseDown(object sender, MouseEventArgs e)
        {
            HSB_colorpicker_MouseMove(sender, e);
        }

        private void HSB_colorpicker_DoubleClick(object sender, MouseEventArgs e)
        {
            if (!this.Enabled)
                return;

            updating = true;
            if (e.Button == MouseButtons.Left)
            {
                mCoords = this.Center;
                GetColorFromPosition();
            }
            updating = false;
        }

        private void GetColorFromPosition()
        {
            eraseCircle();
            Color c = CreateColorFromCoords(mCoords, this.Radius, this.Center);
            ActualSetColor(c, true);
            drawCircle();
        }

        /// <summary>
        /// repaint portion of form with the circle on it
        /// </summary>
        private void eraseCircle()
        {
            if (this.mCircleCoords != null)
            {
                Point p = (Point)mCircleCoords;
                int radius = CircleDiameter + 1;
                this.Invalidate(new Rectangle(p.X - radius, p.Y - radius, radius * 2, radius * 2));
                this.Update();
            }
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            redraw();
        }

        protected virtual void OnSelectedColorChanged()
        {
            if (this.SelectedColorChanged != null)
                this.SelectedColorChanged(this, EventArgs.Empty);
        }

    }
}
