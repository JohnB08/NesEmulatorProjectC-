using System;
using System.Collections.Generic;
using System.Numerics;
using NesEmulatorAvaloniaTest.NES.OpCodes;

namespace NesEmulatorAvaloniaTest.NES.CPU;
using NesEmulatorAvaloniaTest.NES.CpuFlags;
using AddressingMode;

public class Cpu
{
    public static CpuFlags TruncateFromBits(byte bits)
    {
        var validMask = (byte)
        (
            CpuFlags.Carry |
            CpuFlags.Break2 |
            CpuFlags.Negative | 
            CpuFlags.Overflow | 
            CpuFlags.Negative | 
            CpuFlags.Carry | 
            CpuFlags.Break |
            CpuFlags.DecimalMode | 
            CpuFlags.InterruptDisable
        );
        return (CpuFlags)(bits & validMask);
    }

    public byte StackReset = 0xfd;
    public ushort Stack = 0x0100;
    public byte RegisterA = 0;
    public byte RegisterX = 0;
    public byte RegisterY = 0;
    public byte StackPointer;
    public CpuFlags Status = TruncateFromBits(0b100100);
    public ushort ProgramCounter = 0;
    public byte[] Memory = new byte[0xffff];
    public Dictionary<byte, OpCode> OpCodeMap = OpCodes.OpCodes.OpCodeMap; 

    public Cpu()
    {
        StackPointer = StackReset;
    }

    public byte MemRead(ushort address)
    {
        return Memory[address];
    }

    private ushort MemRead16(ushort address)
    {
        var lo = MemRead(address);
        var hi = MemRead((ushort)(address + 1));
        return (ushort)((hi << 8) | lo);
    }

    public void MemWrite(ushort address, byte value)
    {
        Memory[address] = value;
    }

    private void MemWrite16(ushort address, ushort value)
    {
        var hi = (byte)(value >> 8);
        var lo = (byte)(value & 0xff);
        MemWrite(address, lo);
        MemWrite((ushort)(address + 1), hi);
    }

    public void Reset()
    {
        RegisterA = 0;
        RegisterX = 0;
        Status = (CpuFlags)TruncateFromBits(0b100100);
        StackPointer = StackReset;
        ProgramCounter = MemRead16(0xfffc);
    }

    public void Load(byte[] data)
    {
        data.CopyTo(Memory, 0x0600);
        MemWrite16(0xFFFC, 0x0600);
    }

    public void Run()
    {
        RunWithCallback(_ => { });
    }

    public void RunWithCallback(Action<Cpu> callback)
    {
        while (true)
        {
            callback(this);
            var code = MemRead(ProgramCounter);
            ProgramCounter++;
            var ProgramCounterState = ProgramCounter;
            var opCode = OpCodeMap[code];
            if (code == 0x00) return;
            code switch
            {
                0xa9 or 0xa5 or 0xb5 or 0xad or 0xbd or 0xb9 or 0xa1 or 0xb1 => Lda(opCode.AddressingMode),
                0xAA => Tax(),
                0xE8 => Inx(),
                0xd8 => ClearCpuFlag(CpuFlags.DecimalMode),
                0x58 => ClearCpuFlag(CpuFlags.InterruptDisable),
                0xb8 => ClearCpuFlag(CpuFlags.Overflow),
                0x18 => ClearCpuFlag(CpuFlags.Carry),
                0x38 => InsertCpuFlag(CpuFlags.Carry),
                0x78 => InsertCpuFlag(CpuFlags.InterruptDisable),
                0xf8 => InsertCpuFlag(CpuFlags.DecimalMode),
                0x48 => StackPush(RegisterA),
                0x68 => Pla(),
                0x08 => Php(),
                0x28 => Plp(),
                0x69 or 0x65 or 0x75 or 0x6d or 0x79 or 0x61 or 0x71 => Adc(opCode.AddressingMode),
                0xe9 or 0xe5 or 0xf5 or 0xed or 0xfd or 0xf9 or 0xe1 or 0xf1 => Sbc(opCode.AddressingMode),
                
                
            }
        }
    }

    public void LoadAndRun(byte[] data)
    {
        Load(data);
        Reset();
        Run();
    }

