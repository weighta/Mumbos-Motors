using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

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



        public FilePage(string dir)
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
