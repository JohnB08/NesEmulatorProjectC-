using System;
using System.Collections.Generic;
using System.Numerics;
using NesEmulatorAvaloniaTest.NES.OpCodes;

namespace NesEmulatorAvaloniaTest.NES.CPU;
using CpuFlags;
using AddressingMode;
using AritmetricHelper;

public class Cpu
{
    private static CpuFlags TruncateFromBits(byte bits)
    {
        const byte validMask = (byte)
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

    public bool running = true;
    private byte _stackReset = 0xfd;
    private ushort _stack = 0x0100;
    public byte RegisterA = 0;
    public byte RegisterX = 0;
    public  byte RegisterY = 0;
    private byte _stackPointer;
    public CpuFlags Status = TruncateFromBits(0b100100);
    public ushort _programCounter = 0;
    private byte[] _memory = new byte[0xffff];
    private readonly Dictionary<byte, OpCode> _opCodeMap = OpCodes.OpCodes.OpCodeMap;

    public Cpu()
    {
        _stackPointer = _stackReset;
    }

    public byte MemRead(ushort address)
    {
        return _memory[address];
    }

    private ushort MemRead16(ushort address)
    {
        var lo = (ushort)MemRead(address);
        var hi = (ushort)MemRead((ushort)(address + 1));
        return (ushort)((hi << 8) | lo);
    }

    public void MemWrite(ushort address, byte value)
    {
        _memory[address] = value;
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
        Status = TruncateFromBits(0b0100100);
        _stackPointer = _stackReset;
        _programCounter = MemRead16(0xfffc);
    }

    public void Load(byte[] data)
    {
        data.CopyTo(_memory, 0x0600);
        MemWrite16(0xFFFC, 0x0600);
    }

    public void Run()
    {
        RunWithCallback((_,_,_) => { });
    }

    public void RunWithCallback(Action<Cpu, byte, OpCode> callback)
    {
        while (running)
        {
            var code = MemRead(_programCounter);
            _programCounter++;
            var programCounterState = _programCounter;
            var opCode = _opCodeMap[code];
            callback(this, code, opCode);
            if (code == 0x00) return;
            switch (code)
            {
                case 0xa9 or 0xa5 or 0xb5 or 0xad or 0xbd or 0xb9 or 0xa1 or 0xb1:
                    Lda(opCode.AddressingMode); 
                    break;
                case 0xAA:
                    Tax();
                    break;
                case 0xE8:
                    Inx();
                    break;
                case 0xd8:
                    ClearCpuFlag(CpuFlags.DecimalMode);
                    break;
                case 0x58:
                    ClearCpuFlag(CpuFlags.InterruptDisable);
                    break;
                case 0xb8:
                    ClearCpuFlag(CpuFlags.Overflow);
                    break;
                case 0x18:
                    ClearCpuFlag(CpuFlags.Carry);
                    break;
                case 0x38:
                    InsertCpuFlag(CpuFlags.Carry);
                    break;
                case 0x78:
                    InsertCpuFlag(CpuFlags.InterruptDisable);
                    break;
                case 0xf8:
                    InsertCpuFlag(CpuFlags.DecimalMode);
                    break;
                case 0x48:
                    StackPush(RegisterA);
                    break;
                case 0x68:
                    Pla();
                    break;
                case 0x08:
                    Php();
                    break;
                case 0x28:
                    Plp();
                    break;
               case 0x69 or 0x65 or 0x75 or 0x6d or 0x79 or 0x61 or 0x71:
                    Adc(opCode.AddressingMode);
                    break;
                case 0xe9 or 0xe5 or 0xf5 or 0xed or 0xfd or 0xf9 or 0xe1 or 0xf1:
                    Sbc(opCode.AddressingMode);
                    break;
                case 0x29 or 0x25 or 0x35 or 0x2d or 0x3d or 0x39 or 0x21 or 0x31:
                    And(opCode.AddressingMode);
                    break;
                case 0x49 or 0x45 or 0x55 or 0x4d or 0x5d or 0x59 or 0x41 or 0x51:
                    Eor(opCode.AddressingMode);
                    break;
                case 0x09 or 0x05 or 0x15 or 0x0d or 0x1d or 0x19 or 0x01 or 0x11:
                    Ora(opCode.AddressingMode);
                    break;
                case 0x4a:
                    LsrAccumulator();
                    break;
                case 0x46 or 0x56 or 0x4e or 0x5e:
                    SetOperandNoReturn(Lsr(opCode.AddressingMode));
                    break;
                case 0x0a:
                    AslAccumulator();
                    break;
                case 0x06 or 0x16 or 0x0e or 0x1e:
                    SetOperandNoReturn(Asl(opCode.AddressingMode));
                    break;
                case 0x2a:
                    RolAccumulator();
                    break;
                case 0x26 or 0x36 or 0x2e or 0x3e:
                    SetOperandNoReturn(Rol(opCode.AddressingMode));
                    break;
                case 0x6a:
                    RorAccumulator();
                    break;
                case 0x66 or 0x76 or 0x6e or 0x7e:
                    SetOperandNoReturn(Ror(opCode.AddressingMode));
                    break;
                case 0xe6 or 0xf6 or 0xee or 0xfe:
                    SetOperandNoReturn(Inc(opCode.AddressingMode));
                    break;
                case 0xc8:
                    Iny();
                    break;
                case 0xc6 or 0xd6 or 0xce or 0xde:
                    SetOperandNoReturn(Dec(opCode.AddressingMode));
                    break;
                case 0xca:
                    Dex();
                    break;
                case 0x88:
                    Dey();
                    break;
                case 0xc9 or 0xc5 or 0xd5 or 0xcd or 0xdd or 0xd9 or 0xc1 or 0xd1:
                    Compare(opCode.AddressingMode,
                        RegisterA);
                    break;
                case 0xc0 or 0xc4 or 0xcc:
                    Compare(opCode.AddressingMode, RegisterY);
                    break;
                case 0xe0 or 0xe4 or 0xec:
                    Compare(opCode.AddressingMode, RegisterX);
                    break;
                case 0x4c:
                    LoadProgramCounter16();
                    break;
                case 0x6c:
                    JmpIndirect();
                    break;
                case 0x20:
                    Jrs();
                    break;
                case 0x60:
                    SetProgramCounterFromStack16();
                    break;
                case 0x40:
                    Rti();
                    break;
                case 0xd0:
                    Branch(!CpuFlagIsSet(CpuFlags.Zero));
                    break;
                case 0x70:
                    Branch(CpuFlagIsSet(CpuFlags.Overflow));
                    break;
                case 0x50:
                    Branch(!CpuFlagIsSet(CpuFlags.Overflow));
                    break;
                case 0x10:
                    Branch(!CpuFlagIsSet(CpuFlags.Negative));
                    break;
                case 0x30:
                    Branch(CpuFlagIsSet(CpuFlags.Negative));
                    break;
                case 0xf0:
                    Branch(CpuFlagIsSet(CpuFlags.Zero));
                    break;
                case 0xb0:
                    Branch(CpuFlagIsSet(CpuFlags.Carry));
                    break;
                case 0x90:
                    Branch(!CpuFlagIsSet(CpuFlags.Carry));
                    break;
                case 0x24 or 0x2c:
                    Bit(opCode.AddressingMode);
                    break;
                case 0x85 or 0x95 or 0x8d or 0x9d or 0x99 or 0x81 or 0x91:
                    Sta(opCode.AddressingMode);
                    break;
                case 0x86 or 0x96 or 0x8e:
                    Stx(opCode.AddressingMode);
                    break;
                case 0x84 or 0x94 or 0x8c:
                    Sty(opCode.AddressingMode);
                    break;
                case 0xa2 or 0xa6 or 0xb6 or 0xae or 0xbe:
                    Ldx(opCode.AddressingMode);
                    break;
                case 0xa0 or 0xa4 or 0xb4 or 0xac or 0xbc:
                    Ldy(opCode.AddressingMode);
                    break;
                case 0xea:
                    Nop();
                    break;
                case 0xa8:
                    Tay();
                    break;
                case 0xba:
                    Tsx();
                    break;
                case 0x8a:
                    Txa();
                    break;
                case 0x9a:
                    Txs();
                    break;
                case 0x98:
                    Tya();
                    break;
            };
            if (programCounterState == _programCounter) _programCounter += (ushort)(opCode.Length - 1);

        }
    }

    public void LoadAndRun(byte[] data)
    {
        Load(data);
        Reset();
        Run();
    }

    private ushort GetOperandAddress(AddressingMode addressingMode)
    {
        return addressingMode switch
        {
            AddressingMode.Immediate => _programCounter,
            AddressingMode.ZeroPage => MemRead(_programCounter),
            AddressingMode.Absolute => MemRead16(_programCounter),
            AddressingMode.ZeroPageX => AritmetricHelper.WrappingAdd(MemRead(_programCounter), RegisterX),
            AddressingMode.ZeroPageY => AritmetricHelper.WrappingAdd(MemRead(_programCounter), RegisterY),
            AddressingMode.AbsoluteX => AritmetricHelper.WrappingAdd(MemRead16(_programCounter), RegisterX),
            AddressingMode.AbsoluteY => AritmetricHelper.WrappingAdd(MemRead16(_programCounter), RegisterY),
            AddressingMode.IndirectX => IndirectX(),
            AddressingMode.IndirectY => IndirectY(),
            _ => throw new NotImplementedException(),
        };
    }

    private void Lda(AddressingMode addressingMode)
    {
        var addr = GetOperandAddress(addressingMode);
        var val = MemRead(addr);
        RegisterA = val;
        UpdateZeroAndNegativeFlags(RegisterA);
    }

    private void InsertCpuFlag(CpuFlags flag)
    {
        Status |= flag;
    }

    private void ClearCpuFlag(CpuFlags flag)
    {
        Status &= ~flag;
    }

    public bool CpuFlagIsSet(CpuFlags flag)
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
        AddToRegisterA(AritmetricHelper.WrappingSub<byte, sbyte>(data, 1));
    }

