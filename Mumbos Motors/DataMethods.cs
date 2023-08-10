using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Mumbos_Motors
{
    class DataMethods
    {
        public DataMethods()
        {

        }
        /**
         * Reads 32 bit integers starting at the offset.
        **/
        public static int readInt16(byte[] data, int offs)
        {
            int num = 0;
            for (int i = 0; i < 2; i++)
            {
                num |= data[offs + i] << (16 - ((i + 1) * 8));
            }
            return num;
        }
        public static int readInt32(byte[] data, int offs)
        {
            int num = 0;
            for (int i = 0; i < 4; i++)
            {
                num |= data[offs + i] << (32 - ((i + 1) * 8));
            }
            return num;
        }
        public static int readInt32(string path, int offs)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(path));
            br.BaseStream.Position = offs;
            int num = 0;
            for (int i = 0; i < 4; i++)
            {

                num |= br.ReadByte() << (32 - ((i + 1) * 8));
            }
            return num;
        }
        public static int readInt(byte[] data, int offs, int len)
        {
            int num = 0;
            for (int i = 0; i < len; i++)
            {
                num |= data[offs + i] << ((len * 8) - ((i + 1) * 8));
            }
            return num;
        }
        public static byte[] readDataSection(string path, int offs, int len)
        {
            byte[] ret = new byte[len];
            BinaryReader br = new BinaryReader(File.OpenRead(path));
            br.BaseStream.Position = offs;
            for (int i = 0; i < len; i++)
            {
                ret[i] = br.ReadByte();
            }
            br.Dispose();
            return ret;
        }

        public static byte[] writeInt16(byte[] data, int offs, int value)
        {
            for (int i = offs; i < offs + 2; i++)
            {
                data[i] = (byte)(value >> (8 - (8 * (i - offs))));
            }
            return data;
        }
        public static byte[] writeInt32(byte[] data, int offs, int value)
        {
            for (int i = offs; i < offs + 4; i++)
            {
                data[i] = (byte)(value >> (24 - (8 * (i - offs))));
            }
            return data;
        }
        public static byte[] writeInt(byte[] data, int offs, int value, int len)
        {
            for (int i = 0; i < len; i++)
            {
                data[offs + i] = (byte)(value >> ((len * 8) - ((i + 1) * 8)));
            }
            /*
            string debug = "";
            MessageBox.Show(data.Length + "");
            for (int i = 0; i < data.Length; i++)
            {
                debug += data[i].ToString("X2") + " ";
                if (i % 16 == 0)
                {
                    debug += "\n";
                }
            }
            MessageBox.Show(debug);
            */
            return data;
        }
        public static void writeDataSection(string path, int offs, int len, byte[] data)
        {
            BinaryWriter br = new BinaryWriter(File.OpenWrite(path));
            br.BaseStream.Position = offs;
            for (int i = 0; i < len; i++)
            {
                br.Write(data[i]);
            }
            br.Dispose();
        }

        public static float readFloat32(byte[] data, int offs)
        {
            byte[] bytes = new byte[4];
            for (int i = offs; i < offs + 4; i++)
            {
                bytes[i - offs] = data[i];
            }
            if (BitConverter.IsLittleEndian)
            {
                bytes = bytes.Reverse().ToArray();
            }
            return BitConverter.ToSingle(bytes, 0);
        }
        public static float readFloat(string value)
        {
            byte[] bytes = BitConverter.GetBytes(Convert.ToInt32(value));
            if (BitConverter.IsLittleEndian)
            {
                bytes = bytes.Reverse().ToArray();
            }
            return BitConverter.ToSingle(bytes, 0);
        }

        //KEEP GOING UNTIL NULL CHAR
        public static string readString(byte[] data, int offs, int length)
        {
            string hi = "";
            for (int i = offs; i < offs + length; i++)
            {
                if (data[i] >= 0x20)
                {
                    //MessageBox.Show(((char)(data[i])).ToString());
                    hi += Convert.ToChar(data[i]);
                }
            }
            return hi;
        }
        public static string readString(string path, int offs, int length)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(path));
            string hi = "";
            for (int i = offs; i < offs + length; i++)
            {
                br.BaseStream.Position = i;
                byte sample = br.ReadByte();
                if (sample >= 0x20)
                {
                    hi += Convert.ToChar(sample);
                }
            }
            br.Dispose();
            return hi;
        }
        public static string readString(byte[] data, int offs)
        {
            string hi = "";
            int i = offs;
            while (data[i] != 0)
            {
                hi += Convert.ToChar(data[i]);
                i++;
            }
            return hi;
        }
        public static string readNullTerminatedString(string path, int offs)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(path));
            br.BaseStream.Position = offs;
            string ret = "";

            bool stop = false;
            while (!stop)
            {
                byte read = br.ReadByte();
                stop = read == 0x0;
                if (!stop)
                {
                    ret += Convert.ToChar(read);
                }
            }
            br.Dispose();
            return ret;
        }
        public static string readString(string symbol, int offs)
        {
            string hi = "";
            int i = offs;
            while (symbol[i] != '_')
            {
                hi += symbol[i];
                i++;
            }
            return hi;
        }
        public static byte[] writeString(byte[] data, string text, int offs, int len)
        {
            for (int i = offs; i < offs + len; i++)
            {
                if (i - offs < text.Length)
                {
                    data[i] = (byte)text[i - offs];
                }
                else
                {
                    data[i] = 0x0;
                }
            }
            return data;
        }

        /// <summary>
        /// Read symbol string, and TRUNCATE D:\LocalLibrary and \default.rtx
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="offs"></param>
        /// <returns></returns>
        public static string readTextureSymbol(string symbol)
        {
            string ret = "";
            int i = 23;
            while(symbol[i] != '\\' && i != symbol.Length - 1)
            {
                //MessageBox.Show(symbol.Length + "\ni: " + i + "\nSymbol[i]: " + symbol[i]);
                ret += symbol[i];
                i++;

            }
            return ret;
        }

        public static string rebuildTextureSymbol(string symbol)
        {
            return "D:\\LocalLibrary\\BanjoX\\" + symbol + "\\default.rtx";
        }

        public static void SetClipboard(string text)
        {
            StringBuilder clipboard = new StringBuilder();
            Clipboard.SetText(text.ToString());
        }

        public static string[] getAllTagCatagories(string[] symbols)
        {
            string[] tags = new string[0];
            for (int i = 0; i < symbols.Length; i++)
            {
                if (symbols[i][0] == 'D')
                {
                    if (!tags.Contains("texture"))
                    {
                        tags = addToArray(tags, "texture");
                    }
                }
                else if (symbols[i][0] == 'a')
                {
                    if (!tags.Contains(readString(symbols[i], 4)))
                    {
                        tags = addToArray(tags, readString(symbols[i], 4));
                    }
                }
            }
            tags = sortArray(tags);
            return tags;
        }

        public static string[][] orderTags(string[] Catagories, string[] symbols)
        {
            string[][] orderedTags = new string[Catagories.Length][];
            for (int i = 0; i < orderedTags.Length; i++) //instantiate catagories
            {
                orderedTags[i] = new string[0];
                //MessageBox.Show(orderedTags[i].Length + "");
            }
            for (int i = 0; i < symbols.Length; i++) //Add all symbols to jagged array
            {
                if (symbols[i][0] == 'D')
                {
                    int catagoryIndex = getIndexBySearch(Catagories, readString(symbols[i], 0x1b));
                    //MessageBox.Show(catagoryIndex + "");
                    orderedTags = addToJaggedArray(orderedTags, catagoryIndex, readTextureSymbol(symbols[i]));
                    //MessageBox.Show(orderedTags[catagoryIndex][orderedTags[catagoryIndex].Length - 1]);
                }
                else if (symbols[i][0] == 'a')
                {
                    int catagoryIndex = getIndexBySearch(Catagories, readString(symbols[i], 0x4));
                    //MessageBox.Show(catagoryIndex + "");
                    orderedTags = addToJaggedArray(orderedTags, catagoryIndex, symbols[i]);
                    //MessageBox.Show(orderedTags[catagoryIndex][orderedTags[catagoryIndex].Length - 1]);
                }
            }

            for (int i = 0; i < orderedTags.Length; i++) //Sort each catagory
            {
                orderedTags[i] = sortArray(orderedTags[i]);
            }
            return orderedTags;
        }
        public static string[] addToArray(string[] array, string s)
        {
            string[] ret = new string[array.Length + 1]; //1
            array.CopyTo(ret, 0);
            ret[ret.Length - 1] = s;
            return ret;
        }
        private static string[][] addToJaggedArray(string[][] array, int catagoryIndex, string s)
        {
            /*
            string debug = "";
            for (int i = 0; i < array.Length; i++)
            {
                debug += i + " " + array[i].Length + ": ";
                for (int j = 0; j < array[i].Length; j++)
                {
                    debug += array[i][j] + " ";
                }
                debug += "\n";
            }
            MessageBox.Show(debug);
            */
            //MessageBox.Show("len of catagory: " + array[catagoryIndex].Length);

            #region//CREATE TEMPORARY COPY OF ARRAY
            string[][] temp = new string[array.Length][];

            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = new string[array[i].Length];
                for (int j = 0; j < temp[i].Length; j++)
                {
                    temp[i][j] = array[i][j];
                }
            }


            #endregion

            //string[][] ret = new string[array.Length][];
            temp[catagoryIndex] = new string[array[catagoryIndex].Length + 1];

            //MessageBox.Show("len of catagory: " + ret[catagoryIndex].Length);
            for (int i = 0; i < array[catagoryIndex].Length; i++)
            {
                    temp[catagoryIndex][i] = array[catagoryIndex][i];
                    //MessageBox.Show("Copy THIS: " + array[catagoryIndex][i] + "\nlen of catagory: " + array[catagoryIndex].Length);
                
            }
            //array[catagoryIndex].CopyTo(ret[catagoryIndex], 0);

            temp[catagoryIndex][temp[catagoryIndex].Length - 1] = s;
            //MessageBox.Show("start of array: " + ret[catagoryIndex][ret[catagoryIndex].Length - 1]);
            //MessageBox.Show("end");
            return temp;
        }

        public static string[] getStringsBySearch(string[] symbols, string search)
        {
            string[] occurances = new string[0];
            for (int i = 0; i < symbols.Length; i++)
            {
                if (symbols[i].Contains(search))
                {
                    occurances = addToArray(occurances, symbols[i]);
                }
            }
            //MessageBox.Show(occurances[0] + "\n" + occurances[1]);
            return occurances;
        }

        public static int getIndexBySearch(string[] symbols, string symbol)
        {
            int i = 0;
            for (int g = 0; g < symbols.Length; g++)
            {
                i = g;
                if (symbols[i] == symbol)
                {
                    g = symbols.Length;
                }
            }
            return i;
        }

        /// <summary>
        /// Returns a substring from a symbol based on position, and # underscores allowed
        /// </summary>
        /// <param name="symbol">string symbol input</param>
        /// <param name="position">where to start reading</param>
        /// <param name="amount">number of allowed underscores</param>
        /// <returns></returns>
        public static string getNameOfSymbol(string symbol, int position, int amount)
        {
            string ret = "";
            int underscoreOccurances = 0;
            int i = position;
            bool stop = false;
            while (!stop && i < symbol.Length)
            {
                underscoreOccurances += Convert.ToInt32(symbol[i] == '_');
                stop = underscoreOccurances > amount;
                if (!stop)
                {
                    ret += symbol[i];
                }
                i++;
            }
            return ret;
        }
        /// <summary>
        /// Returns a substring from a symbol BACKWARDS based on number of underscores amount 
        /// </summary>
        /// <param name="symbol">input string</param>
        /// <param name="position">position from backtracing</param>
        /// <param name="amount">number of allowed underscores</param>
        /// <returns></returns>
        public static string getNameOfSymbolBackwards(string symbol, int position, int amount)
        {
            int numberOfUnderscores = 0;
            int skips = 0;
            string ret = "";
            int Amount = 0;
            amount++;

            for (int i = 0; i < symbol.Length; i++) //Get number of underscores
            {
                if (symbol[i] == '_')
                {
                    numberOfUnderscores++;
                }
            }
            position = numberOfUnderscores - position;

            for (int i = 0; i < symbol.Length - 1; i++)//Read symbol
            {
                if (symbol[i] == '_')
                {
                    skips++;
                }
                if (skips == position)
                {
                    i++;
                    bool stop = false;
                    while (!stop)
                    {
                        if (symbol[i] == '_' || i == symbol.Length - 1)
                        {
                            Amount++;
                        }
                        if (Amount != amount)
                        {
                            ret += symbol[i];
                            i++;
                        }
                        else
                        {
                            if (symbol[i] != '_')
                            {
                                ret += symbol[i];
                            }
                            stop = true;
                        }
                    }
                    i = symbol.Length;
                }
            }
            return ret;
        }

        public static string[] sortArray(string[] symbols)
        {
            List<string> sorted = new List<string>();
            for (int i = 0; i < symbols.Length; i++)
            {
                sorted.Add(symbols[i]);
            }
            sorted.Sort();
            string[] ret = new string[symbols.Length];
            for (int i = 0; i < symbols.Length; i++)
            {
                ret[i] = sorted[i];
            }
            return ret;
        }

        public static string[] listToArray(List<string> list)
        {
            string[] ret = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                ret[i] = list[i];
            }
            return ret;
        }

        public static string dataToHexString(byte[] sectionData, int row)
        {
            string ret = "";
            for (int i = 0; i < sectionData.Length; i++)
            {
                if (i > 0 && i % row == 0)
                {
                    ret += "\n";
                }
                ret += sectionData[i].ToString("X2");
                if (i != sectionData.Length - 1 && (i + 1) % row != 0)
                {
                    ret += " ";
                }
            }
            return ret;
        }

        public static string dataToASCIIString(byte[] sectionData, int row)
        {
            string ret = "";
            for (int i = 0; i < sectionData.Length; i++)
            {
                if (i > 0 && i % row == 0)
                {
                    ret += "\n";
                }
                if (sectionData[i] >= 0x20)
                {
                    ret += (char)(sectionData[i]);
                }
                else
                {
                    ret += '.';
                }
                
            }
            return ret;
        }

        public static void saveFileDialog(byte[] data, string filename, string title)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save " + title;
            sfd.Filter = "All files (*.*)|";
            sfd.FileName = filename;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(Path.GetFullPath(sfd.FileName), data);
            }
            sfd.Dispose();
        }

        public static byte[] openFileDialog(string title)
        {
            byte[] ret = new byte[0];
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = title;
            ofd.Filter = title + " (*.*)|";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ret = File.ReadAllBytes(ofd.FileName);
                ofd.Dispose();
            }
            return ret;
        }

        public static string saveFileDialogTexture(string fileName)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.Title = "Save Image as .png";
            ofd.Filter = "png files (*.png)|*.*";
            ofd.FileName = fileName;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                return ofd.FileName;
            }
            return "";
        }

        public static byte[] swapEndianness(byte[] data, int len)
        {
            byte[] newData = new byte[data.Length];
            for (int i = 0; i < data.Length; i += len)
            {
                for (int j = 0; j < len; j++)
                {
                    newData[i + (len - 1 - j)] = data[i + j];
                }
            }
            return newData;
        }

        public static int swapBits(int n, int p1, int p2)
        {
            int result = n;
            result = nullBit(result, p1);
            result = nullBit(result, p2);
            result |= ((n >> p1) & 1) << p2;
            result |= ((n >> p2) & 1) << p1;
            return result;
        }
        public static int nullBit(int num, int pos)
        {
            return num &= ~(1 << pos);
        }
    }
}
