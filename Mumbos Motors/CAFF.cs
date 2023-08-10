using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Mumbos_Motors
{
    public struct fileInfo
    {
        public int ID;
        public int dataOffs;
        public int dataSize;
        public int section;
        public byte unknown0xD;
    }

    public struct sectionInfo
    {
        public string name;
        public int unknown0x1;
        public int unknown0x5;
        public int dataLength;
        public int dataLength2;
    }

    public class CAFF
    {
        public string path;
        public string fileName;
        private string[] tagCatagories;
        string[][] orderedTags;

        public byte[] data;
        public int address;
        private string version;
        public int sizeOfHeader;
        public int headerChecksumOffs = 0x18;
        private int headerChecksum;
        private int numberOfSymbols;
        private int numberOfFileParts;
        private int unknown0x2C;
        private int unknown0x30;
        private int unknown0x44;
        private byte type;
        private byte numberOfSections;
        private byte unknown0x4A;
        private byte unknown0x4B;
        private int unknown0x4C;
        private int infoSectionSize;
        private int infoSectionSize2;
        private int unknownDataSize;
        private int unknownDataSize2;
        private int sectionHeaderSize = 0x21;

        public fileInfo[] fileInfos;
        public int fileInfoHeaderSize = 0xE;
        public sectionInfo[] sections;

        private int symbolsLength;
        private int[] symbolsOffsets;
        private int symbolsStart;
        private string[] symbols;
        private int unknownNameSize;
        private string unknownName;
        private byte[] unknownData;
        public int fileInfosStart;
        private int dataStart;


        private bool error = false;
        private string errorMessage = "none";


        public CAFF(string path)
        {
            this.path = path;
            fileName = Path.GetFileName(path);
            data = File.ReadAllBytes(path);
            readCAFFData();
        }

        public CAFF(byte[] data, int address)
        {
            this.data = data;
            this.address = address;
            readCAFFData();
        }

        public void readCAFFData()
        {
            int p;
            try
            {
                //Juicy information
                version = DataMethods.readString(data, 0x4, 0x10);
                sizeOfHeader = DataMethods.readInt32(data, 0x14);
                headerChecksum = DataMethods.readInt32(data, headerChecksumOffs);
                numberOfSymbols = DataMethods.readInt32(data, 0x1C);
                numberOfFileParts = DataMethods.readInt32(data, 0x20);
                type = data[0x48];
                numberOfSections = data[0x49];
                infoSectionSize = DataMethods.readInt32(data, 0x50);
                infoSectionSize2 = DataMethods.readInt32(data, 0x60);

                //Unknown Junk
                unknown0x2C = DataMethods.readInt32(data, 0x2C);
                unknown0x30 = DataMethods.readInt32(data, 0x30);
                unknown0x44 = DataMethods.readInt32(data, 0x44);
                unknown0x4A = data[0x4A];
                unknown0x4B = data[0x4B];
                unknown0x4C = DataMethods.readInt32(data, 0x4C);
                unknownDataSize = DataMethods.readInt32(data, 0x64);
                unknownDataSize2 = DataMethods.readInt32(data, 0x74);

                sections = new sectionInfo[numberOfSections];

                //Store the differet section header infos in the struct (0x21 each)
                for (int i = 0; i < numberOfSections; i++)
                {
                    //Start of section header
                    p = sizeOfHeader + (i * sectionHeaderSize) + 1;

                    //Fart nugget
                    sections[i].unknown0x1 = DataMethods.readInt32(data, p);
                    sections[i].unknown0x5 = DataMethods.readInt32(data, p + 4);

                    //Juice
                    sections[i].dataLength = DataMethods.readInt32(data, p + 8);
                    sections[i].dataLength2 = DataMethods.readInt32(data, p + 0x1C);

                }
                p = (numberOfSections * sectionHeaderSize) + sizeOfHeader;
                //READ "data, .texturegpu, .gpu, .gpucached, .stream"
                for (int i = 0; i < numberOfSections; i++)
                {
                    sections[i].name = DataMethods.readString(data, p);
                    p = p + sections[i].name.Length + 1;

                }

                symbolsLength = DataMethods.readInt32(data, p); //Length of symbol buffer
                p += 0x4;
                symbolsOffsets = new int[numberOfSymbols]; //ARRAY FOR LOCATIONS OF NAMES (Symbols)...
                for (int i = 0; i < numberOfSymbols; i++)
                {
                    symbolsOffsets[i] = DataMethods.readInt32(data, p);
                    p += 0x4;

                }
                symbolsStart = p;

                //Get all TAG (symbol) names with the symbol offsets
                symbols = new string[numberOfSymbols];
                for (int i = 0; i < numberOfSymbols; i++)
                {
                    p = symbolsStart + symbolsOffsets[i];
                    symbols[i] = DataMethods.readString(data, p);
                }
                p += symbols[numberOfSymbols - 1].Length + 1;

                unknownNameSize = DataMethods.readInt32(data, p);
                p += 4;
                unknownName = DataMethods.readString(data, p, unknownNameSize);
                p += unknownNameSize;

                fileInfos = new fileInfo[numberOfFileParts];
                fileInfosStart = p;
                for (int i = 0; i < numberOfFileParts; i++)
                {
                    fileInfos[i].ID = DataMethods.readInt32(data, p);
                    fileInfos[i].dataOffs = DataMethods.readInt32(data, p + 0x4);
                    fileInfos[i].dataSize = DataMethods.readInt32(data, p + 0x8);
                    fileInfos[i].section = data[p + 0xC];
                    fileInfos[i].unknown0xD = data[p + 0xD];
                    p += fileInfoHeaderSize;
                }

                /**
                 * Credit to mojobojo for this code in caffextractor reference.
                 **/
                int padding = (p % 4);
                if (padding != 0)
                {
                    p += 4 - padding;
                }

                unknownData = new byte[unknownDataSize];
                for (int i = p; i < unknownDataSize + p; i++)
                {
                    unknownData[i - p] = data[i];
                }

                dataStart = p + unknownDataSize;
                //MessageBox.Show(dataStart.ToString("X8"));
            }
            catch(Exception ex)
            {
                error = true;
                errorMessage = ex.ToString();
            }


        }

        /// <summary>
        /// Return the index for WHERE the fileInfos[].ID is located
        /// </summary>
        /// <param name="preFileID"></param> The ACTUAL fileInfos[].ID we are looking for.
        /// <returns></returns>
        public byte[][] readSectionsData(int symbolID)
        {
            //MessageBox.Show("symbolID: " + symbolID);
            int offsFileID = findFileIDSymbolID(symbolID);
            //MessageBox.Show("fileinfo offs: " + offsFileID + "");
            byte[][] sectionData = new byte[numSectionsSymbolID(symbolID)][];
            //MessageBox.Show("sections: " + sectionData.Length);
            //MessageBox.Show("Name: " + caff.getSymbols()[caff.fileInfos[newFileID].ID - 1] + "\n DataStart: " + caff.getDataStart().ToString("X8") + "\n GetSectionOffset: " + caff.getSectionOffset(caff.fileInfos[newFileID].section).ToString("X8") + "\n FileOffset: " + caff.fileInfos[newFileID].dataOffs.ToString("X8") + "\nPosition = " + (caff.getDataStart() + caff.getSectionOffset(caff.fileInfos[newFileID].section) + caff.fileInfos[newFileID].dataOffs).ToString("X8") + "\n\n DataSize: " + caff.fileInfos[newFileID].dataSize.ToString("X8") + "\nID: " + caff.fileInfos[newFileID].ID.ToString("X8"));
            for (int i = 0; i < sectionData.Length; i++)
            {
                int p = getSectionOffset(offsFileID, i);
                sectionData[i] = new byte[fileInfos[offsFileID + i].dataSize];

                for (int j = p; j < p + sectionData[i].Length; j++)
                {
                    sectionData[i][j - p] = data[j];
                }
            }
            return sectionData;
        }
        public byte[] readSectionData(int symbolID, int section) //section base 0
        {
            int offsFileID = findFileIDSymbolID(symbolID);
            //MessageBox.Show("Name: " + caff.getSymbols()[caff.fileInfos[newFileID].ID - 1] + "\n DataStart: " + caff.getDataStart().ToString("X8") + "\n GetSectionOffset: " + caff.getSectionOffset(caff.fileInfos[newFileID].section).ToString("X8") + "\n FileOffset: " + caff.fileInfos[newFileID].dataOffs.ToString("X8") + "\nPosition = " + (caff.getDataStart() + caff.getSectionOffset(caff.fileInfos[newFileID].section) + caff.fileInfos[newFileID].dataOffs).ToString("X8") + "\n\n DataSize: " + caff.fileInfos[newFileID].dataSize.ToString("X8") + "\nID: " + caff.fileInfos[newFileID].ID.ToString("X8"));
            int p = getSectionOffset(offsFileID, section);
            byte[] sectionData = new byte[fileInfos[offsFileID + section].dataSize];

            for (int j = p; j < p + sectionData.Length; j++)
            {
                sectionData[j - p] = data[j];
            }
            return sectionData;
        }

        public void writeSectionDataNew(int symbolID, byte[] sectionData, int section, int newSize)
        {
            int Section = section + 1;
            int offsFileID = findFileIDSymbolID(symbolID);

            //create the new data with shifted data
            byte[] newData = new byte[data.Length + newSize];

            Array.Copy(data, newData, getSectionOffsetSymbolID(symbolID, section));

            sectionData.CopyTo(newData, getSectionOffsetSymbolID(symbolID, section));
            int remainingDataOffs = getSectionOffsetSymbolID(symbolID, section) + sectionData.Length;
            for (int i = remainingDataOffs; i < newData.Length; i++)
            {
                newData[i] = data[i - newSize];
            }

            //up the datasize for fileinfos[] datasize
            fileInfos[offsFileID].dataSize = fileInfos[offsFileID].dataSize + newSize;

            //traverse through fileInfos[i > findFileIDsymbolID(symbolID)]: if (fileinfos[i].Section == section) then dataoffset += newsize
            for (int i = offsFileID + 1; i < fileInfos.Length; i++)
            {
                if (fileInfos[i].section == Section)
                {
                    fileInfos[i].dataOffs = fileInfos[i].dataOffs + newSize;
                }
            }

            //sectioninfo[section].datalength += newsize
            sections[section].dataLength = sections[section].dataLength + newSize;
            sections[section].dataLength2 = sections[section].dataLength2 + newSize;

            //write new fileinfos[], write new sectioninfo[]
            newData = writeFileInfos(newData);
            newData = writeSectionInfos(newData);

            //recalculatechecksum()
            newData = recalcHeaderChecksum(newData);

            data = new byte[newData.Length];
            newData.CopyTo(data, 0);
            File.WriteAllBytes(path, data);
            //MessageBox.Show("newData Size: " + newData.Length + "\n oldSize: " + data.Length);
        }

        public byte[] writeFileInfos(byte[] Data)
        {
            int g = fileInfosStart;
            for (int i = 0; i < numberOfFileParts; i++)
            {
                Data = DataMethods.writeInt32(Data, g, fileInfos[i].ID);
                Data = DataMethods.writeInt32(Data, g + 0x4, fileInfos[i].dataOffs);
                Data = DataMethods.writeInt32(Data, g + 0x8, fileInfos[i].dataSize);
                Data[g + 0xC] = (byte)fileInfos[i].section;
                Data[g + 0xD] = fileInfos[i].unknown0xD;
                g += fileInfoHeaderSize;
            }
            return Data;
        }
        public byte[] writeSectionInfos(byte[] Data)
        {
            int p = 0;
            for (int i = 0; i < numberOfSections; i++)
            {
                //Start of section header
                p = sizeOfHeader + (i * sectionHeaderSize) + 1;

                Data = DataMethods.writeInt32(Data, p, sections[i].unknown0x1);
                Data = DataMethods.writeInt32(Data, p + 0x4, sections[i].unknown0x5);
                Data = DataMethods.writeInt32(Data, p + 0x8, sections[i].dataLength);
                Data = DataMethods.writeInt32(Data, p + 0x1C, sections[i].dataLength2);

            }
            return Data;
        }

        public void writeSectionData(int symbolID, byte[][] sectionData)
        {
            try
            {
                BinaryWriter bw = new BinaryWriter(File.OpenWrite(path));
                //MessageBox.Show(sectionData[1][sectionData[1].Length - 1] + "");
                int NumberOfSections = sectionData.Length;
                int offsFileID = findFileIDSymbolID(symbolID);
                //MessageBox.Show("Name: " + caff.getSymbols()[caff.fileInfos[newFileID].ID - 1] + "\n DataStart: " + caff.getDataStart().ToString("X8") + "\n GetSectionOffset: " + caff.getSectionOffset(caff.fileInfos[newFileID].section).ToString("X8") + "\n FileOffset: " + caff.fileInfos[newFileID].dataOffs.ToString("X8") + "\nPosition = " + (caff.getDataStart() + caff.getSectionOffset(caff.fileInfos[newFileID].section) + caff.fileInfos[newFileID].dataOffs).ToString("X8") + "\n\n DataSize: " + caff.fileInfos[newFileID].dataSize.ToString("X8") + "\nID: " + caff.fileInfos[newFileID].ID.ToString("X8"));
                for (int i = 0; i < NumberOfSections; i++)
                {

                    int p = getSectionOffset(offsFileID, i);
                    bw.Seek(p, SeekOrigin.Begin);
                    for (int j = p; j < p + sectionData[i].Length; j++)
                    {
                        int offs = j - p;
                        //MessageBox.Show("offs: " + offs + "\nsectionData.len:" + sectionData[i].Length + "\nWrite: " + sectionData[i][offs]);
                        bw.Write(sectionData[i][offs]);
                    }


                }
                //File.WriteAllBytes(path, data);
                bw.Dispose();
            }
            catch
            {
                MessageBox.Show("File Inaccessible");
            }
            
        }
        public void writeSectionData(int symbolID, byte[] sectionData, int section)
        {
            try
            {
                BinaryWriter bw = new BinaryWriter(File.OpenWrite(path));
                int offsFileID = findFileIDSymbolID(symbolID);
                int p = getSectionOffset(offsFileID, section);

                bw.Seek(p, SeekOrigin.Begin);
                for (int i = 0; i < sectionData.Length; i++)
                {
                    bw.Write(sectionData[i]);
                }
                bw.Dispose();
            }
            catch
            {
                MessageBox.Show("File Inaccessible");
            }
        }

        public int getSectionOffset(int offsFileID, int section)
        {
            return getDataStart() + getSectionOffset(fileInfos[offsFileID + section].section) + fileInfos[offsFileID + section].dataOffs;
        }
        public int getSectionOffsetSymbolID(int symbolID, int section)
        {
            return getDataStart() + getSectionOffset(fileInfos[findFileIDSymbolID(symbolID) + section].section) + fileInfos[findFileIDSymbolID(symbolID) + section].dataOffs;
        }

        public int findFileIDSymbolID(int symbolID)//Add +1 to symbolID for fileInfos[].ID to correspond
        {
            return findFileID(symbolID + 1);
        }
        public int findFileID(int fileID)//Find the OFFSET for this file id
        {
            int i = 0;
            while (fileID != fileInfos[i].ID)
            {
                i++;
            }
            return i;
        }

        /// <summary>
        /// Count the number of SAME fileInfos[].ID occurances.
        /// </summary>
        /// <param name="fileID"></param>
        /// <returns></returns>
        public int numSectionsFileID(int fileID)
        {
            int occurances = 0;
            for (int i = 0; i < fileInfos.Length; i++)
            {
                if (fileID == fileInfos[i].ID)
                {
                    occurances++;
                }
            }
            return occurances;
        }
        public int numSectionsSymbolID(int symbolID)
        {
            return numSectionsFileID(symbolID + 1);
        }

        /// <summary>
        /// Credit to mojobojo for this function
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public int getSectionOffset(int section)
        {
            int ret = 0;
            for (int i = 0; i < section - 1; i++)
            {
                ret += sections[i].dataLength;
            }
            return ret;
        }

        public int getFileInfoOffs(int symbolID, int section)
        {
            return ((findFileIDSymbolID(symbolID) + section) * fileInfoHeaderSize) + fileInfosStart;
        }

        /// <summary>
        /// CREDIT TO MOJOBOJO FOR THIS AWESOME, WORKING HEADER CHECKSUM RECALCULATOR :D
        /// Calculate the new header checksum if pointers change.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public uint checksum32(byte[] data, int length)
        {
            uint r11 = 0;

            for (int i = 0; i < length; i++)
            {
                uint r8 = data[i];
                uint r10 = r11 << 4;

                if ((r8 & 0x80) > 0)
                {
                    r11 = 0xFFFFFF80 | r8;
                }
                else
                {
                    r11 = r8;
                }

                r11 = r11 + r10;
                r10 = r11 & 0xF0000000;

                if (r10 != 0)
                {
                    r8 = r10 >> 24;
                    r10 = r8 | r10;
                    r11 = r10 ^ r11;
                }
            }
            return r11;
        }

        public byte[] recalcHeaderChecksum(byte[] data)
        {
            int length = 0x78;
            data = DataMethods.writeInt32(data, headerChecksumOffs, 0x0);
            int checksum = (int)checksum32(data, length);
            //MessageBox.Show(checksum.ToString("X8") + "");
            data = DataMethods.writeInt32(data, headerChecksumOffs, checksum);
            return data;
        }


        public string getVersion()
        {
            return version;
        }
        public int getSizeOfHeader()
        {
            return sizeOfHeader;
        }
        public int getHeaderChecksum()
        {
            return headerChecksum;
        }
        public int getNumberOfSymbols()
        {
            return numberOfSymbols;
        }
        public int getNumberOfFileParts()
        {
            return numberOfFileParts;
        }
        public int getUnknown0x2C()
        {
            return unknown0x2C;
        }
        public int getUnknown0x30()
        {
            return unknown0x30;
        }
        public int getUnknown0x44()
        {
            return unknown0x44;
        }
        public int getType()
        {
            return type;
        }
        public int getNumberOfSections()
        {
            return numberOfSections;
        }
        public int getUnknown0x4A()
        {
            return unknown0x4A;
        }
        public int getUnknown0x4B()
        {
            return unknown0x4B;
        }
        public int getUnknown0x4C()
        {
            return unknown0x4C;
        }
        public int getInfoSectionSize()
        {
            return infoSectionSize;
        }
        public int getInfoSectionSize2()
        {
            return infoSectionSize2;
        }
        public int getUnknownDataSize()
        {
            return unknownDataSize;
        }
        public int getUnknownDataSize2()
        {
            return unknownDataSize2;
        }

        //symbols
        public int getSymbolsLength()
        {
            return symbolsLength;
        }
        public int[] getSymbolOffsets()
        {
            return symbolsOffsets;
        }
        public int getSymbolsStart()
        {
            return symbolsStart;
        }
        public string[] getSymbols()
        {
            return symbols;
        }

        //unknown name
        public int getUnknownNameSize()
        {
            return unknownNameSize;
        }
        public string getUnknownName()
        {
            return unknownName;
        }
        public byte[] getUnknownData()
        {
            return unknownData;
        }
        public int getDataStart()
        {
            return dataStart;
        }

        public bool getError()
        {
            return error;
        }
        public string getErrorMessage()
        {
            return errorMessage;
        }

        public void setTagCatagories(string[] tagCatagories)
        {
            this.tagCatagories = tagCatagories;
        }

        public void setOrderedTags(string[][] orderedTags)
        {
            this.orderedTags = orderedTags;
        }

        public string[] getTagCatagories()
        {
            return tagCatagories;
        }

        public string[][] getOrderedTags()
        {
            return orderedTags;
        }
    }
}
