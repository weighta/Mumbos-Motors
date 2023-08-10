using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Mumbos_Motors.MetaInfo
{
    public abstract class MetaBlock
    {
        public Panel Background;
        public Label Title;
        public string title;
        public int section;
        protected MetaBlock(string title, int section)
        {
            this.title = title;
            this.section = section;

            Background = new Panel();
            Background.BackColor = Color.DarkGray;
            Title = new Label();
            Title.AutoSize = true;
            Title.Text = title;
            Title.ForeColor = Color.White;
            Title.Location = new Point(3, 6);
            Background.Controls.Add(Title);
        }

        public Panel getBackground()
        {
            return Background;
        }

        public abstract void design();
    }
}
