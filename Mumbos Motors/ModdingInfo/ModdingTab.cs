using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace Mumbos_Motors
{
    public abstract class ModdingTab
    {
        private string name;
        private string symbol;
        public string path;
        public int symbolID;
        public CAFF caff;
        public MULTICAFF multiCaff;
        public int caffIndex;

        public TabPage moddingPage;
        public TabControl moddingTabControl;

        public MetaInfo.MetaTab metaTab;
        public MetaInfo.BottomToolbar bottomToolBar;
        public HexInfo.HexEditor hxd;

        protected ModdingTab(byte[][] sectionData, string name) //MISC DATA
        {
            this.name = name;
            hxd = new HexInfo.HexEditor(sectionData);
            moddingPage = new TabPage();
            moddingPage.Text = name;
            metaTab = new MetaInfo.MetaTab(sectionData);
            coreDesign();
        }

        protected ModdingTab(byte[][] sectionData, string symbol, string path) //FILE
        {
            this.path = path;

            symbol = Path.GetFileName(path);
            hxd = new HexInfo.HexEditor(path, sectionData, symbol);
            moddingPage = new TabPage();
            moddingPage.Text = DataMethods.getNameOfSymbolBackwards(symbol, 1, 1) + "  X";
            metaTab = new MetaInfo.MetaTab(hxd.sectionData, path);
            coreDesign();
        }

        /// <summary>
        /// FILE ID IS THE SYMBOL INDEX, NOT THE LOCATION OF fileInfos[fileID] FILE ID
        /// </summary>
        /// <param name="Caff"></param>
        /// <param name="FileID"></param>
        /// 
        protected ModdingTab(CAFF caff, int symbolID) //CAFF
        {
            this.caff = caff;
            this.symbolID = symbolID;
            symbol = caff.getSymbols()[symbolID];

            hxd = new HexInfo.HexEditor(caff, symbolID);
            moddingPage = new TabPage();
            moddingPage.Text = DataMethods.getNameOfSymbolBackwards(caff.getSymbols()[symbolID], 1, 1) + "  X";
            metaTab = new MetaInfo.MetaTab(caff, hxd.sectionData, symbolID);
            coreDesign();
        }

        protected ModdingTab(MULTICAFF multiCaff, int caffIndex, int symbolID) //MULTICAFF
        {
            this.multiCaff = multiCaff;
            this.caffIndex = caffIndex;

            hxd = new HexInfo.HexEditor(multiCaff, caffIndex, symbolID);
            moddingPage = new TabPage();
            moddingPage.Text = DataMethods.getNameOfSymbolBackwards(multiCaff.caffs[caffIndex].getSymbols()[symbolID], 1, 1) + "  X";
            metaTab = new MetaInfo.MetaTab(multiCaff, hxd.sectionData, caffIndex, symbolID);
            coreDesign();
        }

        public void coreDesign()
        {

            // #constructor#
            metaTab.AllMetaChanged();

            //Bottom Tool Bar
            bottomToolBar = new MetaInfo.BottomToolbar();
            bottomToolBar.buttons[0].Click += new EventHandler(save_Button);
            bottomToolBar.buttons[1].Click += new EventHandler(refresh_Button);
            bottomToolBar.buttons[2].Click += new EventHandler(exportFile);
            moddingPage.Controls.Add(bottomToolBar.Background);

            //Pages for modding
            moddingTabControl = new TabControl();
            moddingTabControl.Selected += new TabControlEventHandler(moddingTabControl_tabChanged);
            moddingTabControl.Width = 650;
            moddingTabControl.Height = 497;
            moddingPage.Controls.Add(moddingTabControl);

            moddingTabControl.TabPages.Add(metaTab.getPage());
            //Add Section pages to moddingTabControl
            for (int i = 0; i < hxd.sectionPages.Length; i++)
            {
                moddingTabControl.TabPages.Add(hxd.sectionPages[i]);
            }
        }

        public abstract void buildMetaPage();

        /// <summary>
        /// Constructs a meta block to modify section data from dynamic controls
        /// </summary>
        /// <param name="title"></param>
        /// <param name="section"></param>
        /// <param name="offs"></param>
        /// <param name="len"></param>
        /// 
        public void MetaBlock_Text(string title, int section, int offs, int len, int format)
        {
            metaTab.buildMetaBlock_Text(title, section, offs, len, format);
        }
        public void MetaBlock_Text(string title, int section, int offs, int len, int format, string nodename, int index)
        {
            metaTab.buildMetaBlock_Text(title, section, offs, len, format, nodename, index);
        }

        public void MetaBlock_Combo(string title, string catagory, string subcatagory, int section, int offs, int len)
        {
            metaTab.buildMetaBlock_ComboRef(title, catagory, subcatagory, section, offs, len);
        }
        public void MetaBlock_Combo(string title, string catagory, string subcatagory, int section, int offs, int len, string nodename, int index)
        {
            metaTab.buildMetaBlock_ComboRef(title, catagory, subcatagory, section, offs, len, nodename, index);
        }

        public void MetaBlock_Combo_Custom(string title, int section, int offs, int len)
        {
            metaTab.buildMetaBlock_ComboRef_Custom(title, section, offs, len);
        }
        public void MetaBlock_Combo_Custom(string title, int section, int offs, int len, string nodename, int index)
        {
            metaTab.buildMetaBlock_ComboRef_Custom(title, section, offs, len, nodename, index);
        }

        public void MetaBock_String(string title, int section, int offs, int len)
        {
            metaTab.buildMetaBlock_String(title, section, offs, len);
        }
        public void MetaBLock_String(string title, int section, int offs, int len, string nodename, int index)
        {
            metaTab.buildMetaBlock_String(title, section, offs, len, nodename, index);
        }

        public void createNode(string title, int pages)
        {
            metaTab.createNode(title, pages);
        }

        /// <summary>
        /// Constructs a meta panel for any fun controls to the section data
        /// </summary>
        public void buildMetaFunPanelButton(string title)
        {
            metaTab.buildMetaFunPanelButton(title);
        }

        public void buildMetaFunPanelButton(string textBoxTitle, string buttonTitle)
        {
            metaTab.buildMetaFunPanelButton(textBoxTitle, buttonTitle);
        }

        public Button latestFunButton()
        {
            return metaTab.metaFunPanel.funButtons[metaTab.metaFunPanel.funButtons.Count - 1];
        }

        public TabPage getModdingTabPage()
        {
            return moddingPage;
        }

        void save_Button(object sender, EventArgs e)
        {
            switch (metaTab.type)
            {
                case 3:
                    {
                        
                        break;
                    }
                default:
                    {
                        hxd.Save();
                        break;
                    }
            }

        }

        void refresh_Button(object sender, EventArgs e)
        {
            hxd.updateHexTextBox(moddingTabControl.SelectedIndex - 1);
        }

        /// <summary>
        /// When block textbox is updated, write to section code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void exportFile(object sender, EventArgs e)
        {
            int size = 0;
            for (int i = 0; i < hxd.sectionData.Length; i++)
            {
                size += hxd.sectionData[i].Length;
            }
            byte[] data = new byte[size];
            int g = 0;
            for (int i = 0; i < hxd.sectionData.Length; i++)
            {
                for (int j = 0; j < hxd.sectionData[i].Length; j++)
                {
                    data[g] = hxd.sectionData[i][j];
                    g++;
                }
            }
            switch (metaTab.type)
            {
                case 0:
                    {
                        if (hxd.isTexture)
                        {
                            symbol = DataMethods.readTextureSymbol(symbol);
                        }
                        DataMethods.saveFileDialog(data, symbol, "Extract File");
                        break;
                    }
                case 1:
                    {
                        if (hxd.isTexture)
                        {
                            symbol = DataMethods.readTextureSymbol(symbol);
                        }
                        DataMethods.saveFileDialog(data, symbol, "Extract File");
                        break;
                    }
                case 2:
                    {
                        if (hxd.isTexture)
                        {
                            symbol = DataMethods.readTextureSymbol(multiCaff.caffs[caffIndex].getSymbols()[symbolID]);
                        }
                        DataMethods.saveFileDialog(data, symbol, "Extract File");
                        break;
                    }
                default:
                    {
                        DataMethods.saveFileDialog(data, name, "Extract File");
                        break;
                    }
            }
        }

        void moddingTabControl_tabChanged(object sender, TabControlEventArgs e)
        {
            hxdRefreshSections();
            bottomToolBar.buttons[1].Visible = !(moddingTabControl.SelectedIndex == 0);
            if (moddingTabControl.SelectedIndex != 0)
            {
                if (metaTab.metaChanged[moddingTabControl.SelectedIndex - 1])
                {
                    hxd.updateHexTextBox_partial(moddingTabControl.SelectedIndex - 1);
                }
                metaTab.metaChanged[moddingTabControl.SelectedIndex - 1] = false;
            }
        }
        void hxdRefreshSections()
        {
            for (int i = 0; i < metaTab.metaChanged.Length; i++)
            {
                if (metaTab.metaChanged[i])
                {
                    hxd.sectionData[i] = metaTab.sectionData[i];
                }
            }
        }
    }
}
