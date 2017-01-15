using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace DeskLamp_WinClient
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main(string[] paras)
        {
            List<string> parameters = new List<string>();
            if(paras != null) parameters.AddRange(paras.Select(c => c.ToLowerInvariant()));

            int? initialIntensity = null;

            int i = parameters.IndexOf("-intensity");
            if (i >= 0 && (i + 1) < parameters.Count)
            {
                string intensity = parameters[i + 1];
                int d;
                if (int.TryParse(intensity, out d))
                    initialIntensity = d;
            }

            bool minimized = parameters.Contains("-minimized");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(){InitialIntensity = initialIntensity, StartMinimized = minimized});
        }
    }
}