    private void Adc(AddressingMode addressingMode)
    {
        var addr = GetOperandAddress(addressingMode);
        var data = MemRead(addr);
        AddToRegisterA(data);
    }

    private byte StackPop()
    {
        _stackPointer = AritmetricHelper.WrappingAdd<byte>(_stackPointer, 1);
        return MemRead((ushort)(_stack + _stackPointer));
    }

    private void StackPush(byte value)
    {
        MemWrite((ushort)(_stack + _stackPointer), value);
        _stackPointer = AritmetricHelper.WrappingSub<byte, sbyte>(_stackPointer, 1);
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
        if (oldCarry) data = (byte)(data | 1);
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
        var oldCarry = CpuFlagIsSet(CpuFlags.Carry);
        if ((data & 1) == 1) InsertCpuFlag(CpuFlags.Carry);
        else ClearCpuFlag(CpuFlags.Carry);
        data = (byte)(data >> 1);
        if (oldCarry) data = (byte)(data | 0b10000000);
        MemWrite(addr, data);
        UpdateZeroAndNegativeFlags(data);
        return data;
    }

    private byte Inc(AddressingMode addressingMode)
    {
        var addr = GetOperandAddress(addressingMode);
        var data = MemRead(addr);
        data = AritmetricHelper.WrappingAdd<byte>(data, 1);
        MemWrite(addr, data);
        UpdateZeroAndNegativeFlags(data);
        return data;
    }

