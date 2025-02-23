using NesEmulatorAvaloniaTest.NES.AritmetricHelper;
using NesEmulatorAvaloniaTest.NES.CpuFlags;
using Xunit.Abstractions;

namespace ComponentTests;

using NesEmulatorAvaloniaTest.NES.CPU;

public class UnitTest1(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Test0Xa9LdaImmediateLoadData()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa9, 0x05, 0x00]);
        Assert.Equal(5, cpu.RegisterA);
        Assert.True(((byte)cpu.Status & 0b00000010) == 0b00);
        Assert.True(((byte)cpu.Status & 0b10000000) == 0);
    }

    [Fact]
    public void Test0XaaTaxMoveAToX()
    {
        var cpu = new Cpu();
        cpu.Load([0xaa, 0x00]);
        cpu.Reset();
        cpu.RegisterA = 10;
        cpu.Run();
        Assert.Equal(10, cpu.RegisterX);
    }

    [Fact]
    public void Test5OpsWorkingTogether()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa9, 0xc0, 0xaa, 0xe8, 0x00]);
        Assert.Equal(0xc1, cpu.RegisterX);
    }

    [Fact]
    public void TestInxOverflow()
    {
        var cpu = new Cpu();
        cpu.Load([0xe8, 0xe8, 0x00]);
        cpu.Reset();
        cpu.RegisterX = 0xff;
        cpu.Run();
        Assert.Equal(1, cpu.RegisterX);
    }

    [Fact]
    public void TestLdaFromMemory()
    {
        var cpu = new Cpu();
        cpu.MemWrite(0x10, 0x55);
        cpu.LoadAndRun([0xa5, 0x10, 0x00]);
        Assert.Equal(0x55, cpu.RegisterA);
    }
    
    [Fact]
    public void TestVideoMemoryIsUpdated()
    {
        // Arrange
        var cpu = new Cpu();
        var lastPC = 0;
        byte[] snakeGameData = [
                                       0x20, 0x06, 0x06, 0x20, 0x38, 0x06, 0x20, 0x0d, 0x06, 0x20, 0x2a, 0x06, 0x60, 0xa9, 0x02,
                                       0x85, 0x02, 0xa9, 0x04, 0x85, 0x03, 0xa9, 0x11, 0x85, 0x10, 0xa9, 0x10, 0x85, 0x12, 0xa9,
                                       0x0f, 0x85, 0x14, 0xa9, 0x04, 0x85, 0x11, 0x85, 0x13, 0x85, 0x15, 0x60, 0xa5, 0xfe, 0x85,
                                       0x00, 0xa5, 0xfe, 0x29, 0x03, 0x18, 0x69, 0x02, 0x85, 0x01, 0x60, 0x20, 0x4d, 0x06, 0x20,
                                       0x8d, 0x06, 0x20, 0xc3, 0x06, 0x20, 0x19, 0x07, 0x20, 0x20, 0x07, 0x20, 0x2d, 0x07, 0x4c,
                                       0x38, 0x06, 0xa5, 0xff, 0xc9, 0x77, 0xf0, 0x0d, 0xc9, 0x64, 0xf0, 0x14, 0xc9, 0x73, 0xf0,
                                       0x1b, 0xc9, 0x61, 0xf0, 0x22, 0x60, 0xa9, 0x04, 0x24, 0x02, 0xd0, 0x26, 0xa9, 0x01, 0x85,
                                       0x02, 0x60, 0xa9, 0x08, 0x24, 0x02, 0xd0, 0x1b, 0xa9, 0x02, 0x85, 0x02, 0x60, 0xa9, 0x01,
                                       0x24, 0x02, 0xd0, 0x10, 0xa9, 0x04, 0x85, 0x02, 0x60, 0xa9, 0x02, 0x24, 0x02, 0xd0, 0x05,
                                       0xa9, 0x08, 0x85, 0x02, 0x60, 0x60, 0x20, 0x94, 0x06, 0x20, 0xa8, 0x06, 0x60, 0xa5, 0x00,
                                       0xc5, 0x10, 0xd0, 0x0d, 0xa5, 0x01, 0xc5, 0x11, 0xd0, 0x07, 0xe6, 0x03, 0xe6, 0x03, 0x20,
                                       0x2a, 0x06, 0x60, 0xa2, 0x02, 0xb5, 0x10, 0xc5, 0x10, 0xd0, 0x06, 0xb5, 0x11, 0xc5, 0x11,
                                       0xf0, 0x09, 0xe8, 0xe8, 0xe4, 0x03, 0xf0, 0x06, 0x4c, 0xaa, 0x06, 0x4c, 0x35, 0x07, 0x60,
                                       0xa6, 0x03, 0xca, 0x8a, 0xb5, 0x10, 0x95, 0x12, 0xca, 0x10, 0xf9, 0xa5, 0x02, 0x4a, 0xb0,
                                       0x09, 0x4a, 0xb0, 0x19, 0x4a, 0xb0, 0x1f, 0x4a, 0xb0, 0x2f, 0xa5, 0x10, 0x38, 0xe9, 0x20,
                                       0x85, 0x10, 0x90, 0x01, 0x60, 0xc6, 0x11, 0xa9, 0x01, 0xc5, 0x11, 0xf0, 0x28, 0x60, 0xe6,
                                       0x10, 0xa9, 0x1f, 0x24, 0x10, 0xf0, 0x1f, 0x60, 0xa5, 0x10, 0x18, 0x69, 0x20, 0x85, 0x10,
                                       0xb0, 0x01, 0x60, 0xe6, 0x11, 0xa9, 0x06, 0xc5, 0x11, 0xf0, 0x0c, 0x60, 0xc6, 0x10, 0xa5,
                                       0x10, 0x29, 0x1f, 0xc9, 0x1f, 0xf0, 0x01, 0x60, 0x4c, 0x35, 0x07, 0xa0, 0x00, 0xa5, 0xfe,
                                       0x91, 0x00, 0x60, 0xa6, 0x03, 0xa9, 0x00, 0x81, 0x10, 0xa2, 0x00, 0xa9, 0x01, 0x81, 0x10,
                                       0x60, 0xa2, 0x00, 0xea, 0xea, 0xca, 0xd0, 0xfb, 0x60,
                                   ];
        cpu.Load(snakeGameData);
        cpu.Reset();
        var i = 0;
        cpu.RunWithCallback((_,code, opCode) =>
        {
            testOutputHelper.WriteLine($"Step {i}: PC={cpu._programCounter:X4}, A={cpu.RegisterA:X2}, X={cpu.RegisterX:X2}, Y={cpu.RegisterY:X2}");
            testOutputHelper.WriteLine($"Instruction at {cpu._programCounter:X4}: {opCode.Mnemonic}, opcode={code:X2}");
            lastPC = cpu._programCounter;
            i++;
            if (i == 100000) cpu.running = false;
        });
        var anyNonZero = false;
        for (ushort addr = 0x0200; addr < 0x600; addr++)
        {
            if (cpu.MemRead(addr) == 0) continue;
            anyNonZero = true;
            break;
        }
        Assert.True(anyNonZero, "Expected video memory to be updated, but it remains all zero.");
    }

    [Fact]
    public void TestWrappingAdd()
    {
        byte b1 = 255;
        byte b2 = 1;
        Assert.Equal(0, AritmetricHelper.WrappingAdd<byte>(b1,b2));
        
        byte b3 = 200;
        byte b4 = 100;
        Assert.Equal(44, AritmetricHelper.WrappingAdd<byte>(b3,b4));
        
        ushort us1 = 65535;
        ushort us2 = 1;
        Assert.Equal(0, AritmetricHelper.WrappingAdd<ushort>(us1,us2));

        ushort us3 = 65530;
        ushort us4 = 10;
        Assert.Equal(4, AritmetricHelper.WrappingAdd<ushort>(us3,us4));

        ushort us5 = 40000;
        ushort us6 = 30000;
        Assert.Equal(4464, AritmetricHelper.WrappingAdd<ushort>(us5,us6));
    }
    [Fact]
    public void TestSimpleVideoMemoryWrite()
    {
        var cpu = new Cpu();
        // LDA #$01   ; Load 1 into A
        // STA $0200  ; Store A at video memory location 0x0200
        // BRK        ; Break
        cpu.LoadAndRun(new byte[] { 0xa9, 0x01, 0x8d, 0x00, 0x02, 0x00 });
    
        Assert.Equal(1, cpu.MemRead(0x0200));
    }

    [Fact]
    public void TestLdaImmediate()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa9, 0x42, 0x00]);
        Assert.Equal(0x42, cpu.RegisterA);
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Zero));       
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Negative));
    }

    [Fact]
    public void TestLdaZeroFlag()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa9, 0x00, 0x00]);
        Assert.Equal(0x00, cpu.RegisterA);
        Assert.True(cpu.CpuFlagIsSet(CpuFlags.Zero));
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Negative));
    }

    [Fact]
    public void TestLdaNegativeFlag()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa9, 0x80, 0x00]); //LDA #80 BRK
        Assert.Equal(0x80, cpu.RegisterA);
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Zero));
        Assert.True(cpu.CpuFlagIsSet(CpuFlags.Negative));
    }

    [Fact]
    public void TestTax()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([ 0xa9, 0x42, 0xaa, 0x00]); //LDA #42 TAX BRK
        Assert.Equal(0x42, cpu.RegisterA);
        Assert.Equal(0x42, cpu.RegisterX); 
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Zero));
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Negative));
    }

    [Fact]
    public void TestInx()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa9, 0x42, 0xaa, 0xe8, 0x00]); //LDA #42 TAX INX BRK
        Assert.Equal(0x42, cpu.RegisterA);
        Assert.Equal(0x43, cpu.RegisterX);
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Zero));
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Negative));
    }

    [Fact]
    public void TestStaAbsolute()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa9, 0x55, 0x8d, 0x42, 0x42, 0x00]); //LDA #55 STA #4242 BRK
        Assert.Equal(0x55, cpu.MemRead(0x4242));
        Assert.Equal(0x55, cpu.RegisterA);
    }

    [Fact]
    public void TestStxZeroPage()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa2, 0x42, 0x86, 0x42, 0x00]); //LDX #42 STX #42 BRK
        Assert.Equal(0x42, cpu.MemRead(0x42));
        Assert.Equal(0x42, cpu.RegisterX);
    }

    [Fact]
    public void TestStyZeroPage()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa0, 0x42, 0x84, 0x42, 0x00]); //LDY #42 STY #42 BRK
        Assert.Equal(0x42, cpu.MemRead(0x42));
        Assert.Equal(0x42, cpu.RegisterY);
    }

    [Fact]
    public void TestStaZeroPageX()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa2, 0x01, 0xa9, 0x55, 0x95, 0x42, 0x00]); //LDX #01 LDA #55 STA #42RegisterX BRK
        Assert.Equal(0x55, cpu.MemRead(0x43));
        Assert.Equal(0x55, cpu.RegisterA);
    }

    [Fact]
    public void TestStaZeroPageXWrap()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa2, 0xff, 0xa9, 0x55, 0x95, 0x42, 0x00]); //LDX #FF LDA #55 STA #42RegisterX BRK
        Assert.Equal(0x55, cpu.MemRead(0x41)); //#FF + #42 should wrap to #41
    }

    [Fact]
    public void TestAdcBasic()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa9, 0x50, 0x69, 0x50, 0x00]); //LDA #50 ADC #50 BRK
        Assert.Equal(0xa0, cpu.RegisterA); //#50 + #50 = #A0
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Zero));
        Assert.True(cpu.CpuFlagIsSet(CpuFlags.Negative));
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Carry));
    }

    [Fact]
    public void TestAdcWithCarry()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0x38, 0xa9, 0x50, 0x69, 0x50, 0x00]);
        Assert.Equal(0xa1, cpu.RegisterA);
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Zero));
        Assert.True(cpu.CpuFlagIsSet(CpuFlags.Negative));
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Carry));
    }

    [Fact]
    public void TestAdcOverflow()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa9, 0xff, 0x69, 0x01, 0x00]);
        Assert.Equal(0x00, cpu.RegisterA);
        Assert.True(cpu.CpuFlagIsSet(CpuFlags.Zero));
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Negative));
        Assert.True(cpu.CpuFlagIsSet(CpuFlags.Carry));
    }

    [Fact]
    public void TestSbcBasic()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0x38, 0xa9, 0x50, 0xe9, 0x20, 0x00]); //SEC LDA #50 SBC #20 BRK
        Assert.Equal(0x30, cpu.RegisterA); //#50 - #20 = #30
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Zero));
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Negative));
        Assert.True(cpu.CpuFlagIsSet(CpuFlags.Carry));
    }

    [Fact]
    public void TestSbcWithBorrow()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0x38, 0xa9, 0x50, 0xe9, 0xF0, 0x00]); //SEC LDA #50 SBC #F0 BRK
        Assert.Equal(0x60, cpu.RegisterA); //#50 - #F0 = #60 (with borrow)
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Zero));
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Negative));
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Carry));
    }

    [Fact]
    public void TestSbcZero()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0x38, 0xa9, 0x20, 0xe9, 0x20, 0x00]); //SEC LDA #20 SBC #20 BRK
        Assert.Equal(0x00, cpu.RegisterA); //#20 - #20 = #00
        Assert.True(cpu.CpuFlagIsSet(CpuFlags.Zero));
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Negative));
        Assert.True(cpu.CpuFlagIsSet(CpuFlags.Carry));
    }

    [Fact]
    public void TestAdcBinaryOverflow()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa9, 0x7f, 0x69, 0x01, 0x00]); //LDA #7F ADC #01 BRK
        Assert.Equal(0x80, cpu.RegisterA); //#7F + #01 = #80
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Zero));
        Assert.True(cpu.CpuFlagIsSet(CpuFlags.Negative));
        Assert.True(cpu.CpuFlagIsSet(CpuFlags.Overflow));
    }

    [Fact]
    public void TestSbcBinaryOverflow()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0x38, 0xa9, 0x80, 0xe9, 0x01, 0x00]); //SEC LDA #80 SBC #01 BRK
        Assert.Equal(0x7F, cpu.RegisterA);//#80 - #01 = #7F
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Zero));
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Negative));
        Assert.True(cpu.CpuFlagIsSet(CpuFlags.Overflow));
    }

    [Fact]
    public void TestAndBasic()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa9, 0xff, 0x29, 0x0f, 0x00]); //LDA #FF AND #0F BRK
        Assert.Equal(0x0f, cpu.RegisterA); //#FF & #0F = #0F
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Zero));
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Negative));
    }

    [Fact]
    public void TestAndWithZero()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa9, 0xf0, 0x29, 0x0f, 0x00]); //LDA #F0 AND #0F BRK
        Assert.Equal(0x00, cpu.RegisterA); //#F0 & #0F == #00
        Assert.True(cpu.CpuFlagIsSet(CpuFlags.Zero));
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Negative));
    }

    [Fact]
    public void TestAndNegativeFlag()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa9, 0xff, 0x29, 0xf0, 0x00]); //LDA #FF AND #F0 BRK
        Assert.Equal(0xF0, cpu.RegisterA); //#FF & #F0 == #F0
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Zero));
        Assert.True(cpu.CpuFlagIsSet(CpuFlags.Negative));
    }

    [Fact]
    public void TestOraBasic()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa9, 0x0f, 0x09, 0xf0, 0x00]); //LDA #0F ORA #F0 BRK
        Assert.Equal(0xff, cpu.RegisterA); //#0F | #F0 = #FF
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Zero));
        Assert.True(cpu.CpuFlagIsSet(CpuFlags.Negative));
    }

    [Fact]
    public void TestOraWithZero()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa9, 0x00, 0x09, 0x00, 0x00]); //LDA #00 ORA #00 BRK
        Assert.Equal(0x00, cpu.RegisterA); //#00 | #00 = #00
        Assert.True(cpu.CpuFlagIsSet(CpuFlags.Zero));
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Negative));
    }

    [Fact]
    public void TestEorBasic()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa9, 0xff, 0x49, 0x0f, 0x00]); //LDA #FF EOR #0F BRK
        Assert.Equal(0xf0, cpu.RegisterA); //#FF XOR #0F == #F0
        Assert.True(!cpu.CpuFlagIsSet(CpuFlags.Zero));
        Assert.True(cpu.CpuFlagIsSet(CpuFlags.Negative));
    }

    
    
    
    
    
    /*Branching*/
    [Fact]
    public void TestBeqTaken()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa9, 0x00, 0xf0, 0x03, 0xa9, 0x42, 0x00, 0xa9, 0x24, 0x00]); //LDA #00 BEQ #03 LDA #42 BRK LDA #24 BRK
        Assert.Equal(0x24, cpu.RegisterA); //Should skip to second LDA instruction and LDA #24
    }

    [Fact]
    public void TestBeqNotTaken()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa9, 0x42, 0xf0, 0x02, 0xa9, 0x24, 0x00]); //LDA #42 BEQ #2 #LDA #24 BRK
        Assert.Equal(0x24, cpu.RegisterA); //Should NOT skip to BRK instruction.
    }

    [Fact]
    public void TestBneTaken()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa9, 0x42, 0xd0, 0x03, 0xa9, 0x24, 0x00, 0xa9, 0x12, 0x00]); //LDA #42 BNE #03 LDA #24 BRK LDA #12 BRK
        Assert.Equal(0x12, cpu.RegisterA);//Should skip to second LDA after branching. 
    }

    [Fact]
    public void TestBneNotTaken()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0xa9, 0x00, 0xd0, 0x03, 0xa9, 0x24, 0x00, 0xa9, 0x12, 0x00]);
        Assert.Equal(0x24, cpu.RegisterA);
    }

    [Fact]
    public void TestBcsTaken()
    {
        var cpu = new Cpu();
        cpu.LoadAndRun([0x38, 0xb0, 0x03, 0xa9, 0x4, 0x00, 0xa9, 0x12, 0x00]);
        Assert.Equal(0x12, cpu.RegisterA);  
    }

    /*Jmp indirect*/
    [Fact]
    public void TestJmpIndirect()
    {
        var cpu = new Cpu();
        cpu.MemWrite(0x30ff, 0x80);
        cpu.MemWrite(0x3000, 0x40);
        cpu.LoadAndRun([0x6c, 0xff, 0x30, 0x00]);
        Assert.Equal(0x4081, cpu._programCounter);
    }
}
