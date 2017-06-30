using System;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;

namespace csch8
{
    class Emulator
    {
        private static readonly byte[] fontset =
        {
            0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
            0x20, 0x60, 0x20, 0x20, 0x70, // 1
            0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
            0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
            0x90, 0x90, 0xF0, 0x10, 0x10, // 4
            0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
            0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
            0xF0, 0x10, 0x20, 0x40, 0x40, // 7
            0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
            0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
            0xF0, 0x90, 0xF0, 0x90, 0x90, // A
            0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
            0xF0, 0x80, 0x80, 0x80, 0xF0, // C
            0xE0, 0x90, 0x90, 0x90, 0xE0, // D
            0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
            0xF0, 0x80, 0xF0, 0x80, 0x80  // F
        };

        //0x000-0x1FF (0000 - 0511): Unused or font data (each font sprite takes up 40 bits (5 bytes))
        //0x200-0xE9F (0512 - 3743): Program/ROM
        //0xEA0-0xEFF (3744 - 3839): Call stack/ other internal use
        //0xF00-0xFFF (3840 - 4095): Display Refresh (1bit/px, 64x32)
        public byte[] Memory { get; private set; } = new byte[4096];

        public bool[,] Display { get; private set; } = new bool[64, 32];

        //16, 8 bit data Registers: V0-VF. VF may be a flag register for some instructions
        public byte[] Registers { get; private set; } = new byte[16];

        //16bit address register, called I
        public ushort AddressRegister { get; private set; }
       
        //16bit PC
        public ushort ProgramCounter { get; private set; }

        public ushort CurrentOpcode
        {
            get { return (ushort)(Memory[ProgramCounter] << 8 | Memory[ProgramCounter + 1]); }
        }

        public bool Paused = false;

        //Timers count down at 60Hz
        public byte DelayTimer { get; private set; }
        public byte SoundTimer { get; private set; }

        //Stack and stack pointer
        private ushort[] stack = new ushort[16];
        private ushort stackPointer;

        //Keys input, 0x0-0xF
        public byte[] Keys = new byte[16];

        public bool DynamicRecompiler { get; set; }

        private Random random;

        //Stores last 10 PC values for debugging purposes
        public Queue<ushort> History { get; private set; }

        public Emulator(string filename, bool dynaRec = false)
        {
            History = new Queue<ushort>(20);
            DynamicRecompiler = dynaRec;
            System.IO.FileStream fs = File.Open(filename, FileMode.Open);

            if (fs.Length > 3231)
            {
                fs.Close();
                throw new FileLoadException("Invalid file size, may not be a Chip-8 ROM");
            }
            System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
            Array.Copy(br.ReadBytes((int)fs.Length), 0,  Memory, 512, fs.Length);
            
            br.Close();
            fs.Close();

            ProgramCounter = 512;
            random = new Random();

            Array.Copy(fontset, 0, Memory, 0, fontset.Length);
        }

