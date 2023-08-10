using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace Mumbos_Motors.FileTab
{
    public abstract class FileInfoPage
    {
        string path;
        string fileName;
        public TabPage tabPage = new TabPage();
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
            Background.BackColor = Color.Aquamarine;
            Background.Width = 300;
            Background.Height = 400;

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
    }
}
