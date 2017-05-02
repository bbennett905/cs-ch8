using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csch8
{
    class Emulator
    {
        //0x000-0x1FF (0000 - 0511): Unused or font data
        //0x200-0xE9F (0512 - 3743): Program/ROM
        //0xEA0-0xEFF (3744 - 3839): Call stack/ other internal use
        //0xF00-0xFFF (3840 - 4095): Display Refresh (1bit/px, 64x32)
        private static byte[] memory = new byte[4096];

        //16, 8 bit data registers: V0-VF. VF may be a flag register for some instructions
        private static byte[] registers = new byte[16];

        //16bit address register, called I
        private static ushort addressRegister;

        //16bit PC
        private static ushort programCounter;

        //Array holding graphics data: 1 = black, 0 = white
        //Converted to SDL_Point array in renderer
        //NOTE: this should be located in memory, leave it as this for the moment
        //private static byte[] pixelState = new byte[64 * 32];

        //Timers count down at 60Hz
        private static byte delayTimer;
        private static byte soundTimer;

        //Stack and stack pointer
        private static ushort[] stack = new ushort[16];
        private static ushort[] sp;

        //Keys input, 0x0-0xF
        private static byte[] keys = new byte[16];

        public Emulator(string filename)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(filename);
            sr.Close();
        }
    }
}