    public ushort GetOperandAddress(AddressingMode addressingMode)
    {
        return addressingMode switch
        {
            AddressingMode.Immediate => ProgramCounter,
            AddressingMode.ZeroPage => MemRead(ProgramCounter),
            AddressingMode.Absolute => MemRead16(ProgramCounter),
            AddressingMode.ZeroPageX => WrappingAdd(MemRead(ProgramCounter), RegisterX),
            AddressingMode.ZeroPageY => WrappingAdd(MemRead(ProgramCounter), RegisterY),
            AddressingMode.AbsoluteX => WrappingAdd(MemRead16(ProgramCounter), RegisterX),
            AddressingMode.AbsoluteY => WrappingAdd(MemRead16(ProgramCounter), RegisterY),
            AddressingMode.IndirectX => IndirectX(),
            AddressingMode.IndirectY => IndirectY(),
            AddressingMode.NoneAddressing or _ => throw new NotImplementedException(),
        };
    }

    private void Lda(AddressingMode addressingMode)
    {
        var addr = GetOperandAddress(addressingMode);
        var val = MemRead(addr);
        RegisterA = val;
        /*UpdateZeroAndNegativeFlags();*/
    }

    private void InsertCpuFlag(CpuFlags flag)
    {
        Status |= flag;
    }

    private void ClearCpuFlag(CpuFlags flag)
    {
        Status &= ~flag;
    }

    private bool CpuFlagIsSet(CpuFlags flag)
    {
        return (Status & flag) == flag;
    }

    private void SetCpuFlag(CpuFlags flag, bool value)
    {
        if (value) InsertCpuFlag(flag);
        else ClearCpuFlag(flag);
    }


    private void AddToRegisterA(byte data)
    {
        var sum = (ushort)(RegisterA + data + (CpuFlagIsSet(CpuFlags.Carry) ? 1 : 0));
        var carry = sum > 0xFF;
        if (carry) InsertCpuFlag(CpuFlags.Carry);
        else ClearCpuFlag(CpuFlags.Carry);
        var result = (byte)sum;
        if (((data ^ result) & (result ^ RegisterA) & 0x89) != 0) InsertCpuFlag(CpuFlags.Overflow);
        else ClearCpuFlag(CpuFlags.Overflow);
        RegisterA = result;
        UpdateZeroAndNegativeFlags(RegisterA);
    }

    private void Sbc(AddressingMode addressingMode)
    {
        var addr = GetOperandAddress(addressingMode);
        var data = MemRead(addr);
        AddToRegisterA(WrappingSub<byte, sbyte>(data, 1));
    }

    private void Adc(AddressingMode addressingMode)
    {
        var addr = GetOperandAddress(addressingMode);
        var data = MemRead(addr);
        AddToRegisterA((byte)data);
    }

    private byte StackPop()
    {
        StackPointer = WrappingAdd<byte>(StackPointer, 1);
        return MemRead((ushort)(Stack + StackPointer));
    }

    private void StackPush(byte value)
    {
        MemWrite((ushort)(Stack + StackPointer), value);
        StackPointer = WrappingSub<byte, sbyte>(StackPointer, 1);
    }

    private void StackPush16(ushort value)
    {
        var hi = (byte)(value >> 8);
        var lo = (byte)(value & 0xff);
        StackPush(hi);
        StackPush(lo);
    }

    private ushort StackPop16()
    {
        var lo = (ushort)(StackPop());
        var hi = (ushort)(StackPop());
        return (ushort)(hi << 8 | lo);
    }

    private void AslAccumulator()
    {
        if (RegisterA >> 7 == 1) InsertCpuFlag(CpuFlags.Carry);
        else ClearCpuFlag(CpuFlags.Carry);
        RegisterA = (byte)(RegisterA << 1);
        UpdateZeroAndNegativeFlags(RegisterA);
    }

    private byte Asl(AddressingMode addressingMode)
    {
        var addr = GetOperandAddress(addressingMode);
        var data = MemRead(addr);
        if (data >> 7 == 1) InsertCpuFlag(CpuFlags.Carry);
        else ClearCpuFlag(CpuFlags.Carry);
        data = (byte)(data << 1);
        MemWrite(addr, data);
        UpdateZeroAndNegativeFlags(data);
        return data;
    }

    private void LsrAccumulator()
    {
        if ((RegisterA & 1) == 1) InsertCpuFlag(CpuFlags.Carry);
        else ClearCpuFlag(CpuFlags.Carry);
        RegisterA = (byte)(RegisterA >> 1);
        UpdateZeroAndNegativeFlags(RegisterA);
    }

