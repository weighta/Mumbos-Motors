using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mumbos_Motors.ModdingInfo
{
    public class sound : ModdingTab
    {
        string soundName;
        MULTICAFF multiCAFF;
        int DNBWIndex;
        byte[][] sectionData;

        public sound(string soundName, MULTICAFF multiCAFF, byte[][] sectionData) : base (sectionData, soundName + ".xwb")
        {
            this.soundName = soundName;
            this.multiCAFF = multiCAFF;
            this.sectionData = sectionData;
            DNBWIndex = multiCAFF.getDNBWIndexByName(soundName);

            buildMetaPage();
        }
        public override void buildMetaPage()
        {
            bottomToolBar.addButton("Import Sound");
            bottomToolBar.buttons[bottomToolBar.buttons.Count - 1].Click += new EventHandler(import_sound);
            bottomToolBar.buttons[0].Click += new EventHandler(save);
        }

        void import_sound(object sender, EventArgs e)
        {
            byte[] data = DataMethods.openFileDialog(".wav files (16 bit PCM)");
            if (data.Length != 0)
            {
                byte[] samples = new byte[data.Length - 0x104];
                for (int i = 0x40; i < data.Length - 0xC4; i++)
                {
                    samples[i - 0x40] = data[i];
                }
                if (samples.Length < multiCAFF.dnbws[DNBWIndex].len && samples.Length % 2 == 0)
                {
                    samples = DataMethods.swapEndianness(samples, 2);
                    for (int i = 0x9C0; i < (samples.Length - 0x9C0); i++)
                    {
                        hxd.sectionData[0][i] = samples[i - 0x9C0];
                    }
                    metaTab.metaChanged[0] = true;
                    MessageBox.Show("Sound imported successfully. Don't forget to save it :)\n\n");
                    //Status: samples.Length + " < " + multiCAFF.dnbws[DNBWIndex].name + "   " + DNBWIndex
                }
                else
                {
                    MessageBox.Show(".wav too big");
                }
            }
        }

        void save(object sender, EventArgs e)
        {
            DataMethods.writeDataSection(multiCAFF.path, multiCAFF.dnbws[DNBWIndex].offs, hxd.sectionData[0].Length, hxd.sectionData[0]);
            HexInfo.HexEditor.SaySaved();
        }
    }
}
