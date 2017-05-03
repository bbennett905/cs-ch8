using System;
using System.IO;
using System.Text;

namespace csch8
{
    class Emulator
    {
        //0x000-0x1FF (0000 - 0511): Unused or font data
        //0x200-0xE9F (0512 - 3743): Program/ROM
        //0xEA0-0xEFF (3744 - 3839): Call stack/ other internal use
        //0xF00-0xFFF (3840 - 4095): Display Refresh (1bit/px, 64x32)
        private byte[] memory = new byte[4096];

        //16, 8 bit data registers: V0-VF. VF may be a flag register for some instructions
        private byte[] registers = new byte[16];

        //16bit address register, called I
        private ushort addressRegister;

        //16bit PC
        private ushort programCounter;

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
        }

        public void RunCycle()
        {
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
                                throw new NotImplementedException($"Opcode {opcode:X2} at location {programCounter:X2} has not yet been implemented");
                                break;

                            //Calls RCA1802 program at NNN (0x0NNN), rarely used
                            default:
                                throw new NotImplementedException($"Opcode {opcode:X2} at location {programCounter:X2} has not yet been implemented");
                                break;
                        }
                        break;

                    //Jump to address at NNN (0x1NNN)
                    case 0x1000:
                        programCounter = (ushort)(opcode & 0x0FFF);
                        break;

                    //Call subroutine at NNN (0x2NNN)
                    case 0x2000:
                        throw new NotImplementedException($"Opcode {opcode:X2} at location {programCounter:X2} has not yet been implemented");
                        break;

                    //Skip next instruction if VX == NN (0x3XNN)
                    case 0x3000:
                        throw new NotImplementedException($"Opcode {opcode:X2} at location {programCounter:X2} has not yet been implemented");
                        break;

                    //Skip next instruction if VX != NN (0x4XNN)
                    case 0x4000:
                        throw new NotImplementedException($"Opcode {opcode:X2} at location {programCounter:X2} has not yet been implemented");
                        break;

                    //Skip next instruction if VX == VY (0x5XY0)
                    case 0x5000:
                        throw new NotImplementedException($"Opcode {opcode:X2} at location {programCounter:X2} has not yet been implemented");
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
                                throw new InvalidOperationException($"Unrecognized opcode {opcode:X2} at location {programCounter:X2}");
                        }
                        break;

                    //Skip next instruction if VX != VY (0x9XY0)
                    case 0x9000:
                        throw new NotImplementedException($"Opcode {opcode:X2} at location {programCounter:X2} has not yet been implemented");
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
                        throw new NotImplementedException($"Opcode {opcode:X2} at location {programCounter:X2} has not yet been implemented");
                        break;

                    //Draws a sprite at coordinate (VX, VY) with a width of 8 and height of N px, starting at memory location I. (Use XOR)s
                    //Set VF to 1 if any pixels are flipped from set to unset, otherwise 0. (0xDXYN)
                    case 0xD000:
                        throw new NotImplementedException($"Opcode {opcode:X2} at location {programCounter:X2} has not yet been implemented");
                        break;

                    //2 instructions of format 0xE000
                    case 0xE000:
                        switch (opcode & 0x00FF)
                        {
                            //Skip next instruction if key stored in VX is pressed (0xEX9E)
                            case 0x009E:
                                throw new NotImplementedException($"Opcode {opcode:X2} at location {programCounter:X2} has not yet been implemented");
                                break;

                            //Skip next instruction if key stored in VX isn't pressed (0xEXA1)
                            case 0x00A1:
                                throw new NotImplementedException($"Opcode {opcode:X2} at location {programCounter:X2} has not yet been implemented");
                                break;

                            default:
                                throw new InvalidOperationException($"Unrecognized opcode {opcode:X2} at location {programCounter:X2}");
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
                                throw new NotImplementedException($"Opcode {opcode:X2} at location {programCounter:X2} has not yet been implemented");
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
                                throw new NotImplementedException($"Opcode {opcode:X2} at location {programCounter:X2} has not yet been implemented");
                                break;

                            //Store the BCD representation of VX at I, I+1, I+2 (0xFX33)
                            case 0x0033:
                                throw new NotImplementedException($"Opcode {opcode:X2} at location {programCounter:X2} has not yet been implemented");
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
                                throw new InvalidOperationException($"Unrecognized opcode {opcode:X2} at location {programCounter:X2}");
                        }
                        break;

                    default:
                        throw new InvalidOperationException($"Unrecognized opcode {opcode:X2} at location {programCounter:X2}");
                }
            }
        }
        
    }
}
