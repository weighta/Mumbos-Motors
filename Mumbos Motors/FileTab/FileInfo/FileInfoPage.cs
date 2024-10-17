using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;

namespace Mumbos_Motors.FileTab
{
    public abstract class FileInfoPage
    {
        string path;
        string fileName;
        public TabPage tabPage = new TabPage();
        public Form1 Form;
        public Panel Background;
        public List<Label> infoLabels = new List<Label>();
        public Label fileNameLabel;
        public Label directoryLabel;
        private int numLabels;
        private int spacing = 10;
        private int labelSpacing = 20;

        
        protected FileInfoPage(string dir)
        {
            path = dir;
            fileName = Path.GetFileName(path);
            tabPage = new TabPage();
            tabPage.Text = "File Information";
            tabPage.BackColor = Color.AliceBlue;

            Background = new Panel();
            Background.Location = new Point(spacing, spacing);
            Background.Width = 600;
            Background.Height = 400;

            //Background color based on if the file is decompressed
            if (fileName.Contains("_decompressed"))
            {
                Background.BackColor = Color.LightCoral;
                Button compressbutton = new Button();
                compressbutton.Text = "Recompress";
                compressbutton.Location = new Point(500, 360);
                compressbutton.Size = new Size(100, 40);
                compressbutton.Visible = true;
                compressbutton.BringToFront();
                compressbutton.Click += new EventHandler(compress);
                tabPage.Controls.Add(compressbutton);
            }
            else
            {
                Background.BackColor = Color.Aquamarine;
            }
            
            //extra labels
            fileNameLabel = newLabel(fileName);
            fileNameLabel.Location = new Point(5, 10);
            fileNameLabel.Font = new Font("Arial", 24, FontStyle.Bold);

            directoryLabel = newLabel(path);
            directoryLabel.Font = new Font("Arial", 8.25f, FontStyle.Italic);
            directoryLabel.Location = new Point(fileNameLabel.Location.X, fileNameLabel.Location.Y + 36);

            tabPage.Controls.Add(Background);
        }

        public abstract void labels();

        public Label newLabel(string text)
        {
            Label label = new Label();
            label.Name = "label_info" + (numLabels + 1);
            label.Text = text;
            label.Font = new Font("Arial", 9f, FontStyle.Bold);
            label.AutoSize = true;
            label.Location = new Point(spacing, 75 + (labelSpacing * numLabels)); //Label spacing
            label.Click += new EventHandler(copy);
            Background.Controls.Add(label);
            numLabels++;
            return label;
        }
        public void copy(object sender, EventArgs e)
        {
            Label lab = sender as Label;
            DataMethods.SetClipboard(lab.Text);
        }

        public void compress(object sender, EventArgs e)
        {
            string xboxPath = Environment.ExpandEnvironmentVariables("%XEDK%");

            //Path is not valid, ether path is missing or you need a restart
            if (xboxPath == "%XEDK%")
            {
                if (MessageBox.Show("You need to install the XBOX 360 SDK and restart windows. Would you like to install the SDK?", "SDK ERROR", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("https://www.mediafire.com/file/l9786i9endh5w5e/XBOX360+SDK+21256.3.exe");
                }

            }
            else //Path is valid in which case we want to compress the file and force the Form to load it.
            {
                
                string fullPath = xboxPath + "\\bin\\win32\\xbcompress.exe";
                string newPath = path.Replace("_decompressed", "_recompressed");

                string cmdCommand = "\"" + fullPath + "\" \"" + path + "\" \"" + newPath + "\"";
                //Clipboard.SetText(cmdCommand); //Copys the command to the clipboard for debugging.

                //Starts up xbdecompress and feeds it the path_decompressed and the output as path
                Process p = new Process();
                p.StartInfo.FileName = fullPath;
                p.StartInfo.Arguments = "\"" + path + "\" \"" + newPath + "\"";
                p.Start();

                /*
                We wait until the file has been decompressed, 
                Might want to add a failsafe here but from my testing as long as you close the xbdecompress.exe cmd, Mumbo should be fine.. 
                */
                while (!p.HasExited)
                {

                }

                //If the file Exist aka if xbdecompress.exe did its job, then we load up the file.
                if (File.Exists(newPath))
                {
                    //Form.ForceLoadFile(newPath);
                }
            }
        }
    }
}
