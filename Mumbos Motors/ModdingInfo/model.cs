using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mumbos_Motors
{
    //top
    //0x00  data tables
    //0x04  num data tables
    
    //data table
    //0x00  data table id
    //0x04  data table offs


    class model : ModdingTab
    {
        public model(CAFF caff, int fileID) : base(caff, fileID)
        {
            buildMetaPage();
        }

        public model(MULTICAFF multiCaff, int caffIndex, int symbolID) : base(multiCaff, caffIndex, symbolID)
        {
            buildMetaPage();
        }

        public override void buildMetaPage()
        {

        }
    }
}