    private void Dey()
    {
        RegisterY = AritmetricHelper.WrappingSub<byte, sbyte>(RegisterY, 1);
        UpdateZeroAndNegativeFlags(RegisterY);
    }

    private void Dex()
    {
        RegisterX = AritmetricHelper.WrappingSub<byte, sbyte>(RegisterX, 1);
        UpdateZeroAndNegativeFlags(RegisterX);
    }

    private byte Dec(AddressingMode addressingMode)
    {
        var addr = GetOperandAddress(addressingMode);
        var data = MemRead(addr);
        data = AritmetricHelper.WrappingSub<byte, sbyte>(data, 1);
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
        UpdateZeroAndNegativeFlags(AritmetricHelper.WrappingSub<byte, sbyte>(comparison, data));
    }

    private void Branch(bool condition)
    {
        if (!condition) return;
        var jump = (sbyte)MemRead(_programCounter);
        _programCounter = AritmetricHelper.WrappingAdd<ushort>(_programCounter, 1);
        _programCounter = AritmetricHelper.WrappingAdd(_programCounter, (ushort)(short)jump);
    }

    private void Tax()
    {
        RegisterX = RegisterA;
        UpdateZeroAndNegativeFlags(RegisterX);
    }

    private void Inx()
    {
        RegisterX = AritmetricHelper.WrappingAdd<byte>(RegisterX, 1);
        UpdateZeroAndNegativeFlags(RegisterX);
    }