        public void RunCycle()
        {
            if (Paused)
            {
                return;
            }
            if (DynamicRecompiler)
            {
                throw new NotImplementedException("Dynamic recompilation is not yet available");
            }
            else
            {
                ushort opcode = (ushort)(Memory[ProgramCounter] << 8 | Memory[ProgramCounter + 1]);
                if (History.Count >= 20)
                {
                    History.Dequeue();
                }
                History.Enqueue(ProgramCounter);

                //for most opcodes, increment pc by 2
                switch (opcode & 0xF000)
                {
                    //3 instructions of format 0x0NNN
                    case 0x0000:
                        switch (opcode & 0x00FF)
                        {
                            //Display clear, set all disp buffer values to 0
                            case 0x00E0:
                                for (int i = 0; i < 64; i++)
                                {
                                    for (int j = 0; j < 32; j++)
                                    {
                                        Display[i, j] = false;
                                    }
                                }
                                ProgramCounter += 2;
                                break;

                            //Return, end subroutine
                            case 0x00EE:
                                stackPointer--;
                                ProgramCounter = stack[stackPointer];
                                ProgramCounter += 2;
                                break;

                            //Calls RCA1802 program at NNN (0x0NNN), rarely used
                            default:
                                ProgramCounter += 2;
                                throw new NotImplementedException($"Opcode {opcode:X2} at location {ProgramCounter - 2:X2} has not yet been implemented");
                                break;
                        }
                        break;

                    //Jump to address at NNN (0x1NNN)
                    case 0x1000:
                        ProgramCounter = (ushort)(opcode & 0x0FFF);
                        break;

                    //Call subroutine at NNN (0x2NNN)
                    case 0x2000:
                        stack[stackPointer] = ProgramCounter;
                        stackPointer++;
                        ProgramCounter = (ushort)(opcode & 0x0FFF);
                        break;

                    //Skip next instruction if VX == NN (0x3XNN)
                    case 0x3000:
                        if (Registers[(opcode & 0x0F00) >> 8] == (opcode & 0x00FF))
                        {
                            ProgramCounter += 4;
                        }
                        else
                        {
                            ProgramCounter += 2;
                        }
                        break;

                    //Skip next instruction if VX != NN (0x4XNN)
                    case 0x4000:
                        if (Registers[(opcode & 0x0F00) >> 8] != (opcode & 0x00FF))
                        {
                            ProgramCounter += 4;
                        }
                        else
                        {
                            ProgramCounter += 2;
                        }
                        break;

                    //Skip next instruction if VX == VY (0x5XY0)
                    case 0x5000:
                        if (Registers[(opcode & 0x0F00) >> 8] == Registers[(opcode & 0x00F0) >> 4])
                        {
                            ProgramCounter += 4;
                        }
                        else
                        {
                            ProgramCounter += 2;
                        }
                        break;

                    //Sets VX = NN (0x6XNN)
                    case 0x6000:
                        Registers[(opcode & 0x0F00) >> 8] = (byte)(opcode & 0x00FF);
                        ProgramCounter += 2;
                        break;

                    //Increments VX by NN (0x7XNN)
                    case 0x7000:
                        Registers[(opcode & 0x0F00) >> 8] += (byte)(opcode & 0x00FF);
                        ProgramCounter += 2;
                        break;

                    //9 instructions of format 0x8000
                    case 0x8000:
                        switch (opcode & 0x000F)
                        {
                            //Sets VX = VY (0x8XY0)
                            case 0x0000:
                                Registers[(opcode & 0x0F00) >> 8] = Registers[(opcode & 0x00F0) >> 4];
                                ProgramCounter += 2;
                                break;

                            //Sets VX = VX | VY (0x8XY1), set VF to 0
                            case 0x0001:
                                Registers[(opcode & 0x0F00) >> 8] |= Registers[(opcode & 0x00F0) >> 4];
                                Registers[0xF] = 0;
                                ProgramCounter += 2;
                                break;

                            //Sets VX = VX & VY (0x8XY2), set VF to 0
                            case 0x0002:
                                Registers[(opcode & 0x0F00) >> 8] &= Registers[(opcode & 0x00F0) >> 4];
                                Registers[0xF] = 0;
                                ProgramCounter += 2;
                                break;

                            //Sets VX = VX ^ VY (0x8XY3), set VF to 0
                            case 0x0003:
                                Registers[(opcode & 0x0F00) >> 8] ^= Registers[(opcode & 0x00F0) >> 4];
                                Registers[0xF] = 0;
                                ProgramCounter += 2;
                                break;

                            //Increments VX by VY, sets VF to 1 if carry and 0 if not (0x8XY4)
                            case 0x0004:
                                Registers[0xF] = ((Registers[(opcode & 0x0F00) >> 8] + Registers[(opcode & 0x00F0) >> 4]) > Byte.MaxValue) ? (byte)1 : (byte)0;
                                Registers[(opcode & 0x0F00) >> 8] += Registers[(opcode & 0x00F0) >> 4];
                                ProgramCounter += 2;
                                break;

                            //Decrements VX by VY, sets VF to 1 if borrow and 0 if not (0x8XY5)
                            case 0x0005:
                                Registers[0xF] = ((Registers[(opcode & 0x0F00) >> 8] - Registers[(opcode & 0x00F0) >> 4]) < Byte.MinValue) ? (byte)1 : (byte)0;
                                Registers[(opcode & 0x0F00) >> 8] -= Registers[(opcode & 0x00F0) >> 4];
                                ProgramCounter += 2;
                                break;

                            //Shifts VX right by 1. VF is set to the bit shifted out (0x8X06)
                            case 0x0006:
                                Registers[0xF] = (byte)(Registers[(opcode & 0x0F00) >> 8] & 0x01);
                                Registers[(opcode & 0x0F00) >> 8] = (byte)(Registers[(opcode & 0x0F00) >> 8] >> 1);
                                ProgramCounter += 2;
                                break;

                            //Sets VX = VY - VX. VF set to 1 if borrow and 0 if not (0x8X07)
                            case 0x0007:
                                Registers[0xF] = (Registers[(opcode & 0x00F0) >> 4] - (Registers[(opcode & 0x0F00) >> 8]) < Byte.MinValue) ? (byte)1 : (byte)0;
                                Registers[(opcode & 0x0F00) >> 8] = (byte)(Registers[(opcode & 0x00F0) >> 4] - Registers[(opcode & 0x0F00) >> 8]);
                                ProgramCounter += 2;
                                break;

                            //Shifts VX left by 1. VF is set to the bit shifted out (0x8X0E)
                            case 0x000E:
                                Registers[0xF] = (byte)(Registers[(opcode & 0x0F00) >> 8] & 0x80);
                                Registers[(opcode & 0x0F00) >> 8] = (byte)(Registers[(opcode & 0x0F00) >> 8] << 1);
                                ProgramCounter += 2;
                                break;

                            default:
                                ProgramCounter += 2;
                                throw new InvalidOperationException($"Unrecognized opcode {opcode:X2} at location {ProgramCounter - 2:X2}");
                        }
                        break;

                    //Skip next instruction if VX != VY (0x9XY0)
                    case 0x9000:
                        if (Registers[(opcode & 0x0F00) >> 8] != Registers[(opcode & 0x00F0) >> 4])
                        {
                            ProgramCounter += 4;
                        }
                        else
                        {
                            ProgramCounter += 2;
                        }
                        break;

                    //Sets addressRegister (I) to the address NNN (0xANNN)
                    case 0xA000:
                        AddressRegister = (ushort)(opcode & 0x0FFF);
                        ProgramCounter += 2;
                        break;

                    //Jumps to the address NNN + V0 (0xBNNN)
                    case 0xB000:
                        ProgramCounter = (ushort)((opcode & 0x0FFF) + Registers[0]);
                        break;

                    //Sets VX = rand() & NN (0xCXNN)
                    case 0xC000:
                        Registers[(opcode & 0x0F00) >> 8] = (byte)((opcode & 0x00FF) & random.Next(Byte.MinValue, Byte.MaxValue + 1));
                        ProgramCounter += 2;
                        break;

                    //Draws a sprite at coordinate (VX, VY) with a width of 8 and height of N px, starting at Memory location I. (Use XOR)s
                    //Set VF to 1 if any pixels are flipped from set to unset, otherwise 0. (0xDXYN)
                    case 0xD000:
                        //0xF00-0xFFF (3840 - 4095): Display Refresh (1bit/px, 64x32)
                        byte x = Registers[(opcode & 0x0F00) >> 8];
                        byte y = Registers[(opcode & 0x00F0) >> 4];

                        byte h = (byte)(opcode & 0x000F);

                        Registers[0xF] = 0;

                        for (int row = 0; row < h; row++)
                        {
                            for (int col = 0; col < 8; col++)
                            {
                                int xPos = x + col;
                                int yPos = y + row;
                                if (xPos >= 64)
                                    continue;

                                //first check if we're supposed to draw a bit here
                                if ((Memory[(AddressRegister + row)] & (0x80 >> col)) != 0)
                                {
                                    if (Display[xPos, yPos])
                                    {
                                        Registers[0xF] = 1;
                                    }
                                    Display[xPos, yPos] ^= true;
                                }
                            }
                        }

                        ProgramCounter += 2;
                        break;

                    //2 instructions of format 0xE000
                    case 0xE000:
                        switch (opcode & 0x00FF)
                        {
                            //Skip next instruction if key stored in VX is pressed (0xEX9E)
                            case 0x009E:
                                if (Keys[Registers[(opcode & 0x0F00) >> 8]] == 1)
                                {
                                    ProgramCounter += 4;
                                }
                                else
                                {
                                    ProgramCounter += 2;
                                }
                                break;

                            //Skip next instruction if key stored in VX isn't pressed (0xEXA1)
                            case 0x00A1:
                                if (Keys[Registers[(opcode & 0x0F00) >> 8]] == 0)
                                {
                                    ProgramCounter += 4;
                                }
                                else
                                {
                                    ProgramCounter += 2;
                                }
                                break;

                            default:
                                ProgramCounter += 2;
                                throw new InvalidOperationException($"Unrecognized opcode {opcode:X2} at location {ProgramCounter - 2:X2}");
                        }
                        break;

                    //9 instructions of format 0xF000
                    case 0xF000:
                        switch (opcode & 0x00FF)
                        {
                            //Sets VX to the value of the delay timer (0xFX07)
                            case 0x0007:
                                Registers[(opcode & 0x0F00) >> 8] = DelayTimer;
                                ProgramCounter += 2;
                                break;

                            //Halts until key press, which is then stored in VX (0xFX0A)
                            case 0x000A:
                                //Don't increment program counter, unless a key is pressed
                                for (byte i = 0; i < Keys.Length; i++)
                                {
                                    if (Keys[i] == 1)
                                    {
                                        Registers[(opcode & 0x0F00) >> 8] = i;
                                        ProgramCounter += 2;
                                        break;
                                    }
                                }
                                break;

                            //Sets the delay timer equal to VX (0xFX15)
                            case 0x0015:
                                DelayTimer = Registers[(opcode & 0x0F00) >> 8];
                                ProgramCounter += 2;
                                break;

                            //Sets the sound timer equal to VX (0xFX18)
                            case 0x0018:
                                SoundTimer = Registers[(opcode & 0x0F00) >> 8];
                                ProgramCounter += 2;
                                break;

                            //Increments I (address register) by VX (0xFX1E)
                            case 0x001E:
                                AddressRegister += Registers[(opcode & 0x0F00) >> 8];
                                ProgramCounter += 2;
                                break;

                            //Sets I (address register) to the location of the sprite for the character in VX; 8x5 font characters 0-F (0xFX29)
                            case 0x0029:
                                //0x000-0x1FF (0000 - 0511): Unused or font data (each font sprite takes up 40 bits (5 bytes))
                                AddressRegister = (ushort)(5 * Registers[(opcode & 0x0F00) >> 8]);
                                ProgramCounter += 2;
                                break;

                            //Store the BCD representation of VX at I, I+1, I+2 (0xFX33)
                            case 0x0033:
                                Memory[AddressRegister] = (byte)(Registers[(opcode & 0x0F00) >> 8] / 100);
                                Memory[AddressRegister + 1] = (byte)((Registers[(opcode & 0x0F00) >> 8] / 10) % 10);
                                Memory[AddressRegister + 2] = (byte)((Registers[(opcode & 0x0F00) >> 8] % 100) % 10);
                                ProgramCounter += 2;
                                break;

                            //Stores V0-VX (inclusive) in Memory starting at address I (0xFX55)
                            case 0x0055:
                                for (int reg = 0; reg <= ((opcode & 0x0F00) >> 8); reg++)
                                {
                                    Memory[AddressRegister + reg] = Registers[reg];
                                }
                                ProgramCounter += 2;
                                break;

                            //Loads Memory starting at address I into Registers V0-VX (inclusive) (0xFX65)
                            case 0x0065:
                                for (int reg = 0; reg <= ((opcode & 0x0F00) >> 8); reg++)
                                {
                                    Registers[reg] = Memory[AddressRegister + reg];
                                }
                                ProgramCounter += 2;
                                break;

                            default:
                                ProgramCounter += 2;
                                throw new InvalidOperationException($"Unrecognized opcode {opcode:X2} at location {ProgramCounter - 2:X2}");
                        }
                        break;

                    default:
                        ProgramCounter += 2;
                        throw new InvalidOperationException($"Unrecognized opcode {opcode:X2} at location {ProgramCounter - 2:X2}");
                }
                if (DelayTimer > 0)
                {
                    DelayTimer--;
                }

                if (SoundTimer > 0)
                {
                    if (SoundTimer == 1)
                    {
                        SystemSounds.Beep.Play();
                    }
                    SoundTimer--;
                }
            }
        }
        
    }
}
