using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mumbos_Motors.FileTab.FileInfo
{
    public class InfoCAFF : FileInfoPage
    {
        string dir;
        public CAFF caff;

        public InfoCAFF(string dir, CAFF caff) : base(dir)
        {
            this.dir = dir;
            this.caff = caff;
            labels();
        }
        public override void labels()
        {
            infoLabels.Add(newLabel("Version: " + caff.getVersion()));
            infoLabels.Add(newLabel("Header Size: 0x" + caff.getSizeOfHeader().ToString("X")));
            infoLabels.Add(newLabel("Header CheckSum: " + caff.getHeaderChecksum().ToString("X8")));
            infoLabels.Add(newLabel("# of Sections: " + caff.getNumberOfSections()));
            infoLabels.Add(newLabel("# of Symbols: " + caff.getNumberOfSymbols()));
            infoLabels.Add(newLabel("# of Fileparts: " + caff.getNumberOfFileParts()));
            infoLabels.Add(newLabel("Type: " + caff.getType()));
            infoLabels.Add(newLabel("Symbols Start: 0x" + caff.getSymbolsStart().ToString("X")));
            infoLabels.Add(newLabel("FileInfos Start: 0x" + caff.fileInfosStart.ToString("X")));
            infoLabels.Add(newLabel("Data Start: 0x" + caff.getDataStart().ToString("X")));
        }
    }
}
