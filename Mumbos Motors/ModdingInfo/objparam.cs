using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mumbos_Motors
{
    class objparam : ModdingTab
    {
        string nodeTitle = "Object Parameters";
        string catagory = "aid_objparams_banjox";
        string subCatagory = "default";
        public objparam(CAFF caff, int fileID) : base(caff, fileID)
        {
            buildMetaPage();
        }

        public objparam(MULTICAFF multiCaff, int caffIndex, int symbolID) : base(multiCaff, caffIndex, symbolID)
        {
            buildMetaPage();
        }

        public override void buildMetaPage()
        {
            MetaBlock_Text("Data Size: ", 1, 0x0, 0x2, 1);
            determineSubCatagory();
            if (subCatagory == "actor")
            {
                Actor();
            }
            else if (subCatagory == "actorbody")
            {
                Actorbody();
            }
            else if (subCatagory == "actormind")
            {

            }
            else if (subCatagory == "actorstate")
            {

            }
            else if (subCatagory == "actorstrategy")
            {

            }
            else if (subCatagory == "avatar")
            {

            }
            else if (subCatagory == "collectable")
            {
                Collectable();
            }
            else if (subCatagory == "default")
            {

            }
            else if (subCatagory == "props")
            {
                Props();
            }
            else if (subCatagory == "vehicleblock")
            {
                Vehicleblock();
            }
            else
            {

            }
        }

        public void Actor()
        {
            nodeTitle = "Actor";
            createNode(nodeTitle, 1);

            MetaBlock_Combo("Model", catagory, subCatagory, 1, 0xC0, 0x4, nodeTitle, 0);
            MetaBlock_Combo("Animations", catagory, subCatagory, 1, 0xD0, 0x4, nodeTitle, 0);
            MetaBlock_Combo("Callout", catagory, subCatagory, 1, 0xD4, 0x4, nodeTitle, 0);
        }
        public void Actorbody()
        {
            nodeTitle = "Actorbody";
            createNode(nodeTitle, 1);
            MetaBlock_Combo("ID:", catagory, subCatagory, 1, 0x98, 0x4, nodeTitle, 0);
        }
        public void Collectable()
        {
            nodeTitle = "Collectable";
            createNode(nodeTitle, 1);
            MetaBlock_Text("Amount", 1, 0x1C8, 0x4, 0);
        }
        public void Props()
        {
            nodeTitle = "Prop";
            createNode(nodeTitle, 1);
            MetaBlock_Combo("Model", catagory, subCatagory, 1, 0xC0, 0x4, nodeTitle, 0);
            MetaBlock_Combo("Animations", catagory, subCatagory, 1, 0xD0, 0x4, nodeTitle, 0);
            MetaBlock_Combo("Callout", catagory, subCatagory, 1, 0xD4, 0x4, nodeTitle, 0);
        }
        public void Vehicleblock()
        {
            nodeTitle = "Vehicleblock";
            createNode(nodeTitle, 1);
            MetaBlock_Combo("Model", catagory, subCatagory, 1, 0x124, 0x4, nodeTitle, 0);
            MetaBLock_String("Color", 1, 0x130, 0x20, nodeTitle, 0);
        }
        public void determineSubCatagory()
        {
            try
            {
                subCatagory = DataMethods.readString(caff.getSymbols()[symbolID], 21);
            }
            catch
            {

            }
        }
    }
}