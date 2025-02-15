using System;

namespace NesEmulatorAvaloniaTest.NES.OpCodes;

public struct OpCode
{
    public byte Code;
    public required string Mnemonic;
    public byte Length;
    public byte Cycles;
    public AddressingMode.AddressingMode AddressingMode;
}

