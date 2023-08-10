using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Mumbos_Motors
{
    public class MetaNode
    {
        public Label Name;
        public Panel Header;
        public ComboBox indexer;
        public Panel Background;
        public Panel[] background;
        public int[] spacing;
        public int sideSpacing = 30;
        public int downFix = 10;
        public string Title;
        public int pages;
        public MetaNode(string Title, int pages)
        {
            this.Title = Title;
            this.pages = pages;

            spacing = new int[pages];
            background = new Panel[pages];
            Background = new Panel();
            Background.BackColor = Color.DarkSlateGray;
            Background.Width = 530;
            Background.Height = 50 + sideSpacing;

            Header = new Panel();
            Header.BackColor = Color.DimGray;
            Header.Location = new Point(0, 0);
            Header.Width = Background.Width;
            Header.Height = 30;

            Background.Controls.Add(Header);

            Name = new Label();
            Name.Text = Title;
            Name.ForeColor = Color.LightGray;
            Name.AutoSize = true;
            Name.Location = new Point(10, (Header.Height / 2) - downFix);
            indexer = new ComboBox();
            indexer.Location = new Point(Header.Width - indexer.Width - 5, (Header.Height / 2) - downFix);
            indexer.DropDownClosed += new EventHandler(indexer_ChangeIndex);
            indexer.MouseWheel += new MouseEventHandler(indexer_ChangeIndex);
            Header.Controls.Add(indexer);
            Header.Controls.Add(Name);
            for (int i = 0; i < pages; i++)
            {
                spacing[i] = 0;
                background[i] = new Panel();
                background[i].BackColor = Color.SlateGray;
                background[i].Location = new Point(0, Header.Height);
                background[i].Width = Background.Width - background[i].Location.X;
                background[i].Height = Background.Height - Header.Height;
                background[i].Visible = i == 0;
                Background.Controls.Add(background[i]);
                indexer.Items.Add(i + " - " + (pages - 1) + " (" + pages + ")");
                if (i == 0)
                {
                    indexer.Text = indexer.Items[0].ToString();
                }
            }
            indexer.SelectedIndexChanged += new EventHandler(indexer_ChangeIndex);
        }

        void indexer_ChangeIndex(object sender, EventArgs e)
        {
            string selected = indexer.GetItemText(indexer.SelectedItem);

            string i = "";
            int j = 0;
            while(selected[j] != ' ')
            {
                i += selected[j];
                j++;
            }
            int g = Convert.ToInt32(i);
            for (int h = 0; h < pages; h++)
            {
                background[h].Visible = h == g;
            }
        }
    }
}
