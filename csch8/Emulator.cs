using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

namespace csch8
{
    class Emulator
    {
        private static readonly byte[] fontset =
        {
            0xF9, 0x99, 0xF2, // 0
            0x62, 0x27, // 1
            0xF1, 0xF8, 0xFF, // 2
            0x1F, 0x1F, // 3
            0x99, 0xF1, 0x1F, // 4
            0x8F, 0x1F, // 5
            0xF8, 0xF9, 0xFF, // 6
            0x12, 0x44, // 7
            0xF9, 0xF9, 0xFF, // 8
            0x9F, 0x1F, // 9
            0xF9, 0xF9, 0x9E, // A
            0x9E, 0x9E, // B
            0xF8, 0x88, 0xFE, // C
            0x99, 0x9E, // D
            0xF8, 0xF8, 0xFF, // E
            0x8F, 0x88  // F
        };

        //0x000-0x1FF (0000 - 0511): Unused or font data (each font sprite takes up 40 bits (5 bytes))
        //0x200-0xE9F (0512 - 3743): Program/ROM
        //0xEA0-0xEFF (3744 - 3839): Call stack/ other internal use
        //0xF00-0xFFF (3840 - 4095): Display Refresh (1bit/px, 64x32)
        public byte[] memory = new byte[4096];

        //16, 8 bit data registers: V0-VF. VF may be a flag register for some instructions
        private byte[] registers = new byte[16];

        //16bit address register, called I
        private ushort addressRegister;
       
        //16bit PC
        private ushort programCounter;
        public ushort ProgramCounter
        {
            get { return programCounter; }
        }

        public ushort CurrentOpcode
        {
            get { return (ushort)(memory[programCounter] << 8 | memory[programCounter + 1]); }
        }

        public bool Paused = false;
        //Array holding graphics data: 1 = black, 0 = white
        //Converted to SDL_Point array in renderer
        //NOTE: this should be located in memory, leave it as this for the moment
        //private static byte[] pixelState = new byte[64 * 32];

        //Timers count down at 60Hz
        private byte delayTimer;
        private byte soundTimer;

        //Stack and stack pointer
        private ushort[] stack = new ushort[16];
        private ushort stackPointer;

        //Keys input, 0x0-0xF
        private byte[] keys = new byte[16];

        private bool dynamicRecompiler;
        public bool DynamicRecompiler
        {
            get { return dynamicRecompiler; }
            set { dynamicRecompiler = value; }
        }

        private Random random;

        //
        private Stack<ushort> history;

        public Emulator(string filename, bool dynaRec = false)
        {
            dynamicRecompiler = dynaRec;
            System.IO.FileStream fs = File.Open(filename, FileMode.Open);

            if (fs.Length > 3231)
            {
                fs.Close();
                throw new FileLoadException("Invalid file size, may not be a Chip-8 ROM");
            }
            System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
            Array.Copy(br.ReadBytes((int)fs.Length), 0,  memory, 512, fs.Length);
            
            br.Close();
            fs.Close();

            programCounter = 512;
            random = new Random();

            Array.Copy(fontset, 0, memory, 0, fontset.Length);
        }

