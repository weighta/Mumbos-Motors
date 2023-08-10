using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Mumbos_Motors.FileTab
{
    class TagsCAFF : TagsInfo.Tags
    {
        public CAFF Caff;

        public TagsCAFF(CAFF Caff)
        {
            type = 0;
            this.Caff = Caff;
            caff = Caff;
            design();
        }

        public override void design()
        {
            buildTreeView();
            searchBar.TextChanged += new EventHandler(searchbar_type);
        }

        private void buildTreeView()
        {
            string[] symbols = caff.getSymbols();
            caff.setTagCatagories(DataMethods.getAllTagCatagories(symbols));
            caff.setOrderedTags(DataMethods.orderTags(caff.getTagCatagories(), symbols));

            Treeview_tags.Nodes.Clear();
            for (int i = 0; i < caff.getTagCatagories().Length; i++)//Add parent nodes
            {
                Treeview_tags.Nodes.Add(caff.getTagCatagories()[i]);
            }
            for (int i = 0; i < caff.getOrderedTags().Length; i++)//Add child nodes
            {
                for (int h = 0; h < caff.getOrderedTags()[i].Length; h++)
                {
                    Treeview_tags.Nodes[i].Nodes.Add(caff.getOrderedTags()[i][h]);
                }
            }
        }

        private void buildTreeViewNodes(string search)
        {
            string[] newsymbols = DataMethods.getStringsBySearch(caff.getSymbols(), search);

            caff.setTagCatagories(DataMethods.getAllTagCatagories(newsymbols));
            caff.setOrderedTags(DataMethods.orderTags(caff.getTagCatagories(), newsymbols));

            Treeview_tags.Nodes.Clear();
            for (int i = 0; i < caff.getTagCatagories().Length; i++)//Add parent nodes
            {
                Treeview_tags.Nodes.Add(caff.getTagCatagories()[i]);
            }
            for (int i = 0; i < caff.getOrderedTags().Length; i++)//Add child nodes
            {
                for (int h = 0; h < caff.getOrderedTags()[i].Length; h++)
                {
                    Treeview_tags.Nodes[i].Nodes.Add(caff.getOrderedTags()[i][h]);
                }
            }
        }

        void searchbar_type(object sender, EventArgs e)
        {
            //Get the label clicked
            TextBox bar = sender as TextBox;
            buildTreeViewNodes(bar.Text);
            //DataMethods.SetClipboard(lbl.Text);
        }
    }
}
