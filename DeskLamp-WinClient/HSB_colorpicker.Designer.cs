namespace DeskLamp_WinClient
{
    partial class HSB_colorpicker
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
            DisposeControl();
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // HSB_colorpicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "HSB_colorpicker";
            this.Size = new System.Drawing.Size(190, 169);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.HSB_colorpicker_DoubleClick);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HSB_colorpicker_MouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HSB_colorpicker_MouseDown);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(HSB_colorpicker_MouseWheel);
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}
