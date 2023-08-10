using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Mumbos_Motors.MetaInfo
{
    public class tagref_custom : MetaBlock
    {
        public TextBox textBox;
        public ComboBox comboBox;
        public int offs;
        public int len;

        public tagref_custom(string title, int section, int offs, int len) : base(title, section)
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
            textBox.Width = 58;
            textBox.Location = new Point(Background.Width - textBox.Width - 10, (Background.Height / 2) - 9);
            Background.Controls.Add(textBox);

            comboBox = new ComboBox();
            comboBox.BackColor = Color.White;
            comboBox.Width = 360;
            comboBox.Location = new Point(Background.Width - comboBox.Width - textBox.Width - 11, (Background.Height / 2) - 9);
            Background.Controls.Add(comboBox);
        }
    }
}
