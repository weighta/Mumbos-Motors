using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Mumbos_Motors.FileTab.FileInfo
{
    public class InfoMULTICAFF : FileInfoPage
    {
        string dir;
        MULTICAFF multiCAFF;

        public InfoMULTICAFF(string dir, MULTICAFF multiCAFF) : base(dir)
        {
            this.dir = dir;
            this.multiCAFF = multiCAFF;
            Background.BackColor = Color.MediumPurple;

            labels();
        }
        public override void labels()
        {
            infoLabels.Add(newLabel("Title: " + multiCAFF.Title));
            infoLabels.Add(newLabel("Header Size: 0x" + multiCAFF.sectionHeadersStart.ToString("X")));
            infoLabels.Add(newLabel("Section's Header Size: " + multiCAFF.sectionHeaderLen.ToString("X")));
            infoLabels.Add(newLabel("Header Checksum: " + multiCAFF.headerChecksum.ToString("X8")));
            infoLabels.Add(newLabel("# of Sections: " + multiCAFF.numSections.ToString("X")));
            infoLabels.Add(newLabel("# of 0x4 skips: " + multiCAFF.num0x4Skips.ToString("X")));
            infoLabels.Add(newLabel("Data Start: " + multiCAFF.dataStart.ToString("X")));
            //infoLabels.Add(newLabel("Version: " + caff.getVersion()));
            //infoLabels.Add(newLabel("Header Size: 0x" + caff.getSizeOfHeader().ToString("X")));
            //infoLabels.Add(newLabel("Header CheckSum: " + caff.getHeaderChecksum().ToString("X8")));
            //infoLabels.Add(newLabel("# of Sections: " + caff.getNumberOfSections()));
            //infoLabels.Add(newLabel("# of Symbols: " + caff.getNumberOfSymbols()));
            //infoLabels.Add(newLabel("# of Fileparts: " + caff.getNumberOfFileParts()));
            //infoLabels.Add(newLabel("Type: " + caff.getType()));
            //infoLabels.Add(newLabel("Symbols Start: 0x" + caff.getSymbolsStart().ToString("X")));
            //infoLabels.Add(newLabel("FileInfos Start: 0x" + caff.fileInfosStart.ToString("X")));
            //infoLabels.Add(newLabel("Data Start: 0x" + caff.getDataStart().ToString("X")));
        }
    }
}
