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

    public class texture : ModdingTab
    {
        Bitmap btm;
        Random ran = new Random();
        PictureBox pbx;
        int oldwidth;
        int oldheight;
        int width;
        int height;
        int type;
        int format;
        int modifier = 1;
        int last = 0;
        byte[][] tempSectionData;
        public texture(CAFF caff, int fileID) : base(caff, fileID)
        {
            buildMetaPage();
        }

        public texture(MULTICAFF multiCaff, int caffIndex, int symbolID) : base(multiCaff, caffIndex, symbolID)
        {
            buildMetaPage();
        }

        public override void buildMetaPage()
        {
            tempSectionData = hxd.sectionData;
            width = DataMethods.readInt16(hxd.sectionData[0], 0x24);
            height = DataMethods.readInt16(hxd.sectionData[0], 0x26);
            pbx = new PictureBox();
            pbx.Location = new Point(metaTab.sideSpacing, metaTab.sideSpacing);

            solveBitmap();
            metaTab.metaSpacing += pbx.Height + metaTab.sideSpacing;
            if (pbx.Width > metaTab.Background.Width)
            {
                metaTab.Background.Width = pbx.Width + (2 * metaTab.sideSpacing);
            }

            MetaBlock_Text("Format: ", 1, 0x1B, 0x1, 1);
            MetaBlock_Text("Type: ", 1, 0x30, 0x1, 1);
            MetaBlock_Text("LOD Specifier: ", 1, 0x2C, 0x4, 1);
            MetaBlock_Text("Width: ", 1, 0x24, 0x2, 0);
            MetaBlock_Text("Height: ", 1, 0x26, 0x2, 0);

            metaTab.buildMetaFunPanel();
            buildMetaFunPanelButton("Randomize Colors");
            latestFunButton().Click += new EventHandler(RandomizeColors);
            buildMetaFunPanelButton("Checkerboard Colors");
            latestFunButton().Click += new EventHandler(CheckerboardColors);
            buildMetaFunPanelButton("10", "Amplify Colors");
            latestFunButton().Click += new EventHandler(AmplifyColors);


            bottomToolBar.addButton("Export Texture");
            bottomToolBar.buttons[bottomToolBar.buttons.Count - 1].Click += new EventHandler(export_texture);

            bottomToolBar.addButton("Export ALL Textures");
            bottomToolBar.buttons[bottomToolBar.buttons.Count - 1].Click += new EventHandler(export_textures);
        }

        /// <summary>
        /// Randomizes the colors for bkn&b's .rtx format
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RandomizeColors(object sender, EventArgs e) //section 2
        {
            for (int i = 0; i < hxd.sectionData[1].Length; i += 0x8)
            {
                //color 1
                hxd.sectionData[1][i] = (byte)ran.Next(0, 256);
                hxd.sectionData[1][i + 1] = (byte)ran.Next(0, 256);
                //color 2
                hxd.sectionData[1][i + 2] = (byte)ran.Next(0, 256);
                hxd.sectionData[1][i + 3] = (byte)ran.Next(0, 256);
            }
            metaTab.metaChanged[1] = true;
            MessageBox.Show("Color data randomized successfully. Don't forget to save it :)");
        }
        void CheckerboardColors(object sender, EventArgs e) //section 2
        {
            for (int i = 0; i < hxd.sectionData[1].Length; i += 0x8)
            {
                //layout 1
                hxd.sectionData[1][i + 4] = 0x11;
                hxd.sectionData[1][i + 5] = 0x44;
                //layout 2
                hxd.sectionData[1][i + 6] = 0x11;
                hxd.sectionData[1][i + 7] = 0x44;
            }
            metaTab.metaChanged[1] = true;
            MessageBox.Show("Color data checkerboarded successfully. Don't forget to save it :)");
        }
        void AmplifyColors(object sender, EventArgs e) //section 2
        {
            Button addButton = sender as Button;
            int add = Convert.ToInt32(metaTab.metaFunPanel.funTextBoxes[Convert.ToInt32(addButton.Name)].Text);
            for (int i = 0; i < hxd.sectionData[1].Length; i += 0x8)
            {
                //layout 1
                hxd.sectionData[1][i] = (byte)((hxd.sectionData[1][i] + add) % 256);
                hxd.sectionData[1][i + 1] = (byte)((hxd.sectionData[1][i + 1] + (add / 2)) % 256);
                //layout 2
                hxd.sectionData[1][i + 2] = (byte)((hxd.sectionData[1][i + 2] + (add / 2)) % 256);
                hxd.sectionData[1][i + 3] = (byte)((hxd.sectionData[1][i + 3] + add) % 256);
            }
            metaTab.metaChanged[1] = true;


            MessageBox.Show("Colors data amplified. Don't forget to save it :)");
        }

        void export_texture(object sender, EventArgs e)
        {
            btm.Save(DataMethods.saveFileDialogTexture(DataMethods.readTextureSymbol(hxd.symbol)) + ".png");
        }

        void export_textures(object sender, EventArgs e)
        {
            string[] symbols = DataMethods.getStringsBySearch(caff.getSymbols(), "aid_texture");
            for (int i = 0; i < symbols.Length; i++)
            {
                if (symbols[i].Contains("colour") || symbols[i].Contains("Color") || symbols[i].Contains("sky") || symbols[i].Contains("cubemaps"))
                {
                    tempSectionData = caff.readSectionsData(DataMethods.getIndexBySearch(caff.getSymbols(), symbols[i]));
                    RipRTX(DataMethods.readInt16(tempSectionData[0], 0x24), DataMethods.readInt16(tempSectionData[0], 0x26), DataMethods.readTextureSymbol(symbols[i]));
                }
            }
            tempSectionData = hxd.sectionData;
        }

        private void RipRTX(int Width, int Height, string name)
        {
            if (Width >= 512) Width /= 2;
            if (Height >= 512) Height /= 2;
            Color[][] test = new Color[0][];
            //MessageBox.Show(name);
            Bitmap btm;
            if (Width >= 128 && Height >= 128)
            {
                btm = colorToBitmap(test);
                btm.Save(@"C:\Users\Alex Weight\Desktop\textures\" + name + ".png");
            }
        }
        public void solveBitmap()
        {
            type = DataMethods.readInt(hxd.sectionData[0], 0x30, 0x1);
            format = DataMethods.readInt(hxd.sectionData[0], 0x1B, 0x1);
            bool halfed = DataMethods.readInt16(hxd.sectionData[0], 0x2C) != 0xFFFF;

            oldwidth = width;
            oldheight = height;

            if (width >= 512) width /= (int)Math.Pow(2, Convert.ToInt32(halfed));
            if (height >= 512) height /= (int)Math.Pow(2, Convert.ToInt32(halfed));


            switch (format)
            {
                case 0x52:
                    {
                        modifier = 1;
                        break;
                    }
                case 0x53:
                    {
                        modifier = 2;
                        break;
                    }
            }
            //MessageBox.Show(modifier + " (mod)");
            switch (format)
            {
                case 0x52:
                    {
                        btm = colorToBitmap(getDynamicBlocksDXT1(width, height));
                        break;
                    }
                case 0x53:
                    {
                        btm = colorToBitmap(getDynamicBlocksDXT3(width, height));
                        break;
                    }
            }


            pbx.Size = new Size(width, height);
            pbx.Visible = true;
            pbx.Image = btm;
            metaTab.Background.Controls.Add(pbx);
        }

        public Bitmap colorToBitmap(Color[][] colors)
        {
            Bitmap btm = new Bitmap(colors[0].Length, colors.Length);
            for (int i = 0; i < colors.Length; i++)
            {
                for (int j = 0; j < colors[i].Length; j++)
                {
                    btm.SetPixel(j, i, colors[i][j]);
                }
            }
            return btm;
        }

        public Color[][] getDynamicBlocksDXT3(int Width, int Height)
        {
            int p = 0;
            int newWidth = Width;
            int newHeight = Height;
            if (Width % 128 != 0) newWidth = Width + (128 - (Width % 128));
            if (Height % 128 != 0) newHeight = Height + (128 - (Height % 128));

            Color[][] start = new Color[newHeight][];
            for (int i = 0; i < start.Length; i++) start[i] = new Color[newWidth];

            for (int i = 0; i < (newWidth / 128) * (newHeight / 128); i++)
            {
                //MessageBox.Show("ColorCopy(getTile(" + p + "), " + start + ", " + ((i % (newWidth / 128)) * 128) + ", " + ((i / (newWidth / 128)) * 128) + ");");
                start = ColorCopy(getTile0x53(p), start, (i % (newWidth / 128)) * 128, (i / (newWidth / 128)) * 128);
                p += 0x4000;
            }
            Color[][] corrected = new Color[Height][];
            for (int i = 0; i < corrected.Length; i++) corrected[i] = new Color[Width];

            return ColorCopy(start, 0, 0, Width, Height, corrected, 0, 0);
        }

        public Color[][] getTile0x53(int p)
        {
            Color[][] ret = new Color[128][];
            for (int i = 0; i < 128; i++) ret[i] = new Color[128];

            for (int i = 0; i < 8; i++)
            {
                switch(i / 4)
                {
                    case 0:
                        {
                            //MessageBox.Show("Y: " + ((((i % 2) * 64) + ((i / 2) * 16))));
                            ret = ColorCopy(getPanel0x53((i * 0x800) + p), ret, 0, (((i % 2) * 64) + ((i / 2) * 16)));
                            break;
                        }
                    case 1:
                        {
                            //MessageBox.Show("Y: " + ((((i % 2) * 64) + ((i / 2) * 16))));
                            ret = ColorCopy(swapPanel(getPanel0x53((i * 0x800) + p)), ret, 0, (((i % 2) * 64) + ((i / 2) * 16)));
                            break;
                        }
                }
            }
            return ret;
        }
        public Color[][] getPanel0x53(int p)//0x800
        {
            Color[][] ret = new Color[16][];
            for (int i = 0; i < ret.Length; i++) ret[i] = new Color[128];

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    Color[][] bigTexel = getBigTexel0x53(((j * 0x40) + (i * 0x400)) + p);
                    //MessageBox.Show("X: " + (((j * 0x20) % 128) + ((j / 4) * 8)) + " Y: " + (i * 8));
                    ret = ColorCopy(bigTexel, ret, ((j * 0x20) % 128) + ((j / 4) * 8), i * 8);
                }
            }
            return ret;
        }
        public Color[][] getBigTexel0x53(int p)//0x40
        {
            Color[][] ret = new Color[8][];
            for (int i = 0; i < ret.Length; i++) ret[i] = new Color[8];
            for (int i = 0; i < 4; i++)
            {
                Color[] texel = getTexel(DataMethods.readInt16(hxd.sectionData[1], p + (i * 0x10) + 0x0), DataMethods.readInt16(hxd.sectionData[1], p + (i * 0x10) + 0x2), DataMethods.readInt16(hxd.sectionData[1], p + (i * 0x10) + 0x4), DataMethods.readInt16(hxd.sectionData[1], p + (i * 0x10) + 0x6), DataMethods.readInt16(hxd.sectionData[1], p + (i * 0x10) + 0x8), DataMethods.readInt16(hxd.sectionData[1], p + (i * 0x10) + 0xA), DataMethods.readInt16(hxd.sectionData[1], p + (i * 0x10) + 0xC), DataMethods.readInt16(hxd.sectionData[1], p + (i * 0x10) + 0xE));
                for (int j = 0; j < texel.Length; j++)
                {
                    ret[((i * 4) % 8) + (j / 4)][(j % 4) + ((i / 2) * 4)] = texel[j];
                }
            }
            //int change = (p - last);
            //if (change != (0x40))
            //{
            //    MessageBox.Show("change: " + (p.ToString("X")) + " - " + last.ToString("X") + " = " + change.ToString("X") + "\nFormat: " + format.ToString("X2"));
            //}
            //else
            //{
            //    MessageBox.Show("OK");
            //}
            //last = p;

            return ret;
        }
        public Color[] getTexel(int a1, int a2, int a3, int a4, int color1, int color2, int lay1, int lay2) //dxt3
        {
            int[] alphas = new int[] { a1, a2, a3, a4 };
            Color[] colors = { getColor565(color1), getColor565(color2) };
            Color[] c = new Color[16];

            for (int i = 0; i < 16; i++)
            {
                if (i < 8)
                {
                    c[i] = getColorLayout(i, colors[0], colors[1], lay1);
                }
                else
                {
                    c[i] = getColorLayout(i, colors[0], colors[1], lay2);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    int alpha = (int)(((15 & (alphas[i] >> (j * 4))) / 15.0) * 255);
                    c[(i * 4) + j] = Color.FromArgb(alpha, c[(i * 4) + j].R, c[(i * 4) + j].G, c[(i * 4) + j].B);
                }
            }


            return c;
        }
        public Color[][] getDynamicBlocksDXT1(int Width, int Height)
        {
            int p = 0;
            int newWidth = Width;
            int newHeight = Height;
            if (Width % 128 != 0) newWidth = Width + (128 - (Width % 128));
            if (Height % 128 != 0) newHeight = Height + (128 - (Height % 128));

            Color[][] start = new Color[newHeight][];
            for (int i = 0; i < start.Length; i++) start[i] = new Color[newWidth];
            Color[][] corrected = new Color[Height][];
            for (int i = 0; i < corrected.Length; i++) corrected[i] = new Color[Width];

            for (int i = 0; i < (newWidth / 128) * (newHeight / 128); i++)
            {
                //MessageBox.Show("ColorCopy(getTile(" + p + "), " + start + ", " + ((i % (newWidth / 128)) * 128) + ", " + ((i / (newWidth / 128)) * 128) + ");");
                start = ColorCopy(getTile(p), start, (i % (newWidth / 128)) * 128, (i / (newWidth / 128)) * 128);
                p += 0x2000;
            }

            corrected = ColorCopy(start, 0, 0, Width, Height, corrected, 0, 0);
            return corrected;
        }
        public Color[][] getTile(int p)
        {
            int duoPanelLen = 0x800 * modifier;
            Color[][][] duoPanels = new Color[4][][];

            duoPanels[0] = getDuoPanel(p);
            duoPanels[2] = getDuoPanel((p) + duoPanelLen);
            duoPanels[1] = swapPanel(getDuoPanel((p) + (duoPanelLen * 2)));
            duoPanels[3] = swapPanel(getDuoPanel((p) + (duoPanelLen * 3)));

            Color[][] tile = new Color[duoPanels.Length * 32][];
            for (int i = 0; i < tile.Length; i++) //height
            {
                tile[i] = new Color[128];
                for (int j = 0; j < duoPanels[(i / 32)][i % 32].Length; j++) //length
                {
                    tile[i][j] = duoPanels[(i / 32)][i % 32][j];
                    //if (i >= 95 && j >= 110)
                    //{
                    //    MessageBox.Show("tile[" + i + "][" + j + "] = duoPanels[" + (i / 32) + "][" + (i % 32) + "][" + j + "]");
                    //}
                }
            }
            return tile;
        } //128
        public Color[][] getDuoPanel(int p)
        {
            Color[][] duoPanel = new Color[32][];
            Color[][][] panels = new Color[2][][];
            panels[0] = getPanel(p);
            panels[1] = getPanel(p + (0x400 * modifier));

            for (int i = 0; i < duoPanel.Length; i++)
            {
                duoPanel[i] = new Color[128];
                for (int j = 0; j < panels[i / 16][i % 16].Length; j++)
                {
                    Color towrite = panels[i / 16][i % 16][j];
                    duoPanel[i][j] = towrite;
                }
            }


            return duoPanel;
        }
        public Color[][] getPanel(int p) //height, width
        {
            Color[][][] bigTexels = new Color[32][][];
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    bigTexels[((i * 16) + (((j / 2) * 4) - ((j / 8) * 14)) + (j % 2))] = getBigTexel(p);
                    p += 0x20 * modifier;
                }
            }
            Color[][] panel = new Color[16][];
            for (int i = 0; i < panel.Length; i++) //fill in the rows (downward)
            {
                panel[i] = new Color[128];
                for (int j = 0; j < 16; j++) //var for bigTexels across
                {
                    for (int k = 0; k < 8; k++) //traverse bigTexel width
                    {
                        try
                        {

                            panel[i][(j * 8) + k] = bigTexels[((i / 8) * 16) + j][k][i % 8];
                        }
                        catch
                        {
                            MessageBox.Show("panel[" + i + "][" + ((j * 8) + k) + "] = bigTexels[" + (((i / 8) * 16) + j) + "][" + k + "][" + (i + ((i / 8) * 8)) + "]");
                        }

                    }
                }
            }
            return panel;
        }
        public Color[][] getBigTexel(int p)
        {
            //MessageBox.Show("current pos: " + p.ToString("X8"));
            Color[][] bigTexel = new Color[4][];
            for (int j = 0; j < 4; j++)
            {
                bigTexel[j] = getTexel(DataMethods.readInt16(tempSectionData[1], p), DataMethods.readInt16(tempSectionData[1], p + 0x2), DataMethods.readInt16(tempSectionData[1], p + 0x4), DataMethods.readInt16(tempSectionData[1], p + 0x6));

                //int change = (p - last);
                //if (change != (0x8 * modifier))
                //{
                //    MessageBox.Show("change: " + (p.ToString("X")) + " - " + last.ToString("X") + " = " + change.ToString("X") + "\nFormat: " + format.ToString("X2"));
                //}
                //last = p;

                p += 0x8 * modifier;
            }

            Color[][] Ordered = new Color[8][];
            for (int i = 0; i < Ordered.Length; i++)
            {
                Ordered[i] = new Color[8];
            }
            for (int i = 0; i < bigTexel.Length; i++)
            {
                for (int j = 0; j < bigTexel[i].Length; j++)
                {
                    //MessageBox.Show("x: " + (((i % 2) * 4) + (j % 4)) + " y: " + ((j / 4) + ((i / 2) * 4)));
                    Ordered[((i % 2) * 4) + (j % 4)][(j / 4) + ((i / 2) * 4)] = bigTexel[i][j];
                }
            }

            return Ordered;
        }
        public Color[] getTexel(int color1, int color2, int lay1, int lay2) //dxt1
        {
            Color[] colors = { getColor565(color1), getColor565(color2) };
            Color[] c = new Color[16];

            //for (int i = 0; i < 16; i++)
            //{
            //    c[i] = colors[(i + ((i / 4) % 2)) % 2];
            //}

            for (int i = 0; i < 16; i++)
            {
                if (i < 8)
                {
                    c[i] = getColorLayout(i, colors[0], colors[1], lay1);
                }
                else
                {
                    c[i] = getColorLayout(i, colors[0], colors[1], lay2);
                    //MessageBox.Show(c[i].ToString());
                }
            }
            return c;
        }

        public Color[] getTexelOld(int color1, int color2, int lay1, int lay2)
        {
            Color[] colors = { getColor565(color1), getColor565(color2) };
            Color[] c = new Color[16];

            for (int i = 0; i < 16; i++)
            {
                c[i] = colors[(i + ((i / 4) % 2)) % 2];
            }

            return c;
        }//dxt1

        public Color getColorLayout(int p, Color c1, Color c2, int layout)
        {
            p %= 8;
            Color ret = new Color();
            int andIndex = (int)Math.Pow(2, p * 2);

            //MessageBox.Show(p + "");
            int andIndex2 = (int)Math.Pow(2, (p * 2) + 1);
            //MessageBox.Show("if ((" + layout.ToString("X8") + " & " + andIndex + ") == 0");
            if ((layout & andIndex) == 0)
            {
                ret = c1;
            }
            else
            {
                ret = c2;
            }
            if ((layout & andIndex2) != 0) //average
            {
                ret = Average(ret, c1, c2);
            }
            return ret;
        } //dxt1
        public Color getColorLayout(int layout, int avg, int alphaLayout, Color c1, Color c2, int[] alphas) //dxt3
        {
            Color preColor = new Color[] { c1, c2 }[layout];
            if (avg == 1)
            {
                preColor = Average(preColor, c1, c2);
            }
            return Color.FromArgb(alphas[alphaLayout], preColor.R, preColor.G, preColor.B);
        }

        public Color getColor565(int word)
        {
            //16bits to to rgb565
            Color c = new Color();
            int blue = (int)(255.0 * ((word & 0x1F) / (double)0x1F));
            int green = (int)(255.0 * (((word >> 5) & 0x3F) / (double)0x3F));
            int red = (int)(255.0 * (((word >> 11) & 0x1F) / (double)0x1F));
            //int blue = (word & 0x1F) << 3;
            //int green = (word & 0x1E0) >> 3;
            //int red = (word & 0xF800) >> 8;
            c = Color.FromArgb(red, green, blue);
            return c;
        }
        public Color Average(Color c, Color c1, Color c2)
        {
            return Color.FromArgb((c.R + c1.R + c2.R) / 3, (c.G + c1.G + c2.G) / 3, (c.B + c1.B + c2.B) / 3);
        }
        public Color halfColor(Color c)
        {
            return Color.FromArgb(c.R / 2, c.G / 2, c.B / 2);
        }

        public Color[][] ColorCopy(Color[][] sourceArray, Color[][] destArray, int x, int y)
        {
            for (int i = x; i < x + sourceArray[0].Length; i++)
            {
                for (int j = y; j < y + sourceArray.Length; j++)
                {
                    destArray[j][i] = sourceArray[j - y][i - x];
                }
            }
            return destArray;
        }
        public Color[][] ColorCopy(Color[][] sourceArray, int sourceX, int sourceY, int w, int h, Color[][] destArray, int destX, int destY)
        {
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    //MessageBox.Show("destArray[" + (destY + i) + "][" + (destX + j) + "] = sourceArray[" + (sourceY) + " " + (i) + "][" + (sourceX + j) + "]");
                    destArray[destY + i][destX + j] = sourceArray[sourceY + i][sourceX + j];
                }
            }
            return destArray;
        }
        public Color[][] swapPanel(Color[][] duoPanel)
        {
            Color[][] ret = new Color[duoPanel.Length][];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = new Color[duoPanel[i].Length];
                for (int j = 0; j < ret[i].Length / 2; j++)
                {
                    ret[i][j] = duoPanel[i][j + 64];
                }
                for (int j = 0; j < ret[i].Length / 2; j++)
                {
                    ret[i][j + 64] = duoPanel[i][j];
                }
            }
            return ret;
        }

        public int texelCalc(int i)
        {
            int ret = i;
            ret = DataMethods.swapBits(ret, 1, 3);
            ret = DataMethods.swapBits(ret, 2, 1);
            return ret;
        }
    }
}