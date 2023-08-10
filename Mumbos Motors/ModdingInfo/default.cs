using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mumbos_Motors
{
    public class Default : ModdingTab
    {
        public Default(CAFF caff, int fileID) : base(caff, fileID)
        {
            buildMetaPage();
        }

        public Default(MULTICAFF multiCaff, int caffIndex, int symbolID) : base(multiCaff, caffIndex, symbolID)
        {
            buildMetaPage();
        }

        public override void buildMetaPage()
        {
            metaTab.buildMetaFunPanel();
        }
    }
}
