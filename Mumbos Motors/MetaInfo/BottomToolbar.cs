using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Mumbos_Motors.MetaInfo
{
    public class BottomToolbar
    {
        public Panel Background;
        public List<Button> buttons = new List<Button>();
        Color buttonBackColor = Color.White;
        int spacing = 3;

        public BottomToolbar()
        {
            Background = new Panel();
            Background.BackColor = Color.DimGray;
            Background.Width = 650;
            Background.Height = 35;
            Background.Location = new Point(0, 495);

            string[] buttonNames = {"save", "refresh", "export file"};

            for (int i = 0; i < buttonNames.Length; i++)
            {
                addButton(buttonNames[i]);
            }
            buttons[1].Visible = false;
        }

        public void addButton(string title)
        {
            Button newButton = new Button();
            newButton.AutoSize = true;
            newButton.Text = title;
            newButton.BackColor = buttonBackColor;
            newButton.Location = new Point(spacing, (Background.Height / 2) - 12);
            Background.Controls.Add(newButton);
            buttons.Add(newButton);
            spacing += newButton.Width + 7;
        }

        public Panel getBackground()
        {
            return Background;
        }
    }
}
