using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mumbos_Motors
{
    public struct SectionInfo
    {
        public int Checksum;
        public int Address;
        public int Length;
    }
    public class MULTICAFF
    {
        public List<CAFF> caffs = new List<CAFF>();
        public List<DNBW> dnbws = new List<DNBW>();
        public string[] dnbwNames;

        public string path;
        public string Title;
        public int sectionHeaderLen;
        public int numSections;
        public int headerChecksum;
        public int num0x4Skips;
        public int sectionHeadersStart;
        public int dataStart;

        public SectionInfo[] sectionInfo;

        public MULTICAFF(string path)
        {
            this.path = path;
            ReadMULTICAFF();
        }

        public void ReadMULTICAFF()
        {
            Title = DataMethods.readString(path, 0x0, 0x4);
            sectionHeaderLen = DataMethods.readInt32(path, 0x4);
            numSections = DataMethods.readInt32(path, 0x8);
            headerChecksum = DataMethods.readInt32(path, 0xC);
            num0x4Skips = DataMethods.readInt32(path, 0x10);
            sectionHeadersStart = 0x14 + (num0x4Skips * 0x4);
            dataStart = sectionHeadersStart + (numSections * sectionHeaderLen);

            sectionInfo = new SectionInfo[numSections];
            for (int i = sectionHeadersStart; i < sectionHeadersStart + (sectionHeaderLen * numSections); i += sectionHeaderLen)
            {
                sectionInfo[(i - sectionHeadersStart) / sectionHeaderLen].Checksum = DataMethods.readInt32(path, i);
                sectionInfo[(i - sectionHeadersStart) / sectionHeaderLen].Address = DataMethods.readInt32(path, i + 0x4);
                sectionInfo[(i - sectionHeadersStart) / sectionHeaderLen].Length = DataMethods.readInt32(path, i + 0x8);
            }
            DetermineDataSections();
            getDNBWNames();
        }

        public void DetermineDataSections()
        {
            for (int i = 0; i < numSections; i++)
            {
                string word = DataMethods.readString(path, sectionInfo[i].Address, 0x4);
                switch (word)
                {
                    case "CAFF":
                        {
                            caffs.Add(new CAFF(DataMethods.readDataSection(path, sectionInfo[i].Address, sectionInfo[i].Length), sectionInfo[i].Address));
                            break;
                        }
                    case "DNBW":
                        {
                            dnbws.Add(new DNBW(path, sectionInfo[i].Address, sectionInfo[i].Length));
                            break;
                        }
                }
            }
        }

        public void getDNBWNames()
        {
            dnbwNames = new string[0];
            for (int i = 0; i < dnbws.Count; i++)
            {
                dnbwNames = DataMethods.addToArray(dnbwNames, dnbws[i].name);
            }
            dnbwNames = DataMethods.sortArray(dnbwNames);
        }

        public int getCaffIndexBySymbol(string symbol)
        {
            for (int i = 0; i < caffs.Count; i++)
            {
                if (caffs[i].getSymbols().Contains(symbol))
                {
                    return i;
                }
            }
            return 0;
        }

        public int getDNBWIndexByName(string name)
        {
            for (int i = 0; i < dnbws.Count; i++)
            {
                if (dnbws[i].name == name)
                {
                    return i;
                }
            }
            return 0;
        }

        public byte[] readSectionCAFF(int caffIndex, int symbolID, int section) //section base 0
        {
            return caffs[caffIndex].readSectionData(symbolID, section);
        }

        public byte[][] readSectionsCAFF(int caffIndex, int symbolID) //section base 0
        {
            return caffs[caffIndex].readSectionsData(symbolID);
        }
    }
}
