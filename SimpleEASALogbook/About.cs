using System;
using System.Windows.Forms;

namespace SimpleEASALogbook
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void About_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}