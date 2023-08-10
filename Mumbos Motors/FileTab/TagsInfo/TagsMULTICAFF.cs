using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Mumbos_Motors.FileTab.TagsInfo
{
    class TagsMULTICAFF : TagsInfo.Tags
    {
        public TagsMULTICAFF(MULTICAFF multiCAFF)
        {
            type = 1;
            multiCaff = multiCAFF;
            design();
        }

        public override void design()
        {
            buildTreeView();
            searchBar.TextChanged += searchBar_Type;
        }

        void searchBar_Type(object sender, EventArgs e)
        {
            buildTreeView(searchBar.Text);
        }

        public void buildTreeView(string search = "")
        {
            Treeview_tags.Nodes.Clear();

            if (multiCaff.caffs.Count > 0)
            {
                Treeview_tags.Nodes.Add("CAFF");
                List<string> allSymbols = new List<string>();
                for (int i = 0; i < multiCaff.caffs.Count; i++)
                {
                    for (int j = 0; j < multiCaff.caffs[i].getSymbols().Length; j++)
                    {
                        if (multiCaff.caffs[i].getSymbols()[j].IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            allSymbols.Add(multiCaff.caffs[i].getSymbols()[j]);
                        }
                    }
                }

                string[] symbols = DataMethods.listToArray(allSymbols);
                string[] catagories = DataMethods.getAllTagCatagories(symbols);
                string[][] orderedTags = DataMethods.orderTags(catagories, symbols);

                //string test = "";
                //for (int i = 0; i < catagories.Length; i++)
                //{
                //    test += catagories[i] + "\n";
                //}
                //File.WriteAllText(@"C:\Users\Alex Weight\Desktop\fart.txt", test);

                for (int i = 0; i < orderedTags.Length; i++)
                {
                    Treeview_tags.Nodes[0].Nodes.Add(catagories[i]);
                    for (int j = 0; j < orderedTags[i].Length; j++)
                    {
                        Treeview_tags.Nodes[0].Nodes[i].Nodes.Add(orderedTags[i][j]);
                    }
                }
                if (Treeview_tags.Nodes[Treeview_tags.Nodes.Count - 1].Nodes.Count == 0)
                {
                    Treeview_tags.Nodes.Remove(Treeview_tags.Nodes[Treeview_tags.Nodes.Count - 1]);
                }
            }
            if (multiCaff.dnbws.Count > 0)
            {
                Treeview_tags.Nodes.Add("DNBW");

                for (int i = 0; i < multiCaff.dnbwNames.Length; i++)
                {
                    if ((multiCaff.dnbwNames[i] + ".xwb").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Treeview_tags.Nodes[Treeview_tags.Nodes.Count - 1].Nodes.Add(multiCaff.dnbwNames[i] + ".xwb");
                    }
                }
                if (Treeview_tags.Nodes[Treeview_tags.Nodes.Count - 1].Nodes.Count == 0)
                {
                    Treeview_tags.Nodes.Remove(Treeview_tags.Nodes[Treeview_tags.Nodes.Count - 1]);
                }
            }
        }
    }
}
