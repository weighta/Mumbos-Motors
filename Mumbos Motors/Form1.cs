using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace Mumbos_Motors
{

    public partial class Form1 : Form
    {
        private int numFileTabsOpen = 0;
        private List<FilePage> toolBars = new List<FilePage>();
        private List<string> openTabNames = new List<string>();

        public Form1()
        {
            InitializeComponent();
            createStartPage();
        }

        public void ForceLoadFile(string filename)
        {
            addTab_Main_File(filename);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        //LOAD DRAGGED FILES INTO STRING LIST
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            createFilePages(files);
        }
        
        //OPEN CAFF FILES INTO PROGRAM
        private void createFilePages(string[] fileNames)
        {
            for (int i = 0; i < fileNames.Length; i++)
            {
                addTab_Main_File(fileNames[i]);
            }
        }

        /// <summary>
        /// Start Page Tab
        /// </summary>
        /// <param name="title"></param>
        /// <param name="labels"></param>
        private void addTab_Main(string title, Label[] labels)
        {
            TabPage page = new TabPage();
            page.BackColor = Color.LightGray;
            page.Text = title + "      ";

            for (int i = 0; i < labels.Length; i++)
            {
                page.Controls.Add(labels[i]);
            }

            //Add the TabPage to tabPage_Main
            tabControl.TabPages.Add(page);

        }

        /// <summary>
        /// File Page Tab
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="dir"></param>
        private void addTab_Main_File(string dir)
        {
            FilePage filePage = new FilePage(dir,this);
            toolBars.Add(filePage);


            if (!toolBars[toolBars.Count - 1].getError())
            {
                //ADD THE ENTIRE TABCONTROL INSIDE FILE-TAB FOR MAIN_TAB
                tabControl.TabPages.Add(toolBars[toolBars.Count - 1].TabPage_mainpage);
                tabControl.SelectTab(tabControl.TabCount - 1);
            }
            else
            {
                toolBars.Remove(toolBars[toolBars.Count - 1]);
            }
        }

        private void createStartPage()
        {
            Label[] labels = new Label[1];
            Point[] points = new Point[1];
            labels[0] = new Label();
            points[0] = new Point();
            //Title
            labels[0].Text = "Mumbos Motars";
            labels[0].Location = new Point(20, 60);

            addTab_Main("Start Page", labels);
        }


        private void startPageToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            createStartPage();
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void tabControl_Main_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.Graphics.DrawString("X", e.Font, Brushes.Black, e.Bounds.Right - 15, e.Bounds.Top + 4);
            e.Graphics.DrawString(this.tabControl.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + 12, e.Bounds.Top + 4);
            e.DrawFocusRectangle();
        }

        private void tabControl_Main_MouseDown(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < this.tabControl.TabPages.Count; i++)
            {
                Rectangle r = tabControl.GetTabRect(i);
                //Getting the position of the "x" mark.
                Rectangle closeButton = new Rectangle(r.Right - 15, r.Top + 4, 9, 7);
                if (closeButton.Contains(e.Location))
                {
                    this.tabControl.TabPages.RemoveAt(i);
                    toolBars.Remove(toolBars[i - 1]);
                    numFileTabsOpen--;
                    break;
                }
            }
        }
        public static string getTitle()
        {
            return "Mombo's Motars";
        }
    }
}