    private byte Lsr(AddressingMode addressingMode)
    {
        var addr = GetOperandAddress(addressingMode);
        var data = MemRead(addr);
        if ((data & 1) == 1) InsertCpuFlag(CpuFlags.Carry);
        else ClearCpuFlag(CpuFlags.Carry);
        data = (byte)(data >> 1);
        MemWrite(addr, data);
        UpdateZeroAndNegativeFlags(data);
        return data;
    }

    private void RolAccumulator()
    {
        var oldCarry = CpuFlagIsSet(CpuFlags.Carry);
        if ((RegisterA >> 7) == 1) InsertCpuFlag(CpuFlags.Carry);
        else ClearCpuFlag(CpuFlags.Carry);
        RegisterA = (byte)(RegisterA << 1);
        if (oldCarry) RegisterA = (byte)(RegisterA | 1);
        UpdateZeroAndNegativeFlags(RegisterA);
    }

    private byte Rol(AddressingMode addressingMode)
    {
        var addr = GetOperandAddress(addressingMode);
        var data = MemRead(addr);
        var oldCarry = CpuFlagIsSet(CpuFlags.Carry);
        if ((data >> 7) == 1) InsertCpuFlag(CpuFlags.Carry);
        else ClearCpuFlag(CpuFlags.Carry);
        data = (byte)(data << 1);
        MemWrite(addr, data);
        UpdateZeroAndNegativeFlags(data);
        return data;
    }

    private void RorAccumulator()
    {
        var oldCarry = CpuFlagIsSet(CpuFlags.Carry);
        if ((RegisterA & 1) == 1) InsertCpuFlag(CpuFlags.Carry);
        else ClearCpuFlag(CpuFlags.Carry);
        RegisterA = (byte)(RegisterA >> 1);
        if (oldCarry) RegisterA = (byte)(RegisterA | 0b10000000);
        UpdateZeroAndNegativeFlags(RegisterA);
    }

    private byte Ror(AddressingMode addressingMode)
    {
        var addr = GetOperandAddress(addressingMode);
        var data = MemRead(addr);
        var oldCarry  = CpuFlagIsSet(CpuFlags.Carry);
        if ((data & 1) == 1) InsertCpuFlag(CpuFlags.Carry);
        else ClearCpuFlag(CpuFlags.Carry);
        data = (byte)(data >> 1);
        MemWrite(addr, data);
        UpdateZeroAndNegativeFlags(data);
        return data;
    }

    private byte Inc(AddressingMode addressingMode)
    {
        var addr = GetOperandAddress(addressingMode);
        var data = MemRead(addr);
        data = WrappingAdd<byte>(data, 1);
        MemWrite(addr, data);
        UpdateZeroAndNegativeFlags(data);
        return data;
    }

    private void Dey()
    {
        RegisterY = WrappingSub<byte, sbyte>(RegisterY, 1);
        UpdateZeroAndNegativeFlags(RegisterY);
    }

    private void Dex()
    {
        RegisterX = WrappingSub<byte, sbyte>(RegisterX, 1);
        UpdateZeroAndNegativeFlags(RegisterX);
    }

    private byte Dec(AddressingMode addressingMode)
    {
        var addr = GetOperandAddress(addressingMode);
        var data = MemRead(addr);
        data = WrappingSub<byte, sbyte>(data, 1);
        MemWrite(addr, data);
        UpdateZeroAndNegativeFlags(data);
        return data;
    }

    private void Pla()
    {
        RegisterA = StackPop();
        UpdateZeroAndNegativeFlags(RegisterA);
    }

    private void Php()
    {
        var flags = Status;
        flags |= CpuFlags.Break;
        flags |= CpuFlags.Break2;
        StackPush((byte)flags);
    }

    private void Plp()
    {
        var newBits = StackPop();
        Status = TruncateFromBits(newBits);
        ClearCpuFlag(CpuFlags.Break);
        ClearCpuFlag(CpuFlags.Break2);
    }

    private void Bit(AddressingMode addressingMode)
    {
        var addr = GetOperandAddress(addressingMode);
        var data = MemRead(addr);
        var and = RegisterA & data;
        if (and == 0) InsertCpuFlag(CpuFlags.Zero);
        else ClearCpuFlag(CpuFlags.Zero);
        SetCpuFlag(CpuFlags.Negative, (data & 0b10000000) > 0);
        SetCpuFlag(CpuFlags.Overflow, (data & 0b01000000) > 0);
    }

