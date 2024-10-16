using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Data.Common;
using System.Threading;

namespace Mumbos_Motors
{
    class FilePage
    {
        /// <summary>
        /// Construct a tabPage for the file and what the file is about and add it to the main_tabControl
        /// </summary>

        public bool error = false;
        public string fileName;
        CAFF caff;
        MULTICAFF multiCAFF;

        //Main
        public TabPage TabPage_mainpage;
        public TabControl tabControl;

        //Page1
        private FileTab.FileInfoPage fileInfoPage;

        //Page2
        private FileTab.TagsCAFF tagsPage;



        public FilePage(string dir, Form1 Form)
        {
            fileName = Path.GetFileName(dir);

            int headerWord = DataMethods.readInt32(dir, 0x0);
            switch (headerWord)
            {
                case 0x43414646: //CAFF
                    {
                        caff = new CAFF(dir);
                        TabPage_mainpage = new TabPage();
                        TabPage_mainpage.Text = fileName + "        ";

                        tabControl = new TabControl();
                        tabControl.Width = 1063;
                        tabControl.Height = 605;

                        tabControl.TabPages.Add(new FileTab.FileInfo.InfoCAFF(dir, caff).tabPage);
                        tabControl.TabPages.Add(new FileTab.TagsCAFF(caff).tabPage);

                        //spawn tabcontrol in filepage
                        TabPage_mainpage.Controls.Add(tabControl);
                        break;
                    }

                case 0x438CB47C: //MULTICAFF
                    {
                        multiCAFF = new MULTICAFF(dir);
                        TabPage_mainpage = new TabPage();
                        TabPage_mainpage.Text = fileName + "        ";

                        tabControl = new TabControl();
                        tabControl.Width = 1063;
                        tabControl.Height = 605;

                        tabControl.TabPages.Add(new FileTab.FileInfo.InfoMULTICAFF(dir, multiCAFF).tabPage);
                        tabControl.TabPages.Add(new FileTab.TagsInfo.TagsMULTICAFF(multiCAFF).tabPage);

                        //spawn tabcontrol in filepage
                        TabPage_mainpage.Controls.Add(tabControl);
                        break;
                    }

                case 267719405: //XB COMPRESSED FILE
                    {
                        string xboxPath = Environment.ExpandEnvironmentVariables("%XEDK%");
                        if (xboxPath == "%XEDK%")
                        {
                            error = true; ;
                            //MessageBox.Show("Error, You need to ether install the XBOX 360 SDK or Run in administrator.");
                            if (MessageBox.Show("You need to install the XBOX 360 SDK and restart windows. Would you like to install the SDK?", "SDK ERROR", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                            {
                                System.Diagnostics.Process.Start("https://www.mediafire.com/file/l9786i9endh5w5e/XBOX360+SDK+21256.3.exe");
                            }
                            
                            caff = new CAFF(dir);
                            break;
                        }
                        else
                        {
                            error = true; ;
                            string fullPath = xboxPath + "\\bin\\win32\\xbdecompress.exe";

                            // "/C " will terminate the window after running the command and "/K " will keep the window open
                            string cmdCommand = "\"" + fullPath + "\" \"" + dir + "\" \"" + dir + "_decompressed\"";
                            Clipboard.SetText(cmdCommand);

                            //System.Diagnostics.Process.Start("CMD.exe", cmdCommand);

                            Process p = new Process();
                            p.StartInfo.FileName = fullPath;
                            p.StartInfo.Arguments = "\"" + dir + "\" \"" + dir + "_decompressed\"";
                            
                            p.Start();

                            while (!p.HasExited)
                            {
                                
                            }

                            error = true; ;
                            dir += "_decompressed";
                            //FilePage(dir);
                            //caff = new CAFF(dir);
                            Form.ForceLoadFile(dir);
                            caff = new CAFF(dir);
                            break;
                        }
                        error = true; ;
                        MessageBox.Show("Could not find xbdecompress at: " + xboxPath);
                        caff = new CAFF(dir);
                        break;

                    }
                default:
                    {
                        error = true; ;
                        MessageBox.Show("Error opening: " + Path.GetFileName(fileName) + "\n" + DataMethods.readString(dir, 0x0, 0x4));
                        caff = new CAFF(dir);
                        break;
                    }
            }
        }

        public bool getError()
        {
            return error;
        }

        public bool checkCaffFile()
        {
            if (caff.getError())
            {
                MessageBox.Show("There was a problem reading this CAFF file:\n - " + caff.getErrorMessage());
            }
            return caff.getError();
        }
    }
}
