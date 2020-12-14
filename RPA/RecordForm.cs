using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RPA
{
    public partial class RecordForm : Form
    {
        private Form ReturnForm;

        private Recorder MyRecorder;
        private String CounterLabelPreString;

        public RecordForm(Form f)
        {
            InitializeComponent();
            ReturnForm = f;

            Regex regex = new Regex(@"\d*(.*)");
            // won't be changed again
            CounterLabelPreString = regex.Match(counterLabel.Text).Groups[1].ToString();

            MyRecorder = new Recorder(this);
        }

        internal void UpdateCounterLabel(int n)
        {
            counterLabel.Text = "" + n + CounterLabelPreString;
        }

        private void RecordForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MyRecorder.StopRecording();
            ReturnForm.Visible = true;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            startButton.Visible = false;
            pauseButton.Visible = true;

            if (stopButton.Enabled == false)
            {
                stopButton.Enabled = true;
                MyRecorder.InitRecorder();
            }

            MyRecorder.StartRecording();
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            pauseButton.Visible = false;
            startButton.Visible = true;

            MyRecorder.PauseRecording();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            stopButton.Enabled = false;
            startButton.Visible = true;
            pauseButton.Visible = false;

            MyRecorder.StopRecording();

            DialogResult result =
                MessageBox.Show(
                    Properties.Resources.SUCCEED_TO_RECORD_TEXT,
                    Properties.Resources.SUCCEED_TO_RECORD_TITLE,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    MyRecorder.SaveToDisk(saveFileDialog.FileName);
                }
            }
        }
    }
}
