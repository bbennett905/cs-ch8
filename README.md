# cs-ch8 #
A Chip-8 interpreter/emulator written in C#, using .NET and Windows Forms.

*From [Wikipedia](https://en.wikipedia.org/wiki/CHIP-8)*

> CHIP-8 is an interpreted programming language, developed by Joseph Weisbecker. It was initially used on the COSMAC VIP and Telmac 1800 8-bit microcomputers in the mid-1970s. CHIP-8 programs are run on a CHIP-8 virtual machine. It was made to allow video games to be more easily programmed for said computers.

## Links ##

[Chip-8 Specifications and Instruction set](http://devernay.free.fr/hacks/chip8/C8TECH10.HTM): Has an explanation of the Chip-8's memory layout, registers, input, display, timers, and CPU opcodes.

[Chip-8 Public Domain ROMs](https://www.zophar.net/pdroms/chip8.html): Some free ROMs to use with this emulator.

## Known Issues ##

Poor performance after initially loading a ROM: This can typically be resolved by reloading any ROM once or twice. The suspected culprit is likely something that is messing with the C# JIT optimizer, but that isn't conclusive.
