using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mumbos_Motors
{
    struct VehiclePart
    {
        public int[] pos;
        public int mod;
        public int ident;
        public float[] rot;
        public int[] RGBA;
    }
    public class vehicle : ModdingTab
    {
        #region partlist
        string[] partList = {
            ".ë7. - 1FEB371C - Olympic Torch",
            ".»Pb - 1FBB5062 - Sphere A",
            ".M‡^ - 1F4D875E - Sphere B",
            ".¼`ô - 1FBC60F4 - Sphere C",
            ".b´. - 1F62B405 - Grunty's Seat",
            ".›   - 1F941C9B - Thomas's Seat",
            ".H²x - 1F48B278 - Thomas's Seat 2",
            ".ªrq - 1FAA7271 - Pikelet's seat",
            ".fqÕ - 1F6671D5 - Large Seat AI",
            ".êDJ - 1FEA444A - Standard Seat",
            ".z‰  - 1F7A892D - Standard Seat Upside Down",
            ".Ñ™¦ - 1FD199A6 - Small Taxi Seat",
            ".ÿ;E - 1FFF3B45 - Large Taxi Seat",
            "..>Ô - 1F123ED4 - Scuba Seat",
            ".ð0. - 1FF03006 - Super Seat",
            ".Ñ˜N - 1FD1984E - Strong Seat",
            "..Óš - 1F09D39A - Standard Wheel",
            ".ñ¸t - 1FF1B874 - High Grip Wheel",
            "._.˜ - 1F5F9098 - Super Wheel",
            ".•Ð‡ - 1F95D087 - Moster Wheel",
            ".eŽÍ - 1F658ECD - Small Engine",
            ".ckr - 1F636B72 - Medium Engine",
            ".¼n! - 1FBC6E21 - Medium Engine AI",
            ".È”F - 1FC89446 - Large Engine",
            ".(y) - 1F287929 - Super Engine",
            ".+ { - 1F2B227B - Sail", 
            ". q. - 1F207106 - Small Jet",
            "..²« - 1F0EB2AB - Large Jet",
            ".„ý. - 1F84FD7F - Small Fuel",
            ".ü.ø - 1FFC8DF8 - Medium Fuel",
            ".®,w - 1FAE2C77 - Large Fuel",
            ".ÿÏ_ - 1FFFCF5F - Super Fuel",
            "..z. - 1F127A1B - Tray",
            ".)®b - 1F29AE62 - Large Tray",
            ". 9n - 1FA0396E - Box",
            ".·˜ý - 1FB798FD - Large Box",
            ".…ÔÎ - 1F85D4CE - Small Ammo",
            ".ü.ø - 1FE80FCB - Medium Ammo",
            ".¯.Æ - 1FAF05C6 - Large Ammo",
            ".þæî - 1FFEE6EE - Super Ammo",
            ".Kµá - 1F4BB5E1 - Light Cube",
            ".Ìta - 1FCC7461 - Light Wedge",
            ".óæJ - 1FF3E64A - Light Corner",
            ".ÕF¸ - 1FD546B8 - Light Pole",
            ".Ö.. - 1FD61511 - Light L Pole",
            "..Ÿ6 - 1F0A9F36 - Light T Pole",
            ".ó`Ö - 1FF360D6 - Light Panel",
            ".AxÉ - 1F4178C9 - Light L Panel",
            ".3.( - 1F331128 - Light T Panel",
            ".„.E - 1F841B45 - Heavy Cube",
            ".ÕÜ> - 1FD5DC3E - Heavy Wedge",
            ".ö³‡ - 1FF6B387 - Heavy Corner",
            "..è. - 1F1AE81C - Heavy Pole",
            "..4Ã - 1F1C34C3 - Heavy L Pole",
            "..7i - 1F133769 - Heavy T Pole",
            ".êÈ‰ - 1FEAC889 - Heavy Panel",
            ".O.. - 1F4F0110 - Heavy L Panel",
            ".6Då - 1F3644E5 - Heavy T Panel",
            ".êd. - 1FEA640E - Super Cube",
            ".$*Y - 1F242A59 - Super Wedge",
            ".Á¶Š - 1FC1B68A - Super Corner",
            ".t—W - 1F749757 - Super Pole",
            ".Qã. - 1F51E31C - Super L Pole",
            ".âÁ. - 1FE2C10E - Super T Pole",
            "..>î - 1F1B3EEE - Super Panel",
            ".µƒ‚ - 1FB58382 - Super L Panel",
            "..Aè - 1F0141E8 - Super T Panel",
            ".*â¸ - 1F2AE2B8 - Small Propeller",
            "..!. - 1F042115 - Large Propeller",
            ".Àqð - 1FC071F0 - Large Folding Propeller",
            ".üu. - 1FFC7505 - Standard Wings",
            ".,.› - 1F2CAD9B - Folding Wings",
            ".... - 1F8D1F16 - Sinker",
            "..—f - 1F079766 - Balloon",
            ".—Ž. - 1F978E81 - Floater",
            ".÷»l - 1FF7BB6C - Air Cushion",
            ".ÑiŒ - 1FD1698C - Aerial",
            ".t.. - 1F74119D - Gyroscope",
            ".rÔ— - 1F72D497 - Spotlight",
            "...Ç - 1F0002C7 - Spec O Spy",
            ".X40 - 1F583430 - Self Destruct",
            ".AªÀ - 1F41AAC0 - Liquid Squirter",
            ".µk” - 1FB56B94 - Spoiler",
            ".nü4 - 1F6EFC34 - Suck N Blow",
            ".¾WU - 1FBE5755 - Sticky Ball",
            ".²pB - 1FB27042 - Vacuum",
            ".….© - 1F8517A9 - Spring",
            ".þÀµ - 1FFEC0B5 - Tow Bar",
            "..Çw - 1F10C777 - Detacher",
            ".;¢; - 1F3BA23B - Ejector Seat",
            ".NhÇ - 1F4E68C7 - Robo Fix",
            ".˜,B - 1F982C42 - Replenisher",
            "..‘b - 1F029162 - Horn",
            ".c½æ - 1F63BDE6 - Chameleon",
            "..òt - 1F8DF274 - Bumper",
            ".+ò. - 1F2BF215 - Armor",
            ".•7@ - 1F953740 - Energy Shield",
            ".KÆ. - 1F4BC61E - Fulgore's Fist",
            ".7»~ - 1F37BB7E - Boot in a box",
            ".[.< - 1F5B8D3C - Spike",
            ".Tç. - 1F54E704 - Freezeezy",
            ".‹.¸ - 1F8B1BB8 - Egg Turret",
            ".Pm. - 1F506D0F - Weldar's Breath",
            ".ï . - 1FEF20AD - Egg Gun",
            ".›øµ - 1F9BF8B5 - Grenade Gun",
            ".Å—û - 1FC597FB - Rust Bin",
            ".7.. - 1F37131C - Grenade Turret",
            ".1ï« - 1F31EFAB - Torpedo",
            ".|þô - 1F7CFEF4 - Laser",
            ".>Ž. - 1F3E8E01 - Mumbo Bombo",
            ".`<Ã - 1F603CC3 - Clockwork Kaz",
            ".]–. - 1F5D961C - Citrus Slick",
            ".1ÊL - 1F31CA4C - EMP",
            ".XZQ - 1F585A51 - Crusin' Light",
            "..•ç - 1F2E95E7 - Plant Pot",
            ".djâ - 1F646AE2 - Spirit Of Pants",
            "...H - 1F130448 - Windscreen",
            ".»¯* - 1FBBAF2A - Flag (SNS)",
            "..Yý - 1F0C59FD - Mole On A Pole (SNS)",
            ".¾©† - 1FBEA986 - Fluffy Dice (SNS)",
            ".ã8Ý - 1FE338DD - Goldfish (SNS)",
            ".ÆÄ. - 1FC6C417 - Beacon (SNS)",
            "..{É - 1F817BC9 - Disco Ball (SNS)",
            ".Á€¯ - 1FC180AF - Googly Eyes (SNS)",
            "..d¯ - 1F1064AF - Mirror",
            "..×< - 1F0FD73C - Tag Plate",
            ".˜ü. - 1F98FC81 - Stereo",
            ".—Ã' - 1F97C327 - Papery Pal"
            };
        #endregion

        int[] partIdents;
        string[] partNames;
        VehiclePart[] parts;
        int section = 0;
        int nameStart = 0x20;
        string name;
        int dataStart = 0x7C;
        int dataStartSave = 0x84;
        int numParts;
        int numPartsSave;
        int partSize = 0x24;
        public vehicle(CAFF caff, int fileID) : base(caff, fileID)
        {
            buildMetaPage();
        }

        public vehicle(MULTICAFF multiCaff, int caffIndex, int symbolID) : base(multiCaff, caffIndex, symbolID)
        {
            buildMetaPage();
        }

        public override void buildMetaPage()
        {
            readPartStore();
            readVehicle();
            string nodeName = "Vehicle";
            createNode(nodeName, numParts);

            int p = dataStart;
            for (int i = 0; i < parts.Length; i++)
            {
                int ident = DataMethods.readInt32(hxd.sectionData[section], p + 0x8);
                MetaBlock_Text("LEFT dist (X):", 1, p + 0x1, 0x1, 0, nodeName, i);
                MetaBlock_Text("FRONT dist (Y):", 1, p + 0x2, 0x1, 0, nodeName, i);
                MetaBlock_Text("TOP dist (Z):", 1, p + 0x3, 0x1, 0, nodeName, i);
                MetaBlock_Text("Modifier:", 1, p + 0x5, 0x2, 0, nodeName, i);
                MetaBlock_Combo_Custom("Ident:", 1, p + 0x8, 0x4, nodeName, i);
                metaTab.metaTagRefs_Custom[metaTab.metaTagRefs_Custom.Count - 1].comboBox = createIdentList(metaTab.metaTagRefs_Custom[metaTab.metaTagRefs_Custom.Count - 1].comboBox, ident, i);
                metaTab.metaTagRefs_Custom[metaTab.metaTagRefs_Custom.Count - 1].comboBox.DropDownClosed += new EventHandler(comboBoxIdents_update);
                metaTab.metaTagRefs_Custom[metaTab.metaTagRefs_Custom.Count - 1].comboBox.SelectedIndexChanged += new EventHandler(comboBoxIdents_update);
                MetaBlock_Text("Yaw:", 1, p + 0xC, 0x4, 2, nodeName, i);
                MetaBlock_Text("Pitch:", 1, p + 0x10, 0x4, 2, nodeName, i);
                MetaBlock_Text("Roll:", 1, p + 0x14, 0x4, 2, nodeName, i);
                MetaBlock_Text("Red:", 1, p + 0x18, 0x1, 0, nodeName, i);
                MetaBlock_Text("Green:", 1, p + 0x19, 0x1, 0, nodeName, i);
                MetaBlock_Text("Blue:", 1, p + 0x20, 0x1, 0, nodeName, i);
                MetaBlock_Text("Alpha:", 1, p + 0x21, 0x1, 0, nodeName, i);
                p += partSize;
            }
            bottomToolBar.addButton("Extract Vehicle");
            bottomToolBar.buttons[bottomToolBar.buttons.Count - 1].Click += new EventHandler(export_vehicle);
            bottomToolBar.addButton("Import Vehicle");
            bottomToolBar.buttons[bottomToolBar.buttons.Count - 1].Click += new EventHandler(import_vehicle);
        }

        public void readVehicle()
        {
            name = DataMethods.readString(hxd.sectionData[0], 0x20);
            numParts = DataMethods.readInt16(hxd.sectionData[section], 0);
            //MessageBox.Show(numParts + "\n" + hxd.sectionData[section][0] + " " + hxd.sectionData[section][1]);
            parts = new VehiclePart[numParts];
            int p = dataStart;
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i].pos = new int[3];
                parts[i].rot = new float[3];
                parts[i].RGBA = new int[4];
                parts[i].pos[0] = hxd.sectionData[section][p];
                parts[i].pos[1] = hxd.sectionData[section][p + 0x1];
                parts[i].pos[2] = hxd.sectionData[section][p + 0x2];
                parts[i].mod = DataMethods.readInt16(hxd.sectionData[section], p + 0x5);
                parts[i].ident = DataMethods.readInt32(hxd.sectionData[section], p + 0x8);
                parts[i].rot[0] = DataMethods.readFloat32(hxd.sectionData[section], p + 0xC);
                parts[i].rot[1] = DataMethods.readFloat32(hxd.sectionData[section], p + 0x10);
                parts[i].rot[2] = DataMethods.readFloat32(hxd.sectionData[section], p + 0x14);
                parts[i].RGBA[0] = hxd.sectionData[section][p + 0x18]; //red
                parts[i].RGBA[1] = hxd.sectionData[section][p + 0x19]; //green
                parts[i].RGBA[2] = hxd.sectionData[section][p + 0x20]; //blue
                parts[i].RGBA[3] = hxd.sectionData[section][p + 0x21]; //alpha
                p += partSize;
            }
        }

        public ComboBox createIdentList(ComboBox box, int ident, int blockID)
        {
            box.Name = blockID + "";
            box.Items.Clear();
            for (int i = 0; i < partNames.Length; i++)
            {
                box.Items.Add(partNames[i]);
            }
            box.Text = getPartNameFromIdent(ident);
            return box;
        }

        public string getPartNameFromIdent(int ident)
        {
            for (int i = 0; i < partIdents.Length; i++)
            {
                if (partIdents[i] == ident)
                {
                    return partNames[i];
                }
            }
            return "unknown";
        }

        public int getIdentFromPartName(string name)
        {
            for (int i = 0; i < partIdents.Length; i++)
            {
                if (partNames[i] == name)
                {
                    return i;
                }
            }
            return 0;
        }

        public void readPartStore()
        {
            partIdents = new int[partList.Length];
            partNames = new string[partList.Length];
            for (int i = 0; i < partIdents.Length; i++)
            {
                string[] infos = partList[i].Split('-');
                partIdents[i] = Convert.ToInt32(infos[1].Substring(1, 8), 16);
                partNames[i] = infos[2].Substring(1);
            }
        }
        void comboBoxIdents_update(object sender, EventArgs e)
        {
            ComboBox box = sender as ComboBox;
            int index = getIdentFromPartName(box.Text);
            int blockID = Convert.ToInt32(box.Name);
            metaTab.metaTagRefs_Custom[blockID].textBox.Text = partIdents[index].ToString("X8");
        }

        void calcNumPartsSave(byte[] saveData)
        {
            numPartsSave = (saveData.Length - 0x84) / partSize;
        }

        void export_vehicle(object sender, EventArgs e)
        {
            byte[] exp = {
                0x3F, 0x9A, 0xE1, 0x48, 0x40, 0x54, 0x7A, 0xE1, 0x00, 0x00, 0x01, 0x00, 0x43, 0x3E, 0x00, 0x00,
                0x45, 0x88, 0xE2, 0x2A, 0x44, 0xC8, 0x00, 0x00, 0x45, 0x2F, 0x50, 0x01, 0x43, 0x5D, 0x8E, 0x39,
                0x00, 0x00, 0x0D, 0xD8, 0x00, 0x00, 0x00, 0x01
            };
            byte[] Exp = new byte[exp.Length + 0x5C + (parts.Length * partSize)];
            exp.CopyTo(Exp, 0);
            Exp[0x80] = 1;

            //name
            int p = 0x28;
            for (int i = nameStart; i < nameStart + name.Length; i++)
            {
                Exp = DataMethods.writeInt16(Exp, p, Convert.ToInt32(name[i - nameStart]));
                p += 2;
            }

            Exp = DataMethods.writeInt16(Exp, 0x8, numParts);
            //Copy remaining data;
            p = dataStartSave;
            for (int i = dataStart; i < hxd.sectionData[0].Length; i++)
            {
                Exp[p] = hxd.sectionData[0][i];
                p++;
            }
            DataMethods.saveFileDialog(Exp, "00000000", "Content of package");
        }
        void import_vehicle(object sender, EventArgs e)
        {
            byte[] imp = DataMethods.openFileDialog("Vehicle save content");
            try
            {
                calcNumPartsSave(imp);
                byte[] Imp = new byte[dataStart + (numPartsSave * partSize)];
                Array.Copy(hxd.sectionData[section], Imp, dataStart);

                Imp = DataMethods.writeInt16(Imp, 0, numPartsSave);
                int p = dataStartSave;
                for (int i = dataStart; i < Imp.Length; i++)
                {
                    Imp[i] = imp[p];
                    p++;
                }
                hxd.sectionData[section] = new byte[Imp.Length];
                Imp.CopyTo(hxd.sectionData[section], 0);

                hxd.updateHexTextBox_partial(section);
            }


            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
