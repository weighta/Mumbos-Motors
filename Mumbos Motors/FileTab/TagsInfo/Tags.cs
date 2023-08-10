using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Mumbos_Motors.FileTab.TagsInfo
{
    public abstract class Tags
    {
        public int type; //CAFF: 0 MultiCAFF: 1
        public CAFF caff;
        public MULTICAFF multiCaff;
        public TabPage tabPage = new TabPage();
        public TreeView Treeview_tags;
        public TextBox searchBar;

        public TabControl moddingTabControl;
        public List<object> moddingpages = new List<object>();

        public Tags()
        {
            tabPage.Text = "Tags";
            tabPage.BackColor = Color.DimGray;

            //Build TreeView
            Treeview_tags = new TreeView();
            Treeview_tags.Width = 400;
            Treeview_tags.Height = 530;
            Treeview_tags.Location = new Point(0, 25);
            Treeview_tags.NodeMouseClick += new TreeNodeMouseClickEventHandler(Treeview_tags_AfterSelect);

            tabPage.Controls.Add(Treeview_tags); //Add treeview to page

            //Searchbar for tags
            searchBar = new TextBox();
            searchBar.Width = 397;
            searchBar.Location = new Point(3, 3);

            tabPage.Controls.Add(searchBar);

            //Modding Tab control
            moddingTabControl = new TabControl();
            moddingTabControl.Width = 650;
            moddingTabControl.Height = 555;
            moddingTabControl.Location = new Point(400, 0);
            moddingTabControl.MouseDown += new MouseEventHandler(this.tabControl_moddingPages_MouseDown);
            moddingTabControl.MouseClick += moddingTabControl_MouseClick;
            tabPage.Controls.Add(moddingTabControl);
        }

        private void Treeview_tags_AfterSelect(object sender, TreeNodeMouseClickEventArgs e)
        {
            string tabName = e.Node.Text;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        //Check if tab already exists
                        bool exists = false;
                        foreach (TabPage tab in moddingTabControl.TabPages)
                        {
                            //MessageBox.Show("if " + tab.Text + " = " + tabName);
                            if (tab.Text.Contains(DataMethods.getNameOfSymbolBackwards(tabName, 1, 1)))
                            {
                                exists = true;
                            }
                        }
                        //if not, add tabPage
                        if (!exists && tabName.Contains('_'))
                        {
                            switch (type)
                            {
                                case 0:
                                    {
                                        ModdingTab moddingTab = new Default(caff, DataMethods.getIndexBySearch(caff.getSymbols(), tabName));
                                        switch (DataMethods.getNameOfSymbol(tabName, 4, 0))
                                        {
                                            case "texture":
                                                {
                                                    moddingTab = new texture(caff, DataMethods.getIndexBySearch(caff.getSymbols(), DataMethods.rebuildTextureSymbol(tabName)));
                                                    break;
                                                }
                                            case "model":
                                                {
                                                    moddingTab = new model(caff, DataMethods.getIndexBySearch(caff.getSymbols(), tabName));
                                                    break;
                                                }
                                            case "objparams":
                                                {
                                                    moddingTab = new objparam(caff, DataMethods.getIndexBySearch(caff.getSymbols(), tabName));
                                                    break;
                                                }
                                            case "vehicle":
                                                {
                                                    moddingTab = new vehicle(caff, DataMethods.getIndexBySearch(caff.getSymbols(), tabName));
                                                    break;
                                                }
                                            default:
                                                {
                                                    moddingTab = new Default(caff, DataMethods.getIndexBySearch(caff.getSymbols(), tabName));
                                                    break;
                                                }

                                        }
                                        addTab(moddingTab, moddingTab.getModdingTabPage());
                                        break;
                                    }
                                case 1:
                                    {
                                        ModdingTab moddingTab = new Default(multiCaff, multiCaff.getCaffIndexBySymbol(tabName), DataMethods.getIndexBySearch(multiCaff.caffs[multiCaff.getCaffIndexBySymbol(tabName)].getSymbols(), tabName));
                                        switch (DataMethods.getNameOfSymbol(tabName, 4, 0))
                                        {
                                            case "texture":
                                                {
                                                    moddingTab = new texture(multiCaff, multiCaff.getCaffIndexBySymbol(DataMethods.rebuildTextureSymbol(tabName)), DataMethods.getIndexBySearch(multiCaff.caffs[multiCaff.getCaffIndexBySymbol(tabName)].getSymbols(), DataMethods.rebuildTextureSymbol(tabName)));
                                                    break;
                                                }
                                            case "model":
                                                {
                                                    moddingTab = new model(multiCaff, multiCaff.getCaffIndexBySymbol(tabName), DataMethods.getIndexBySearch(multiCaff.caffs[multiCaff.getCaffIndexBySymbol(tabName)].getSymbols(), tabName));
                                                    break;
                                                }
                                            case "objparams":
                                                {
                                                    moddingTab = new objparam(multiCaff, multiCaff.getCaffIndexBySymbol(tabName), DataMethods.getIndexBySearch(multiCaff.caffs[multiCaff.getCaffIndexBySymbol(tabName)].getSymbols(), tabName));
                                                    break;
                                                }
                                            case "vehicle":
                                                {
                                                    moddingTab = new vehicle(multiCaff, multiCaff.getCaffIndexBySymbol(tabName), DataMethods.getIndexBySearch(multiCaff.caffs[multiCaff.getCaffIndexBySymbol(tabName)].getSymbols(), tabName));
                                                    break;
                                                }
                                            default:
                                                {
                                                    moddingTab = new Default(multiCaff, multiCaff.getCaffIndexBySymbol(tabName), DataMethods.getIndexBySearch(multiCaff.caffs[multiCaff.getCaffIndexBySymbol(tabName)].getSymbols(), tabName));
                                                    break;
                                                }

                                        }
                                        addTab(moddingTab, moddingTab.getModdingTabPage());
                                        break;
                                    }
                            }

                        }
                        if (tabName.Substring(tabName.Length - 4) == ".xwb")
                        {
                            string soundName = tabName.Substring(0, tabName.Length - 4);
                            byte[][] sectionData = new byte[1][];
                            sectionData[0] = DataMethods.readDataSection(multiCaff.path, multiCaff.dnbws[multiCaff.getDNBWIndexByName(soundName)].offs, multiCaff.dnbws[multiCaff.getDNBWIndexByName(soundName)].len);
                            ModdingTab moddingTab = new ModdingInfo.sound(soundName, multiCaff, sectionData);
                            addTab(moddingTab, moddingTab.getModdingTabPage());
                        }
                        break;
                    }
                case MouseButtons.Right:
                    {
                        if (tabName == "DNBW")
                        {
                            MenuItem choice1 = new MenuItem("Extract All .xwb");
                            choice1.Click += new EventHandler(DNBWcontextMenu_choice1_Click);
                            ContextMenu DNBWcontextMenu = new ContextMenu();
                            DNBWcontextMenu.MenuItems.Add(choice1);
                            DNBWcontextMenu.Show(Treeview_tags, e.Location);
                        }
                        break;
                    }
            }
        }

        void DNBWcontextMenu_choice1_Click(object sender, EventArgs e)
        {
            string saveDir = "";
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                saveDir = folderBrowserDialog1.SelectedPath;
                for (int i = 0; i < multiCaff.dnbws.Count; i++)
                {
                    File.WriteAllBytes(saveDir + @"\" + multiCaff.dnbws[i].name + ".xwb", DataMethods.readDataSection(multiCaff.path, multiCaff.dnbws[i].offs, multiCaff.dnbws[i].len));
                }

                MessageBox.Show(".xwb Files Extracted Successfully :)" + saveDir);
            }

            else
            {

            }
        }

        private void addTab(object tab, TabPage tabpage)
        {
            moddingpages.Add(tab);
            moddingTabControl.TabPages.Add(tabpage);
            moddingTabControl.SelectTab(moddingTabControl.TabCount - 1);
        }

        public abstract void design();

        private void tabControl_moddingPages_MouseDown(object sender, MouseEventArgs e) //Close Modding Page
        {
            for (int i = 0; i < this.moddingTabControl.TabPages.Count; i++)
            {
                Rectangle r = moddingTabControl.GetTabRect(i);
                //Getting the position of the "x" mark.
                Rectangle closeButton = new Rectangle(r.Right - 15, r.Top + 4, 9, 7);
                if (closeButton.Contains(e.Location))
                {
                    this.moddingTabControl.TabPages.RemoveAt(i);
                    moddingpages.Remove(moddingpages[i]);
                    switch (type) //Refresh Data When ModdingTab Closed
                    {
                        case 0:
                            {
                                caff = new CAFF(caff.path);
                                break;
                            }
                        case 1:
                            {
                                multiCaff = new MULTICAFF(multiCaff.path);
                                break;
                            }
                    }

                    break;
                }
            }
        }

        void moddingTabControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu cm = new ContextMenu();
                MenuItem cms = new MenuItem();
                cm.MenuItems.Add(cms);
                cms.Text = "Close All";
                cms.Click += moddingTabControl_CloseAll_Click;
                cm.Show(moddingTabControl, e.Location);
            }
        }
        void moddingTabControl_CloseAll_Click(object sender, EventArgs e)
        {
            moddingTabControl.TabPages.Clear();
        }
    }
}
