using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using GetID;

namespace VidProject
{
    public partial class MainForm : Form
    {
        // constant and static fields
        private const string strRegexID = "^[A-Z][0-9]{4}$";
        private const string csvFileName = "ment.csv";
        private static readonly Color errorColor = Color.Red;
        private static readonly Color normalColor = Color.Black;
        private static readonly Color warningColor = Color.Blue;


        readonly GetID.GetID getID;
        private IStoreData csvFileWriter;
        private bool isAllSaved;

        public MainForm()
        {
            InitializeComponent();
            getID = new GetID.GetID();
            // getID.Running field is true on startup for some reason --> it is readonly therefore call the stop method to set false
            getID.Stop();
            // register event handlers into getID
            getID.ValueChanged += getID_ValueChange;
            getID.ErrorChanged += getID_ErrorChanged;
            // permanent csv backup writer
            csvFileWriter = new StoreDataToCsv(csvFileName);
            // initialize the message label
            SetLblMessageTextWithColoring("", normalColor);
            // initialize the saving status
            isAllSaved = true;
        }

       
        // handlers of GetID events
         private void getID_ErrorChanged(object sender, EventArgs e)
        {
            SetLblMessageTextWithColoring(getID.ErrorMessage, errorColor);
        }

        private void getID_ValueChange(object sender, EventArgs e)
        {
            string[] id = { getID.Value, idRegexTester(getID.Value) ? "OK" : "NOK" };
            tbxMain.Invoke((Action)delegate
            {
                if (!tbxMain.Text.Equals(""))
                {
                    tbxMain.AppendText(Environment.NewLine);
                }
                tbxMain.AppendText(id[0] + " - " + id[1]);
                csvFileWriter.WriteFile(id, true);
                // received new id therefore not all data are saved
                isAllSaved = false;
            });
        }

        // form event handlers
        private void btnGo_Click(object sender, EventArgs e)
        {
            if (!getID.Running)
            {
                getID.Go();
                SetLblMessageTextWithColoring("Started", normalColor);
            }
            else
            {
                SetLblMessageTextWithColoring("It is already running!", warningColor);
            }

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (getID.Running)
            {
                getID.Stop();
                SetLblMessageTextWithColoring("Stopped", normalColor);
            }
            else
            {
                SetLblMessageTextWithColoring("It is not running!", warningColor);
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (getID.Running)
            {
                SetLblMessageTextWithColoring("Please stop it before clearing", warningColor);
            } else if (isAllSaved)
            {
                SetLblMessageTextWithColoring("Cleared", normalColor);
                tbxMain.Clear();
            } else 
            {
                if (MessageBox.Show("Do you want to save changes before clear?", "Confirmation",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if(SaveToTxtFile())
                    {
                        isAllSaved = true;
                    }
                } else
                {
                    SetLblMessageTextWithColoring("Cleared without saving", warningColor);
                    tbxMain.Clear();
                    isAllSaved = true;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (getID.Running)
            {
                SetLblMessageTextWithColoring("Please stop it before saving!", warningColor);
            }
            else if (tbxMain.Text is null || tbxMain.Text.Equals(""))
            {
                SetLblMessageTextWithColoring("Nothing to save!", errorColor);
            } 
            else 
            {
                if (SaveToTxtFile())
                {
                    SetLblMessageTextWithColoring("The writing was successful", normalColor);
                    isAllSaved=true;
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isAllSaved)
            {
                if (MessageBox.Show("Do you want to save changes before exit?", "My Application",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if (SaveToTxtFile())
                    {
                        isAllSaved = true;
                    }
                }
            }
        }

        // other methods

        // test ID format
        private bool idRegexTester(string id)
        {
            return new Regex(strRegexID).Match(id).Success;
        }

        private void SetLblMessageTextWithColoring(string text, Color color)
        {
            lblMessage.Text = text;
            lblMessage.ForeColor = color;
        }

        private bool SaveToTxtFile()
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
                    SetLblMessageTextWithColoring("Error during writing: "+ex.Message, errorColor);
                    return false;
                }
                return true;
            }
            return false;

        }
    }
}
