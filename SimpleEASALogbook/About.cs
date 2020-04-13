using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SimpleEASALogbook
{
    public partial class About : Form
    {
        public static bool isMono = false;

        public About()
        {
            InitializeComponent();
        }

        private static void IsRunningOnMono()
        {
            if (Type.GetType("Mono.Runtime") != null)
            {
                isMono = true;
            }
        }

        private void About_Load(object sender, EventArgs e)
        {
            IsRunningOnMono();
            label1.Text = "Simple EASA Logbook v0.2" + "\n\n\n" + "Framework-Version: " + Environment.Version.ToString() + "\n" + "OS-Version: " + Environment.OSVersion.ToString();
            textBox1.Text = "This program is free software: you can redistribute it and/or modify\nit under the terms of the GNU General Public License as published by\nthe Free Software Foundation, either version 3 of the License, or\n(at your option) any later version.\n\nThis program is distributed in the hope that it will be useful,\nbut WITHOUT ANY WARRANTY; without even the implied warranty of\nMERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the\nGNU General Public License for more details.\n\nYou should have received a copy of the GNU General Public License\nalong with this program.If not, see <http://www.gnu.org/licenses/>.";
            label3.Text = "This software uses and ships with pdftotext from xpdfreader (GPLv3),\nwkhtmltopdf (LGPLv3).\n\n\nthanks to all the contributors!!";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // in Mono simple URL opening sometimes does not work
            if (isMono)
            {
                if (MessageBox.Show("Opening links in unix directly sometimes fails. Do you want to copy the link to the GitHub Issues Page to clipboard?", "GitHub Issues", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                {
                    Clipboard.SetText("https://github.com/ni720/SimpleEASALogbook");
                }
            }
            else
            {
                if (MessageBox.Show("Do you want to visit the GitHub Issues Page?", "GitHub Issues", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                {
                    Process.Start("https://github.com/ni720/SimpleEASALogbook");
                }
            }
        }
    }
}