    private void Iny()
    {
        RegisterY = AritmetricHelper.WrappingAdd<byte>(RegisterY, 1);
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
        _programCounter = MemRead16(0xFFFC);
        Run();
    }

    private ushort IndirectX()
    {
        var baseMem = MemRead(_programCounter);
        var ptr = AritmetricHelper.WrappingAdd(baseMem, RegisterX);
        var lo = MemRead(ptr);
        var hi = MemRead(AritmetricHelper.WrappingAdd<byte>(ptr, 1));
        return (ushort)((hi << 8) | lo);
    }

    private ushort IndirectY()
    {
        var baseMem = MemRead(_programCounter);
        var lo = MemRead(baseMem);
        var hi = MemRead(AritmetricHelper.WrappingAdd<byte>(baseMem, 1));
        var derefBaseMem = (ushort)((hi << 8) | lo);
        var deref = AritmetricHelper.WrappingAdd(derefBaseMem, RegisterY);
        return deref;
    }

    private void JmpIndirect()
    {
        var addr = MemRead16(_programCounter);
        ushort inDirectRef;
        if ((addr & 0x00FF) == 0x00FF)
        {
            var lo = MemRead(addr);
            var hi = MemRead((byte)(addr & 0x00FF));
            inDirectRef = (ushort)((hi << 8) | lo);
        }
        else inDirectRef = MemRead16(addr);

        _programCounter = inDirectRef;
    }

    private void Jrs()
    {
        StackPush16((ushort)(_programCounter + 2 - 1));
        _programCounter = MemRead16(_programCounter);
    }

    private void Rti()
    {
        var newBits = StackPop();
        Status = TruncateFromBits(newBits);
        ClearCpuFlag(CpuFlags.Break);
        InsertCpuFlag(CpuFlags.Break2);
        _programCounter = StackPop16();
    }

    private void Sta(AddressingMode addressingMode)
    {
        var addr = GetOperandAddress(addressingMode);
        MemWrite(addr, RegisterA);
    }

    private void Stx(AddressingMode addressingMode)
    {
        var addr = GetOperandAddress(addressingMode);
        MemWrite(addr, RegisterX);
    }

    private void Sty(AddressingMode addressingMode)
    {
        var addr = GetOperandAddress(addressingMode);
        MemWrite(addr, RegisterY);
    }

    private void Nop()
    {
    }

    private void Tay()
    {
        RegisterY = RegisterA;
        UpdateZeroAndNegativeFlags(RegisterY);
    }

    private void Tsx()
    {
        RegisterX = _stackPointer;
        UpdateZeroAndNegativeFlags(RegisterX);
    }

    private void Txa()
    {
        RegisterA = RegisterX;
        UpdateZeroAndNegativeFlags(RegisterA);
    }

    private void Txs()
    {
        _stackPointer = RegisterX;
    }

    private void Tya()
    {
        RegisterA = RegisterY;
        UpdateZeroAndNegativeFlags(RegisterA);
    }

    private void LoadProgramCounter16()
    {
        _programCounter = MemRead16(_programCounter);
    }

    private void SetProgramCounterFromStack16()
    {
        _programCounter = (ushort)(StackPop16() + 1);
    }

    private void SetOperandNoReturn(byte op)
    {
        _ = op;
    }
}