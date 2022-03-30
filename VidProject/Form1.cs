using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using GetID;

namespace VidProject
{
    public partial class Form1 : Form
    {
        readonly GetID.GetID egyes;
        static string strRegexID = "^[A-Z][0-9]{4}$";

        public Form1()
        {
            InitializeComponent();
            egyes = new GetID.GetID();
            egyes.ValueChanged += getID_ValueChange;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            egyes.Go();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            egyes.Stop();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tbxMain.Text += egyes.Value+Environment.NewLine;
        }

        private void getID_ValueChange(object sender, EventArgs e)
        {
            string id = egyes.Value;
            id += idRegexTester(id) ? " (MEGFELELŐ)" : " (NEM MEGFELELŐ)";
            tbxMain.Invoke((Action)delegate
            {
                tbxMain.Text += id + Environment.NewLine;
            });
        }

        private bool idRegexTester(string id)
        {
            return new Regex(strRegexID).Match(id).Success;
        }
    }
}
