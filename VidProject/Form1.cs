using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using GetID;

namespace VidProject
{
    public partial class Form1 : Form
    {
        // static fields
        private const string strRegexID = "^[A-Z][0-9]{4}$";
        private const string csvFileName = "ment.csv";
        private static readonly Color errorColor = Color.Red;
        private static readonly Color normalColor = Color.Black;
        private static readonly Color warningColor = Color.Blue;


        readonly GetID.GetID egyes;
        private IStoreData csvFileWriter;
        private bool isAllSaved;

        public Form1()
        {
            InitializeComponent();
            egyes = new GetID.GetID();
            egyes.Stop();
            egyes.ValueChanged += getID_ValueChange;
            egyes.ErrorChanged += getID_ErrorChanged;
            csvFileWriter = new StoreDataToCsv(csvFileName);
            lblMessageSetTextWithColoring("", normalColor);
            isAllSaved = true;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            if (!egyes.Running)
            {
                egyes.Go();
                lblMessageSetTextWithColoring("Started", normalColor);
            } else
            {
                lblMessageSetTextWithColoring("It is already running!", warningColor);
            }
            
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (egyes.Running)
            {
                egyes.Stop();
                lblMessageSetTextWithColoring("Stopped", normalColor);
            } else
            {
                lblMessageSetTextWithColoring("It is not running!", warningColor);
            }
                
        }

         private void getID_ErrorChanged(object sender, EventArgs e)
        {
            lblMessageSetTextWithColoring(egyes.ErrorMessage, errorColor);
        }

        private void getID_ValueChange(object sender, EventArgs e)
        {
            string[] id = { egyes.Value, idRegexTester(egyes.Value) ? "OK" : "NOK" };
            tbxMain.Invoke((Action)delegate
            {
                if (!tbxMain.Text.Equals(""))
                {
                    tbxMain.AppendText(Environment.NewLine);
                }
                tbxMain.AppendText(id[0] + " - " + id[1]);
                csvFileWriter.WriteFile(id, true);
                isAllSaved = false;
            });
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (egyes.Running)
            {
                lblMessageSetTextWithColoring("Please stop it before clearing", warningColor);
            } else if (isAllSaved)
            {
                lblMessageSetTextWithColoring("Cleared", normalColor);
                tbxMain.Clear();
            } else 
            {
                if (MessageBox.Show("Do you want to save changes before clear?", "My Application",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if(SaveToFile())
                    {
                        isAllSaved = true;
                    }
                } else
                {
                    lblMessageSetTextWithColoring("Cleared without saving", warningColor);
                    tbxMain.Clear();
                    isAllSaved = true;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (egyes.Running)
            {
                lblMessageSetTextWithColoring("Please stop it before saving!", warningColor);
            }
            else if (tbxMain.Text is null || tbxMain.Text.Equals(""))
            {
                lblMessageSetTextWithColoring("Nothing to save!", errorColor);
            } 
            else 
            {
                if (SaveToFile())
                {
                    lblMessageSetTextWithColoring("The writing was successful", normalColor);
                    isAllSaved=true;
                }
            }
            
        }

        private bool idRegexTester(string id)
        {
            return new Regex(strRegexID).Match(id).Success;
        }

        private void lblMessageSetTextWithColoring(string text, Color color)
        {
            lblMessage.Text = text;
            lblMessage.ForeColor = color;
        }

        private bool SaveToFile()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text files (*.txt) | *.txt";
            sfd.InitialDirectory = Directory.GetCurrentDirectory();
            sfd.Title = "Save textbox data into txt file";
           
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    IStoreData txtFileWriter = new StoreDataToTxt(sfd.FileName);
                    txtFileWriter.WriteFile(tbxMain.Lines, false);                           
                }
                catch (Exception ex)
                {
                    lblMessageSetTextWithColoring("Error during writing: "+ex.Message, errorColor);
                    return false;
                }
                return true;
            }
            return false;

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isAllSaved)
            {
                if (MessageBox.Show("Do you want to save changes before exit?", "My Application",
                   MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (SaveToFile())
                    {
                        isAllSaved = true;
                    }
                }
            }
        }
    }
}