        public void RunCycle()
        {
            if (Paused)
            {
                return;
            }
            if (dynamicRecompiler)
            {
                throw new NotImplementedException("Dynamic recompilation is not yet available");
            }
            else
            {
                ushort opcode = (ushort)(memory[programCounter] << 8 | memory[programCounter + 1]);

                //for most opcodes, increment pc by 2
                switch (opcode & 0xF000)
                {
                    //3 instructions of format 0x0NNN
                    case 0x0000:
                        switch (opcode & 0x0FF)
                        {
                            //Display clear, set all disp buffer values to 0
                            case 0x00E0:
                                //0xF00-0xFFF (3840 - 4095): Display Refresh (1bit/px, 64x32)
                                for (int i = 0xF00; i <= 0xFFF; i++)
                                {
                                    memory[i] = 0;
                                }
                                programCounter += 2;
                                break;

                            //Return, end subroutine
                            case 0x00EE:
                                programCounter = stack[stackPointer];
                                stackPointer--;
                                break;

                            //Calls RCA1802 program at NNN (0x0NNN), rarely used
                            default:
                                programCounter += 2;
                                throw new NotImplementedException($"Opcode {opcode:X2} at location {programCounter - 2:X2} has not yet been implemented");
                                break;
                        }
                        break;

                    //Jump to address at NNN (0x1NNN)
                    case 0x1000:
                        programCounter = (ushort)(opcode & 0x0FFF);
                        break;

                    //Call subroutine at NNN (0x2NNN)
                    case 0x2000:
                        stack[stackPointer] = programCounter;
                        stackPointer++;
                        programCounter = (ushort)(opcode & 0x0FFF);
                        break;

                    //Skip next instruction if VX == NN (0x3XNN)
                    case 0x3000:
                        if (registers[(opcode & 0x0F00) >> 8] == (opcode & 0x00FF))
                        {
                            programCounter += 4;
                        }
                        else
                        {
                            programCounter += 2;
                        }
                        break;

                    //Skip next instruction if VX != NN (0x4XNN)
                    case 0x4000:
                        if (registers[(opcode & 0x0F00) >> 8] != (opcode & 0x00FF))
                        {
                            programCounter += 4;
                        }
                        else
                        {
                            programCounter += 2;
                        }
                        break;

                    //Skip next instruction if VX == VY (0x5XY0)
                    case 0x5000:
                        if (registers[(opcode & 0x0F00) >> 8] == registers[(opcode & 0x00F0) >> 4])
                        {
                            programCounter += 4;
                        }
                        else
                        {
                            programCounter += 2;
                        }
                        break;

                    //Sets VX = NN (0x6XNN)
                    case 0x6000:
                        registers[(opcode & 0x0F00) >> 8] = (byte)(opcode & 0x00FF);
                        programCounter += 2;
                        break;

                    //Increments VX by NN (0x7XNN)
                    case 0x7000:
                        registers[(opcode & 0x0F00) >> 8] += (byte)(opcode & 0x00FF);
                        programCounter += 2;
                        break;

                    //9 instructions of format 0x8000
                    case 0x8000:
                        switch (opcode & 0x000F)
                        {
                            //Sets VX = VY (0x8XY0)
                            case 0x0000:
                                registers[(opcode & 0x0F00) >> 8] = registers[(opcode & 0x00F0) >> 4];
                                programCounter += 2;
                                break;

                            //Sets VX = VX | VY (0x8XY1), set VF to 0
                            case 0x0001:
                                registers[(opcode & 0x0F00) >> 8] |= registers[(opcode & 0x00F0) >> 4];
                                registers[0xF] = 0;
                                programCounter += 2;
                                break;

                            //Sets VX = VX & VY (0x8XY2), set VF to 0
                            case 0x0002:
                                registers[(opcode & 0x0F00) >> 8] &= registers[(opcode & 0x00F0) >> 4];
                                registers[0xF] = 0;
                                programCounter += 2;
                                break;

                            //Sets VX = VX ^ VY (0x8XY3), set VF to 0
                            case 0x0003:
                                registers[(opcode & 0x0F00) >> 8] ^= registers[(opcode & 0x00F0) >> 4];
                                registers[0xF] = 0;
                                programCounter += 2;
                                break;

                            //Increments VX by VY, sets VF to 1 if carry and 0 if not (0x8XY4)
                            case 0x0004:
                                registers[0xF] = ((registers[(opcode & 0x0F00) >> 8] + registers[(opcode & 0x00F0) >> 4]) > Byte.MaxValue) ? (byte)1 : (byte)0;
                                registers[(opcode & 0x0F00) >> 8] += registers[(opcode & 0x00F0) >> 4];
                                programCounter += 2;
                                break;

                            //Decrements VX by VY, sets VF to 1 if borrow and 0 if not (0x8XY5)
                            case 0x0005:
                                registers[0xF] = ((registers[(opcode & 0x0F00) >> 8] - registers[(opcode & 0x00F0) >> 4]) < Byte.MinValue) ? (byte)1 : (byte)0;
                                registers[(opcode & 0x0F00) >> 8] -= registers[(opcode & 0x00F0) >> 4];
                                programCounter += 2;
                                break;

                            //Shifts VX right by 1. VF is set to the bit shifted out (0x8X06)
                            case 0x0006:
                                registers[0xF] = (byte)(registers[(opcode & 0x0F00)] & 0x01);
                                registers[(opcode & 0x0F00) >> 8] = (byte)(registers[(opcode & 0x0F00)] >> 1);
                                programCounter += 2;
                                break;

                            //Sets VX = VY - VX. VF set to 1 if borrow and 0 if not (0x8X07)
                            case 0x0007:
                                registers[0xF] = (registers[(opcode & 0x00F0) >> 4] - (registers[(opcode & 0x0F00) >> 8]) < Byte.MinValue) ? (byte)1 : (byte)0;
                                registers[(opcode & 0x0F00) >> 8] = (byte)(registers[(opcode & 0x00F0) >> 4] - registers[(opcode & 0x0F00) >> 8]);
                                programCounter += 2;
                                break;

                            //Shifts VX left by 1. VF is set to the bit shifted out (0x8X0E)
                            case 0x000E:
                                registers[0xF] = (byte)(registers[(opcode & 0x0F00)] & 0x80);
                                registers[(opcode & 0x0F00) >> 8] = (byte)(registers[(opcode & 0x0F00)] << 1);
                                programCounter += 2;
                                break;

                            default:
                                programCounter += 2;
                                throw new InvalidOperationException($"Unrecognized opcode {opcode:X2} at location {programCounter - 2:X2}");
                        }
                        break;

                    //Skip next instruction if VX != VY (0x9XY0)
                    case 0x9000:
                        if (registers[(opcode & 0x0F00) >> 8] != registers[(opcode & 0x00F0) >> 4])
                        {
                            programCounter += 4;
                        }
                        else
                        {
                            programCounter += 2;
                        }
                        break;

                    //Sets addressRegister (I) to the address NNN (0xANNN)
                    case 0xA000:
                        addressRegister = (ushort)(opcode & 0x0FFF);
                        programCounter += 2;
                        break;

                    //Jumps to the address NNN + V0 (0xBNNN)
                    case 0xB000:
                        programCounter = (ushort)((opcode & 0x0FFF) + registers[0]);
                        break;

                    //Sets VX = rand() & NN (0xCXNN)
                    case 0xC000:
                        registers[(opcode & 0x0F00) >> 8] = (byte)((opcode & 0x00FF) & random.Next(Byte.MinValue, Byte.MaxValue + 1));
                        programCounter += 2;
                        break;

                    //Draws a sprite at coordinate (VX, VY) with a width of 8 and height of N px, starting at memory location I. (Use XOR)s
                    //Set VF to 1 if any pixels are flipped from set to unset, otherwise 0. (0xDXYN)
                    case 0xD000:
                        //0xF00-0xFFF (3840 - 4095): Display Refresh (1bit/px, 64x32)
                        byte x = registers[(opcode & 0x0F00) >> 8];
                        byte y = registers[(opcode & 0x00F0) >> 4];
                        byte h = (byte)(opcode & 0x000F);

                        registers[0xF] = 0;

                        for (int pos = 0; pos < 8 * h; pos++)
                        {
                            //first check if we're supposed to draw a bit here
                            if ((memory[addressRegister + (pos / 8)] & (1 << (8 - (pos % 8)))) == 1)
                            {
                                //pos is the number of BITS past 0xF00
                                //this might be totally wrong
                                if ((memory[0xF00 + (pos / 8)] & (1 << (8 - (pos % 8)))) == 1)
                                {
                                    registers[0xF] = 1;
                                }
                                memory[0xF00 + (pos / 8)] ^= (byte)(1 << (8 - (pos % 8)));
                            }
                        }
                        programCounter += 2;
                        break;

                    //2 instructions of format 0xE000
                    case 0xE000:
                        switch (opcode & 0x00FF)
                        {
                            //Skip next instruction if key stored in VX is pressed (0xEX9E)
                            case 0x009E:
                                if (keys[registers[(opcode & 0x0F00) >> 8]] == 1)
                                {
                                    programCounter += 4;
                                }
                                else
                                {
                                    programCounter += 2;
                                }
                                break;

                            //Skip next instruction if key stored in VX isn't pressed (0xEXA1)
                            case 0x00A1:
                                if (keys[registers[(opcode & 0x0F00) >> 8]] == 0)
                                {
                                    programCounter += 4;
                                }
                                else
                                {
                                    programCounter += 2;
                                }
                                break;

                            default:
                                programCounter += 2;
                                throw new InvalidOperationException($"Unrecognized opcode {opcode:X2} at location {programCounter - 2:X2}");
                        }
                        break;

                    //9 instructions of format 0xF000
                    case 0xF000:
                        switch (opcode & 0x00FF)
                        {
                            //Sets VX to the value of the delay timer (0xFX07)
                            case 0x0007:
                                registers[(opcode & 0x0F00) >> 8] = delayTimer;
                                programCounter += 2;
                                break;

                            //Halts until key press, which is then stored in VX (0xFX0A)
                            case 0x000A:
                                //Don't increment program counter, unless a key is pressed
                                for (byte i = 0; i < keys.Length; i++)
                                {
                                    if (keys[i] == 1)
                                    {
                                        registers[(opcode & 0x0F00) >> 8] = i;
                                        programCounter += 2;
                                        break;
                                    }
                                }
                                break;

                            //Sets the delay timer equal to VX (0xFX15)
                            case 0x0015:
                                delayTimer = registers[(opcode & 0x0F00) >> 8];
                                programCounter += 2;
                                break;

                            //Sets the sound timer equal to VX (0xFX18)
                            case 0x0018:
                                soundTimer = registers[(opcode & 0x0F00) >> 8];
                                programCounter += 2;
                                break;

                            //Increments I (address register) by VX (0xFX1E)
                            case 0x001E:
                                addressRegister += registers[(opcode & 0x0F00) >> 8];
                                programCounter += 2;
                                break;

                            //Sets I (address register) to the location of the sprite for the character in VX; 4x5 font characters 0-F (0xFX29)
                            case 0x0029:
                                //0x000-0x1FF (0000 - 0511): Unused or font data (each font sprite takes up 40 bits (5 bytes))
                                addressRegister = (ushort)(5 * registers[(opcode & 0x0F00) >> 8]);
                                programCounter += 2;
                                break;

                            //Store the BCD representation of VX at I, I+1, I+2 (0xFX33)
                            case 0x0033:
                                memory[addressRegister] = (byte)(registers[(opcode & 0x0F00) >> 8] / 100);
                                memory[addressRegister] = (byte)((registers[(opcode & 0x0F00) >> 8] / 10) % 10);
                                memory[addressRegister] = (byte)((registers[(opcode & 0x0F00) >> 8] % 100) % 10);
                                programCounter += 2;
                                break;

                            //Stores V0-VX (inclusive) in memory starting at address I (0xFX55)
                            case 0x0055:
                                for (int reg = 0; reg <= ((opcode & 0x0F00) >> 8); reg++)
                                {
                                    memory[addressRegister + reg] = registers[reg];
                                }
                                programCounter += 2;
                                break;

                            //Loads memory starting at address I into registers V0-VX (inclusive) (0xFX65)
                            case 0x0065:
                                for (int reg = 0; reg <= ((opcode & 0x0F00) >> 8); reg++)
                                {
                                    registers[reg] = memory[addressRegister + reg];
                                }
                                programCounter += 2;
                                break;

                            default:
                                programCounter += 2;
                                throw new InvalidOperationException($"Unrecognized opcode {opcode:X2} at location {programCounter - 2:X2}");
                        }
                        break;

                    default:
                        programCounter += 2;
                        throw new InvalidOperationException($"Unrecognized opcode {opcode:X2} at location {programCounter - 2:X2}");
                }
                if (delayTimer > 0)
                {
                    delayTimer--;
                }

                if (soundTimer > 0)
                {
                    if (soundTimer == 1)
                    {
                        MessageBox.Show("Beep");
                    }
                    soundTimer--;
                }
            }
        }
        
    }
}
