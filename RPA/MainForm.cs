using System;
using System.Windows.Forms;

namespace RPA
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void recordButton_Click(object sender, EventArgs e)
        {
            RecordForm rf;
            try
            {
                rf = new RecordForm(this);
                rf.Show();
            }
            catch (Exception)
            {
                return;
            }
            this.Hide();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
