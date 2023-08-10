using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Mumbos_Motors.MetaInfo
{
    public class MetaBlock_text : MetaBlock
    {
        public TextBox textBox;
        public int offs;
        public int len;

        public MetaBlock_text(string title, int section, int offs, int len) : base(title, section)
        {
            this.offs = offs;
            this.len = len;

            design();
        }

        public override void design()
        {
            Background.Width = 505;
            Background.Height = 25;

            textBox = new TextBox();
            textBox.BackColor = Color.White;
            textBox.Location = new Point(Background.Width - textBox.Width - 10, (Background.Height / 2) - 9);
            textBox.Width = 100;
            Background.Controls.Add(textBox);
        }
    }
}
