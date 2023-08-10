using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace Mumbos_Motors.MetaInfo
{
    public struct MetaFunPanel
    {
        public Panel metaPanel;
        public List<Button> funButtons;
        public List<TextBox> funTextBoxes;
    }

    public class MetaTab
    {
        CAFF caff;
        public MULTICAFF multiCaff;
        public int caffIndex;
        public string path;
        public int type;
        public byte[][] sectionData;
        int symbolID;
        public bool[] metaChanged; //bools for reloading section tabs
        public TabPage metaPage;
        public Panel Background;
        public FlowLayoutPanel scroller;
        public Label label_symbol;

        public List<MetaNode> nodes = new List<MetaNode>();
        public List<MetaBlock_text> metaBlockTexts = new List<MetaBlock_text>();
        public List<MetaBlock_tagref> metaTagrefs = new List<MetaBlock_tagref>();
        public List<MetaBlock_string> metaStrings = new List<MetaBlock_string>();
        public List<tagref_custom> metaTagRefs_Custom = new List<tagref_custom>();
        public MetaFunPanel metaFunPanel;
        public int sideSpacing = 30;
        public int metaSpacing = 25;

        public MetaTab(byte[][] sectionData) //MISC FILE
        {
            type = 3;
            this.sectionData = sectionData;
            coreDesign();
        }

        public MetaTab(byte[][] sectionData, string path) //FILE
        {
            type = 0;
            this.sectionData = sectionData;
            this.path = path;
            coreDesign();
        }

        public MetaTab(CAFF caff, byte[][] sectionData, int symbolID) //CAFF
        {
            type = 1;
            this.caff = caff;
            this.symbolID = symbolID;
            this.sectionData = sectionData;
            coreDesign();
        }

        public MetaTab(MULTICAFF multiCaff, byte[][] sectionData, int caffIndex, int symbolID) //MULTICAFF
        {
            type = 2;
            this.multiCaff = multiCaff;
            this.sectionData = sectionData;
            this.caffIndex = caffIndex;
            coreDesign();
        }

        public void coreDesign()
        {
            metaChanged = new bool[sectionData.Length];

            metaPage = new TabPage();
            scroller = new FlowLayoutPanel();
            Background = new Panel();
            label_symbol = new Label();

            metaPage.Text = "meta editor";
            scroller.Width = 635;
            scroller.Height = 470;
            scroller.AutoScroll = true;
            scroller.WrapContents = false;
            scroller.BackColor = Color.CornflowerBlue;
            Background.BackColor = Color.LightGray;
            Background.Width = 600;
            Background.Height = 497;
            Background.Location = new Point(50, 50);
            label_symbol.AutoSize = true;
            switch (type)
            {
                case 0:
                    {
                        label_symbol.Text = Path.GetFileName(path);
                        break;
                    }
                case 1:
                    {
                        label_symbol.Text = caff.getSymbols()[symbolID];
                        break;
                    }
                case 2:
                    {
                        label_symbol.Text = multiCaff.caffs[caffIndex].getSymbols()[symbolID];
                        break;
                    }
                case 3:
                    {
                        label_symbol.Text = "[SYMBOL NAME GOES HERE]";
                        break;
                    }
            }

            label_symbol.Click += new EventHandler(label_symbol_click);
            label_symbol.Location = new Point(sideSpacing / 4, sideSpacing / 4);
            Background.Controls.Add(label_symbol);
            scroller.Controls.Add(Background);
            metaPage.Controls.Add(scroller);
        }


        public void createNode(string title, int pages)
        {
            MetaNode node = new MetaNode(title, pages);
            node.Background.Location = new Point(sideSpacing, metaSpacing);
            Background.Controls.Add(node.Background);
            nodes.Add(node);
            metaSpacing += node.Background.Height + sideSpacing;
        }

        public void addMetaToBackground(MetaBlock a)
        {
            a.Background.Location = new Point(sideSpacing, metaSpacing);
            Background.Controls.Add(a.Background);
            metaSpacing += a.Background.Height + sideSpacing;
            PageResize();
        }
        public void addMetaToNode(MetaBlock a, string nodename, int index)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodename == nodes[i].Title)
                {
                    //MessageBox.Show("index: " + index + "\nnodes len: " + nodes.Count + "\nspacing len: " + nodes[i].spacing.Length);
                    a.Background.Location = new Point(sideSpacing / 2, nodes[i].spacing[index] + (sideSpacing / 2));
                    nodes[i].spacing[index] += sideSpacing;
                    nodes[i].background[index].Controls.Add(a.Background);
                    nodes[i].background[index].Height += sideSpacing;
                    if (index == 0)
                    {
                        nodes[i].Background.Height += sideSpacing;
                    }
                    metaSpacing += Convert.ToInt32(index == 0) * sideSpacing;
                    i = nodes.Count();
                }
            }
            PageResize();
        }

        public MetaBlock_text createBlock_Text(string title, int section, int offs, int len, int format)
        {
            //MetaBlock block = new MetaBlock();
            MetaBlock_text block = new MetaBlock_text(title, section, offs, len);
            block.textBox.Name = (section - 1) + " " + format + " " + metaBlockTexts.Count() + " " + 0;
            block.textBox.Text = DataMethods.readInt(sectionData[block.section - 1], block.offs, block.len) + "";
            if (format == 1)
            {
                block.textBox.Text = Convert.ToInt32(block.textBox.Text).ToString("X" + (2 * len));
                block.textBox.MaxLength = 2 * len;
            }
            else if (format == 2)
            {
                block.textBox.Text = DataMethods.readFloat(block.textBox.Text) + "";
            }
            block.textBox.TextChanged += new EventHandler(MetaBlock_Text_update);

            metaBlockTexts.Add(block);
            return block;
        }
        public void buildMetaBlock_Text(string title, int section, int offs, int len, int format)
        {
            addMetaToBackground(createBlock_Text(title, section, offs, len, format));
        }
        public void buildMetaBlock_Text(string title, int section, int offs, int len, int format, string nodename, int index)
        {
            addMetaToNode(createBlock_Text(title, section, offs, len, format), nodename, index);
        }

        public MetaBlock_tagref createBlock_ComboRef(string title, string catagory, string subcatagory, int section, int offs, int len)
        {
            int ishex = 1;
            MetaBlock_tagref block = new MetaBlock_tagref(title, catagory, subcatagory, section, offs, len);
            switch (type)
            {
                case 0:
                    {
                        block = new MetaBlock_tagref(title, catagory, subcatagory, section, offs, len);
                        break;
                    }
                case 1:
                    {
                        block = new MetaBlock_tagref(title, catagory, subcatagory, section, offs, len, caff);
                        break;
                    }
                case 3:
                    {
                        block = new MetaBlock_tagref(title, catagory, subcatagory, section, offs, len);
                        break;
                    }
            }
            block.textBox.Name = (section - 1) + " " + ishex + " " + metaTagrefs.Count() + " " + 1;
            block.textBox.Text = DataMethods.readInt(sectionData[section - 1], block.offs, block.len) + "";
            block.comboBox.Name = metaTagrefs.Count + " " + section;
            switch (type)
            {
                case 0:
                    {
                        block.comboBox.Text = Path.GetFileName(path);
                        break;
                    }
                case 1:
                    {
                        block.comboBox.Text = caff.getSymbols()[symbolID];
                        break;
                    }
                case 2:
                    {
                        block.comboBox.Text = multiCaff.caffs[caffIndex].getSymbols()[symbolID];
                        break;
                    }
            }

            if (ishex == 1)
            {
                block.textBox.Text = Convert.ToInt32(block.textBox.Text).ToString("X" + (2 * len));
                block.textBox.MaxLength = 2 * len;
            }
            block.comboBox.TextChanged += new EventHandler(MetaBlock_ComboRef_update);
            block.textBox.TextChanged += new EventHandler(MetaBlock_Text_update);

            metaTagrefs.Add(block);
            return block;
        }
        public void buildMetaBlock_ComboRef(string title, string catagory, string subcatagory, int section, int offs, int len)
        {
            addMetaToBackground(createBlock_ComboRef(title, catagory, subcatagory, section, offs, len));
        }
        public void buildMetaBlock_ComboRef(string title, string catagory, string subcatagory, int section, int offs, int len, string nodename, int index)
        {
            addMetaToNode(createBlock_ComboRef(title, catagory, subcatagory, section, offs, len), nodename, index);
        }

        public tagref_custom createBlock_ComboRef_Custom(string title, int section, int offs, int len)
        {
            int ishex = 1;
            tagref_custom block = new tagref_custom(title, section, offs, len);
            block.textBox.Name = (section - 1) + " " + ishex + " " + metaTagRefs_Custom.Count() + " " + 2; //section, ishex, blockID, type
            block.textBox.Text = DataMethods.readInt(sectionData[section - 1], block.offs, block.len) + "";
            block.comboBox.Name = metaTagRefs_Custom.Count + " " + section;
            switch (type)
            {
                case 0:
                    {
                        block.comboBox.Text = Path.GetFileName(path);
                        break;
                    }
                case 1:
                    {
                        block.comboBox.Text = caff.getSymbols()[symbolID];
                        break;
                    }
                case 2:
                    {
                        block.comboBox.Text = multiCaff.caffs[caffIndex].getSymbols()[symbolID];
                        break;
                    }
            }

            if (ishex == 1)
            {
                block.textBox.Text = Convert.ToInt32(block.textBox.Text).ToString("X" + (2 * len));
                block.textBox.MaxLength = 2 * len;
            }
            block.textBox.TextChanged += new EventHandler(MetaBlock_Text_update);

            metaTagRefs_Custom.Add(block);
            return block;
        }
        public void buildMetaBlock_ComboRef_Custom(string title, int section, int offs, int len)
        {
            addMetaToBackground(createBlock_ComboRef_Custom(title, section, offs, len));
        }
        public void buildMetaBlock_ComboRef_Custom(string title, int section, int offs, int len, string nodename, int index)
        {
            addMetaToNode(createBlock_ComboRef_Custom(title, section, offs, len), nodename, index);
        }

        public MetaBlock_string createBlock_String(string title, int section, int offs, int len)
        {
            MetaBlock_string block = new MetaBlock_string(title, section, offs, len);
            block.textBox.Name = (section - 1) + " " + metaStrings.Count;
            block.textBox.Text = DataMethods.readString(sectionData[section - 1], offs, len);
            block.textBox.MaxLength = len;
            block.textBox.TextChanged += new EventHandler(MetaBlock_String_update);

            metaStrings.Add(block);
            return block;
        }
        public void buildMetaBlock_String(string title, int section, int offs, int len)
        {
            addMetaToBackground(createBlock_String(title, section, offs, len));
        }
        public void buildMetaBlock_String(string title, int section, int offs, int len, string nodename, int index)
        {
            addMetaToNode(createBlock_String(title, section, offs, len), nodename, index);
        }

        public void buildMetaFunPanel()
        {
            metaFunPanel = new MetaFunPanel();
            metaFunPanel.metaPanel = new Panel();
            metaFunPanel.metaPanel.BackColor = Color.Orange;
            metaFunPanel.metaPanel.Width = 200;
            metaFunPanel.metaPanel.Height = 50;
            metaFunPanel.metaPanel.Location = new Point(sideSpacing, metaSpacing);
            metaFunPanel.funButtons = new List<Button>();
            metaFunPanel.funTextBoxes = new List<TextBox>();

            buildMetaFunPanelButton("Null Data Sections");
            metaFunPanel.funButtons[metaFunPanel.funButtons.Count - 1].Click += new EventHandler(NullDataSections);
            Background.Controls.Add(metaFunPanel.metaPanel);
            metaSpacing += metaFunPanel.metaPanel.Height;
        }
        public Button buildMetaFunPanelButton(string title)
        {
            Button button = new Button();
            button.Text = title;
            button.Location = new Point(sideSpacing, (metaFunPanel.funButtons.Count * sideSpacing) + sideSpacing);
            button.BackColor = Color.White;
            button.AutoSize = true;
            metaFunPanel.metaPanel.Controls.Add(button);
            metaFunPanel.funButtons.Add(button);
            metaFunPanel.metaPanel.Height += sideSpacing;
            metaSpacing += sideSpacing;
            return button;
        }
        public void buildMetaFunPanelButton(string textBoxTitle, string buttonTitle)
        {
            TextBox txtbox = new TextBox();
            Button button = buildMetaFunPanelButton(buttonTitle);
            button.Name = metaFunPanel.funTextBoxes.Count() + "";

            txtbox.Text = textBoxTitle;
            txtbox.Width = 20;
            txtbox.Location = new Point(sideSpacing - txtbox.Width, button.Location.Y);
            txtbox.BackColor = Color.White;
            txtbox.AutoSize = true;

            metaFunPanel.metaPanel.Controls.Add(txtbox);
            metaFunPanel.funTextBoxes.Add(txtbox);
            metaFunPanel.metaPanel.Height += sideSpacing;
            metaSpacing += sideSpacing;
            PageResize();
        }

        void label_symbol_click(object sender, EventArgs e)
        {
            Label symbol = sender as Label;
            DataMethods.SetClipboard(symbol.Text);
        }

        public TabPage getPage()
        {
            return metaPage;
        }

        void MetaBlock_Text_update(object sender, EventArgs e)
        {
            TextBox box = sender as TextBox;
            string[] info = box.Name.Split(' '); //section, ishex, blockid, type
            int format = Convert.ToInt32(info[1]);
            int blockID = Convert.ToInt32(info[2]);
            int type = Convert.ToInt32(info[3]);
            try
            {
                int towrite = 0;
                switch (format)
                {
                    case 0:
                        {
                            towrite = Convert.ToInt32(box.Text);
                            break;
                        }
                    case 1:
                        {
                            towrite = Convert.ToInt32(box.Text, 16);
                            break;
                        }
                    case 2:
                        {
                            towrite = Convert.ToInt32(box.Text, 16);
                            break;
                        }
                }
                switch (type)
                {
                    case 0:
                        {
                            MetaBlock_Text_Write(metaBlockTexts[blockID], blockID, towrite, metaBlockTexts[blockID].offs, metaBlockTexts[blockID].len);
                            break;
                        }
                    case 1:
                        {
                            MetaBlock_Text_Write(metaTagrefs[blockID], blockID, towrite, metaTagrefs[blockID].offs, metaTagrefs[blockID].len);
                            break;
                        }
                    case 2:
                        {
                            MetaBlock_Text_Write(metaTagRefs_Custom[blockID], blockID, towrite, metaTagRefs_Custom[blockID].offs, metaTagRefs_Custom[blockID].len);
                            break;
                        }
                }
                box.ForeColor = Color.Black;
            }
            catch
            {
                box.ForeColor = Color.Red;
            }
            metaChanged[Convert.ToInt32(info[0])] = true;
        }

        public void MetaBlock_Text_Write(MetaBlock block, int blockID, int towrite, int offs, int len)
        {
            sectionData[block.section - 1] = DataMethods.writeInt(sectionData[block.section - 1], offs, towrite, len);
        }

        void MetaBlock_ComboRef_update(object sender, EventArgs e)
        {
            ComboBox com = sender as ComboBox;
            string[] infos = com.Name.Split(' '); //blockID, section
            //MessageBox.Show(infos[0] + "\n" + infos[1]);
            int blockID = Convert.ToInt32(infos[0]);
            int section = Convert.ToInt32(infos[1]) - 1;
            string pickedItem = com.GetItemText(com.SelectedItem);

            int symbolID;
            byte[] tempSectionData = new byte[0];
            switch (type)
            {
                case 0:
                    {

                        break;
                    }
                case 1:
                    {
                        symbolID = DataMethods.getIndexBySearch(caff.getSymbols(), pickedItem);
                        tempSectionData = caff.readSectionData(symbolID, section);
                        break;
                    }
                case 2:
                    {
                        symbolID = DataMethods.getIndexBySearch(multiCaff.caffs[multiCaff.getCaffIndexBySymbol(pickedItem)].getSymbols(), pickedItem);
                        tempSectionData = multiCaff.readSectionCAFF(caffIndex, symbolID, section);
                        break;
                    }
            }
            metaTagrefs[blockID].textBox.Text = DataMethods.readInt32(tempSectionData, metaTagrefs[blockID].offs).ToString("X8");
        }

        void MetaBlock_String_update(object sender, EventArgs e)
        {
            TextBox box = sender as TextBox;
            string[] info = box.Name.Split(' ');
            int section = Convert.ToInt32(info[0]);
            int blockID = Convert.ToInt32(info[1]);
            DataMethods.writeString(sectionData[section], box.Text, metaStrings[blockID].offs, metaStrings[blockID].len);
            metaChanged[section] = true;
        }

        public void AllMetaChanged()
        {
            for (int i = 0; i < metaChanged.Length; i++)
            {
                metaChanged[i] = true;
            }
        }

        void NullDataSections(object sender, EventArgs e)
        {
            for (int i = 0; i < sectionData.Length; i++)
            {
                for (int j = 0; j < sectionData[i].Length; j++)
                {
                    sectionData[i][j] = 0x0;
                }
            }
            AllMetaChanged();
            MessageBox.Show("All sections nulled successfully. Don't forget to save it :)");
        }

        void PageResize()
        {
            if (Background.Height < metaSpacing)
            {
                Background.Height = metaSpacing;
            }
        }
    }
}
