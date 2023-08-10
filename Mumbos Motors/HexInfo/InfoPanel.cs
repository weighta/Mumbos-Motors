using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Mumbos_Motors.HexInfo
{
    public class InfoPanel
    {
        public Panel Background;
        private Font font;
        public Label error;
        public Label sectionLen;
        public Label offs;
        public Label fileInfoOffs;
        private int height;
        private int spacing;
        private int newspacing;
        public InfoPanel(Point location, int height)
        {
            this.height = height;

            Background = new Panel();
            Background.BackColor = Color.Turquoise;
            Background.Location = location;
            Background.Height = height;
            Background.Width = 200;

            spacing = location.Y / 2;
            newspacing = spacing;
            font = new Font("Arial", 8.25f, FontStyle.Bold);
            //error Label
            error = newlabel();
            error.Text = "Length OK";

            //length label
            sectionLen = newlabel();
            
            //offs label
            offs = newlabel();

            fileInfoOffs = newlabel();
        }

        public Label newlabel()
        {
            Label label = new Label();
            Background.Controls.Add(label);
            label.Font = font;
            label.AutoSize = true;
            label.Location = new Point(spacing, newspacing);
            label.Click += new EventHandler(copy);
            newspacing += label.Height + spacing;
            return label;
        }
        void copy(object sender, EventArgs e)
        {
            Label lab = sender as Label;
            DataMethods.SetClipboard(lab.Text);
        }
    }
}
