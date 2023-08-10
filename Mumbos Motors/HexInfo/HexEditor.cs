using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace Mumbos_Motors.HexInfo
{
    public class HexEditor
    {
        public string path;
        private CAFF caff;
        private MULTICAFF multiCaff;
        private int caffIndex;
        public TabPage[] sectionPages;
        public RichTextBox[][] HexTextBoxes;
        public HexInfo.InfoPanel[] infoPanel;
        public string symbol;
        public int symbolID;
        public int numberOfSections;
        public int sectionSize;
        public int type;
        public byte[][] sectionData;
        string[] positivity = { "-", "+" };
        public bool hexTextError;
        public bool isTexture = false;
        public int bytesPerRow = 16;
        private int spacing = 10;
        private int height = 460;
        private Font defaultFont = new Font("Courier New", 8.25f, FontStyle.Bold);

        public HexEditor(byte[][] sectionData) //MISC DATA
        {
            type = 3;
            this.sectionData = sectionData;
            numberOfSections = sectionData.Length;
            coreDesign();
        }

        public HexEditor(string path, byte[][] sectionData, string symbol) //FILE
        {
            type = 0;
            this.path = path;
            this.sectionData = sectionData;
            this.symbol = symbol;
            isTexture = symbol.Contains("aid_texture");
            numberOfSections = sectionData.Length;

            coreDesign();
        }

        public HexEditor(CAFF caff, int symbolID) //CAFF
        {
            type = 1;
            this.caff = caff;
            this.symbolID = symbolID;
            symbol = caff.getSymbols()[symbolID];
            numberOfSections = caff.numSectionsSymbolID(symbolID);
            readSectionsData();
            isTexture = caff.getSymbols()[symbolID].Contains("aid_texture");
            //numberOfSections = caff.numSectionsSymbolID(symbolID);

            coreDesign();
        }

        public HexEditor(MULTICAFF multiCaff, int caffIndex, int symbolID) //MULTICAFF
        {
            type = 2;
            this.multiCaff = multiCaff;
            this.caffIndex = caffIndex;
            this.symbolID = symbolID;
            symbol = multiCaff.caffs[caffIndex].getSymbols()[symbolID];
            sectionData = new byte[numberOfSections][];
            readSectionsDataMulti();
            isTexture = multiCaff.caffs[caffIndex].getSymbols()[symbolID].Contains("aid_texture");
            numberOfSections = multiCaff.caffs[caffIndex].numSectionsSymbolID(symbolID);

            coreDesign();
        }

        public void coreDesign()
        {
            hexTextError = false;

            // #constructor#
            sectionPages = new TabPage[numberOfSections];
            for (int i = 0; i < sectionPages.Length; i++) //include sections as pages
            {
                sectionPages[i] = new TabPage();
                switch (type)
                {
                    case 0:
                        {
                            break;
                        }
                    case 1:
                        {
                            sectionPages[i].Text = caff.sections[i].name;
                            break;
                        }
                    case 2:
                        {
                            sectionPages[i].Text = multiCaff.caffs[caffIndex].sections[i].name;
                            break;
                        }
                    default:
                        {
                            sectionPages[i].Text = "data " + (i + 1);
                            break;
                        }
                }
                
                sectionPages[i].BackColor = Color.FromArgb(255 - i * 10, 255 - i * 10, 255 - i * 10);
            }

            HexTextBoxes = new RichTextBox[numberOfSections][];
            infoPanel = new HexInfo.InfoPanel[numberOfSections];
            for (int i = 0; i < numberOfSections; i++)
            {
                HexTextBoxes[i] = new RichTextBox[2];
                //HEX
                HexTextBoxes[i][0] = new RichTextBox();
                HexTextBoxes[i][0].Name = i + "";
                HexTextBoxes[i][0].Font = defaultFont;
                HexTextBoxes[i][0].Location = new Point(spacing, spacing);
                HexTextBoxes[i][0].TextChanged += new EventHandler(hexTextBox_type);
                HexTextBoxes[i][0].KeyPress += new KeyPressEventHandler(hexTexBox_keyPressed);
                HexTextBoxes[i][0].Width = 355;
                HexTextBoxes[i][0].Height = height;

                //ASCII
                HexTextBoxes[i][1] = new RichTextBox();
                HexTextBoxes[i][1].Font = defaultFont;
                HexTextBoxes[i][1].Location = new Point(HexTextBoxes[i][0].Location.X + HexTextBoxes[i][0].Width, spacing);
                HexTextBoxes[i][1].Width = 150;
                HexTextBoxes[i][1].Height = height;


                //InfoPanel
                infoPanel[i] = new InfoPanel(new Point(HexTextBoxes[i][1].Location.X + HexTextBoxes[i][1].Width, spacing), height);
                infoPanel[i].sectionLen.Text = "Length: 0x" + sectionData[i].Length.ToString("X");
                switch (type)
                {
                    case 0:
                        {
                            break;
                        }
                    case 1:
                        {
                            infoPanel[i].offs.Text = "Offset: 0x" + caff.getSectionOffsetSymbolID(symbolID, i).ToString("X");
                            infoPanel[i].fileInfoOffs.Text = "FileInfo Offs: 0x" + caff.getFileInfoOffs(symbolID, i).ToString("X");
                            break;
                        }
                    case 2:
                        {
                            infoPanel[i].offs.Text = "Offset: 0x" + multiCaff.caffs[caffIndex].getSectionOffsetSymbolID(symbolID, i).ToString("X");
                            infoPanel[i].fileInfoOffs.Text = "FileInfo Offs: 0x" + multiCaff.caffs[caffIndex].getFileInfoOffs(symbolID, i).ToString("X");
                            infoPanel[i].newlabel().Text = "MultiOffs: 0x" + (multiCaff.caffs[caffIndex].address + multiCaff.caffs[caffIndex].getSectionOffsetSymbolID(symbolID, i)).ToString("X");
                            break;
                        }
                    default:
                        {
                            infoPanel[i].offs.Text = "Offset: 0x";
                            infoPanel[i].fileInfoOffs.Text = "FileInfo Offs: 0x";
                            break;
                        }
                }

                sectionPages[i].Controls.Add(infoPanel[i].Background);
                sectionPages[i].Controls.Add(HexTextBoxes[i][0]);
                sectionPages[i].Controls.Add(HexTextBoxes[i][1]);
            }
        }

        public void readSectionsData()
        {
            sectionData = caff.readSectionsData(symbolID);
        }
        public void readSectionsDataMulti()
        {
            sectionData = multiCaff.readSectionsCAFF(caffIndex, symbolID);
        }
        public void readSectionData(int section)
        {
            sectionData[section] = caff.readSectionData(symbolID, section);
        }

        public void updateHexTextBox(int Section)
        {
            caff.readCAFFData();
            readSectionsData();
            if (getSectionSize(Section) <= 0x15000)
            {
                HexTextBoxes[Section][0].Text = DataMethods.dataToHexString(sectionData[Section], bytesPerRow);
                HexTextBoxes[Section][1].Text = DataMethods.dataToASCIIString(sectionData[Section], bytesPerRow);
            }
            else
            {
                MessageBox.Show("One or more sections were too large to print. Please do not use hex editor tabs");
            }
        }
        public void updateHexTextBox_partial(int section)
        {
            //readDataSection(Section);
            if (getSectionSize(section) <= 0x15000)
            {
                HexTextBoxes[section][0].Text = DataMethods.dataToHexString(sectionData[section], bytesPerRow);
                HexTextBoxes[section][1].Text = DataMethods.dataToASCIIString(sectionData[section], bytesPerRow);
            }
            else
            {
                MessageBox.Show("One or more sections were too large to print. Please do not use one or more hex editor tabs");
            }
        }

        public void Save()
        {
            bool isSaving = true;
            if (!hexTextError)
            {
                switch (type) //Determine if saving a file, caff, or multicaff
                {
                    case 0:
                        {
                            
                            int length = 0;
                            for (int i = 0; i < sectionData.Length; i++)
                            {
                                length += sectionData[i].Length;
                            }
                            byte[] toSave = new byte[length];
                            int k = 0;
                            for (int i = 0; i < sectionData.Length; i++)
                            {
                                for (int j = 0; j < sectionData[i].Length; j++)
                                {
                                    toSave[k] = sectionData[i][j];
                                    k++;
                                }
                            }
                            File.WriteAllBytes(path, toSave);
                            break;
                        }
                    case 1:
                        {
                            //check lengths for optional resizing
                            for (int i = 0; i < sectionData.Length; i++)
                            {
                                int newLength = sectionData[i].Length;
                                int oldLength = caff.readSectionData(symbolID, i).Length;
                                if (newLength == oldLength)
                                {
                                    caff.writeSectionData(symbolID, sectionData[i], i);
                                }
                                else
                                {
                                    int change = newLength - oldLength;
                                    DialogResult dialogResult = MessageBox.Show("Would you like to resize Section " + (i + 1) + " with " + positivity[Convert.ToInt32(change > 0)] + "0x" + Math.Abs(change).ToString("X") + "?", Form1.getTitle(), MessageBoxButtons.YesNo);
                                    isSaving = dialogResult == DialogResult.Yes;
                                    if (isSaving)
                                    {
                                        caff.writeSectionDataNew(symbolID, sectionData[i], i, change);
                                        for (int j = 0; i < sectionData.Length; i++)
                                        {
                                            updateHexTextBox(j);
                                        }
                                    }
                                }
                            }
                            if (isSaving)
                            {
                                SaySaved();
                            }
                            break;
                        }
                    case 2:
                        {
                            for (int i = 0; i < sectionData.Length; i++)
                            {
                                DataMethods.writeDataSection(multiCaff.path, multiCaff.caffs[caffIndex].address + multiCaff.caffs[caffIndex].getSectionOffsetSymbolID(symbolID, i), sectionData[i].Length, sectionData[i]);
                            }
                            SaySaved();
                            break;
                        }
                    case 3:
                        {
                            break;
                        }
                }
            }
            else
            {
                MessageBox.Show("There was a problem reading one or more sections in the hex editor. Please fix it.");
            }
        }

        public RichTextBox[][] getHexTextBoxes()
        {
            return HexTextBoxes;
        }

        public void getSectionDataFromTextBoxes(int section)
        {
            try //Test If Hex Is Formated Properly
            {
                hexTextError = false;
                byte[] Section = new byte[sectionData[section].Length];
                byte[][] hexByteRows;

                string[] hexStringRows = HexTextBoxes[section][0].Text.Split('\n');
                //MessageBox.Show(hexStringRows[i]);
                hexByteRows = new byte[hexStringRows.Length][];
                for (int j = 0; j < hexStringRows.Length; j++)
                {
                    string[] byteRow = hexStringRows[j].Split(' ');
                    //MessageBox.Show(hexStringRows[j]);
                    hexByteRows[j] = new byte[byteRow.Length];
                    //MessageBox.Show(hexByteRows[j].Length + "\n first and last char: " + byteRow[0] + " : " + byteRow[byteRow.Length - 2]);
                    for (int g = 0; g < hexByteRows[j].Length; g++)
                    {
                        hexByteRows[j][g] = Convert.ToByte(byteRow[g], 16);
                    }
                }

                //length test
                int oldLen = 0;
                switch (type)
                {
                    case 1:
                        {
                            oldLen = caff.readSectionData(symbolID, section).Length;
                            break;
                        }
                    case 2:
                        {
                            oldLen = multiCaff.caffs[caffIndex].readSectionData(symbolID, section).Length;
                            break;
                        }
                }

                int newLen = 0;
                for (int i = 0; i < hexByteRows.Length; i++)
                {
                    newLen += hexByteRows[i].Length;
                }

                int change = newLen - oldLen;
                sectionData[section] = new byte[oldLen + change];

                if (change == 0)
                {
                    infoPanel[section].error.Text = "Length: OK";
                }
                //if length is different
                else
                {
                    infoPanel[section].error.Text = "Length: " + positivity[Convert.ToInt32(change > 0)] + "0x" + Math.Abs(change).ToString("X");
                }
                //write hexText[] to sectionData[]
                for (int j = 0; j < hexByteRows.Length; j++)
                {
                    for (int g = 0; g < hexByteRows[j].Length; g++)
                    {
                        sectionData[section][(j * bytesPerRow) + g] = hexByteRows[j][g];
                    }
                }
                HexTextBoxes[section][1].Text = DataMethods.dataToASCIIString(sectionData[section], bytesPerRow);
            }
            catch
            {
                hexTextError = true;
                //MessageBox.Show(Section + "");
                infoPanel[section].error.Text = "Invalid Byte Text";
                section = numberOfSections;
            }
        }

        public int getSectionSize(int Section)
        {
            return sectionData[Section].Length;
        }

        void hexTextBox_type(object sender, EventArgs e)
        {
            RichTextBox hexBox = sender as RichTextBox;
            int sectionID = Convert.ToInt32(hexBox.Name);
            getSectionDataFromTextBoxes(sectionID);
        }

        void hexTexBox_keyPressed(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f') || (c == ' ') || (c == '\n')))
            {
                e.Handled = true;
            }
        }

        public static void SaySaved()
        {
            MessageBoxButtons okbutton = MessageBoxButtons.OK;
            MessageBox.Show("The meta data has been saved back to the original file.", "Mumbo's Motars", okbutton);
        }
    }
}