    private void Compare(AddressingMode addressingMode, byte comparison)
    {
        var addr = GetOperandAddress(addressingMode);
        var data = MemRead(addr);
        if (data <= comparison) InsertCpuFlag(CpuFlags.Carry);
        else ClearCpuFlag(CpuFlags.Carry);
        UpdateZeroAndNegativeFlags(WrappingSub<byte, sbyte>(comparison, data));
    }

    private void Branch(bool condition)
    {
        if (!condition) return;
        var jump = (sbyte)MemRead(ProgramCounter);
        ProgramCounter = WrappingAdd<ushort>(ProgramCounter, 1);
        ProgramCounter = WrappingAdd(ProgramCounter, (byte)jump);
    }

    private void Tax()
    {
        RegisterX = RegisterA;
        UpdateZeroAndNegativeFlags(RegisterX);
    }

    private void Inx()
    {
        RegisterX = WrappingAdd<byte>(RegisterX, 1);
        UpdateZeroAndNegativeFlags(RegisterX);
    }

    private void Iny()
    {
        RegisterY = WrappingAdd<byte>(RegisterY, 1);
        UpdateZeroAndNegativeFlags(RegisterY);
    }

    private void Ldy(AddressingMode addressingMode)
    {
        var addr = GetOperandAddress(addressingMode);
        var data = MemRead(addr);
        RegisterY = data;
        UpdateZeroAndNegativeFlags(RegisterY);
    }

    private void Ldx(AddressingMode addressingMode)
    {
        var addr = GetOperandAddress(addressingMode);
        var data = MemRead(addr);
        RegisterX = data;
        UpdateZeroAndNegativeFlags(RegisterX);
    }

    private void And(AddressingMode addressingMode)
    {
        var addr = GetOperandAddress(addressingMode);
        var data = MemRead(addr);
        RegisterA = (byte)(data & RegisterA);
        UpdateZeroAndNegativeFlags(RegisterA);
    }

    private void Eor(AddressingMode addressingMode)
    {
        var addr = GetOperandAddress(addressingMode);
        var data = MemRead(addr);
        RegisterA = (byte)(data ^ RegisterA);
        UpdateZeroAndNegativeFlags(RegisterA);
    }

    private void Ora(AddressingMode addressingMode)
    {
        var addr = GetOperandAddress(addressingMode);
        var data = MemRead(addr);
        RegisterA = (byte)(data | RegisterA);
        UpdateZeroAndNegativeFlags(RegisterA);
    }

    private void UpdateZeroAndNegativeFlags(byte data)
    {
        SetCpuFlag(CpuFlags.Zero, data == 0);
        SetCpuFlag(CpuFlags.Negative, (data & 0b10000000) != 0);
    }

    public void Interpret(byte[] program)
    {
        Load(program);
        ProgramCounter = MemRead16(0xFFFC);
        Run();
    }

    /*Helper methods to simulate Rusts wrapping add and wrapping sub*/
    private T WrappingSub<T, TSigned>(T value, T sub) 
        where T : INumber<T>
        where TSigned : ISignedNumber<TSigned>
    {
        var val = ToSigned<T, TSigned>(value);
        var subVal = ToSigned<T, TSigned>(sub);
        val = val - subVal;
        return ToUnsigned<T, TSigned>(val);
    }

    private T WrappingAdd<T>(T value, T add) where T : INumber<T>
    {
        return value + add;
    }

    private TSigned ToSigned<T, TSigned>(T value) 
        where T : INumber<T>
        where TSigned : ISignedNumber<TSigned>
    {
        return TSigned.CreateTruncating(value);
    }

    private T ToUnsigned<T, TSigned>(TSigned value)
        where T : INumber<T>
        where TSigned : ISignedNumber<TSigned>
    {
        return T.CreateTruncating(value);
    }

    private ushort IndirectX()
    {
        var baseMem = MemRead(ProgramCounter);
        var ptr = WrappingAdd(baseMem, RegisterX);
        var lo = MemRead(ptr);
        var hi = MemRead(WrappingAdd<byte>(ptr, 1));
        return (ushort)((hi << 8) | lo);
    }

    private ushort IndirectY()
    {
        var baseMem = MemRead(ProgramCounter);
        var lo = MemRead(baseMem);
        var hi = MemRead(WrappingAdd<byte>(baseMem, 1));
        var derefBaseMem = (ushort)((hi << 8) | lo);
        var deref = WrappingAdd(derefBaseMem, RegisterY);
        return deref;
    }
}