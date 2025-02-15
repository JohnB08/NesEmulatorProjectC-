using System.Collections.Generic;

namespace NesEmulatorAvaloniaTest.NES.OpCodes;

public static class OpCodes
{
    public static readonly OpCode[] Codes = [
        /*BRK*/
        /*
         * BRK is the "force break" instruction—a software interrupt. When executed, it:

                Increments the program counter (skipping a dummy byte).
                Pushes the updated PC and the status register (with the break flag set) onto the stack.
                Sets the interrupt disable flag.
                Loads the new PC from the IRQ/BRK interrupt vector (usually at addresses $FFFE/$FFFF).

            This causes control to jump to an interrupt handler, often used for debugging or system calls.
            Op code for force break, or software interrupt.
         */
        new()
        {
            Code = 0x00,
            Mnemonic = "BRK",
            Length = 1,
            Cycles = 7,
            AddressingMode =  AddressingMode.AddressingMode.NoneAddressing
        },
        
        /*NOP*/
        /*
             NOP stands for "No Operation." 
             It does nothing except consume cycles and increment the program counter,
            making it useful for timing adjustments or aligning code without side effects.
         */
        new()
        {
            Code = 0xea,
            Mnemonic = "NOP",
            Length = 1,
            Cycles = 2,
            AddressingMode =  AddressingMode.AddressingMode.NoneAddressing
        },
        
        /*Aritretric*/
        /*adc*/
        /*
         * ADC (Add with Carry) performs addition on the 6502 by adding the accumulator,
         * an operand,
         * and the current carry flag.
         *
         * If the result exceeds 255, the carry flag is set.
         * It also affects the zero, negative, and overflow flags,
         * and in decimal mode, it performs binary-coded decimal arithmetic.
         */
        new()
        {
            Code = 0x69,
            Mnemonic = "ADC",
            Length = 2,
            Cycles = 2,
            AddressingMode =  AddressingMode.AddressingMode.Immediate,
        },
        new()
        {
            Code = 0x65,
            Mnemonic = "ADC",
            Length = 2,
            Cycles = 3,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPage,
        },
        new()
        {
            Code = 0x75,
            Mnemonic = "ADC",
            Length = 2,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPageX,
        },
        new()
        {
            Code = 0x6d,
            Mnemonic = "ADC",
            Length = 3,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.Absolute,
        },
        new()
        {
            Code = 0x7d,
            Mnemonic = "ADC",
            Length = 3,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.AbsoluteX,
        },
        new()
        {
            Code = 0x79,
            Mnemonic = "ADC",
            Length = 3,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.AbsoluteY,
        },
        new()
        {
            Code = 0x61,
            Mnemonic = "ADC",
            Length = 2,
            Cycles = 6,
            AddressingMode =  AddressingMode.AddressingMode.IndirectX,
        },
        new()
        {
            Code = 0x71,
            Mnemonic = "ADC",
            Length = 2,
            Cycles = 5,
            AddressingMode =  AddressingMode.AddressingMode.IndirectY,
        },
        
        /*sbc*/
        /*
         *
         * SBC stands for "Subtract with Carry."
         * It subtracts the operand and the inverse of the carry flag from the accumulator.
         * In other words, if the carry flag is clear (indicating a borrow),
         * an extra 1 is subtracted.
         *
         * The operation affects the negative, zero, overflow, and carry flags.
         * In decimal mode, it performs binary-coded decimal subtraction.
         */
        new()
        {
            Code = 0xe9,
            Mnemonic = "SBC",
            Length = 2,
            Cycles = 2,
            AddressingMode =  AddressingMode.AddressingMode.Immediate,
        },
        new()
        {
            Code = 0xe5,
            Mnemonic = "SBC",
            Length = 2,
            Cycles = 3,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPage,
        },
        new()
        {
            Code = 0xf5,
            Mnemonic = "SBC",
            Length = 2,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPageX,
        },
        new()
        {
            Code = 0xed,
            Mnemonic = "SBC",
            Length = 3,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.Absolute,
        },
        new()
        {
            Code = 0xfd,
            Mnemonic = "SBC",
            Length = 3,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.AbsoluteX,
        },
        new()
        {
            Code = 0xf9,
            Mnemonic = "SBC",
            Length = 3,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.AbsoluteY,
        },
        new()
        {
            Code = 0xe1,
            Mnemonic = "SBC",
            Length = 2,
            Cycles = 6,
            AddressingMode =  AddressingMode.AddressingMode.IndirectX,
        },
        new()
        {
            Code = 0xf1,
            Mnemonic = "SBC",
            Length = 2,
            Cycles = 5,
            AddressingMode =  AddressingMode.AddressingMode.IndirectY,
        },
        
        /*AND*/
        /*
         * The AND instruction performs a bitwise AND between the accumulator and the operand.
         * The result replaces the accumulator's value.
         * It then updates the zero flag (if the result is zero) and the negative flag (if bit 7 of the result is set),
         * leaving other flags unchanged.
         */
        new()
        {
            Code = 0x29,
            Mnemonic = "AND",
            Length = 2,
            Cycles = 2,
            AddressingMode =  AddressingMode.AddressingMode.Immediate,
        },
        new()
        {
            Code = 0x25,
            Mnemonic = "AND",
            Length = 2,
            Cycles = 3,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPage,
        },
        new()
        {
            Code = 0x35,
            Mnemonic = "AND",
            Length = 2,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPageX,
        },
        new()
        {
            Code = 0x2d,
            Mnemonic = "AND",
            Length = 3,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.Absolute,
        },
        new()
        {
            Code = 0x3d,
            Mnemonic = "AND",
            Length = 3,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.AbsoluteX,
        },
        new()
        {
            Code = 0x39,
            Mnemonic = "AND",
            Length = 3,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.AbsoluteY,
        },
        new()
        {
            Code = 0x21,
            Mnemonic = "AND",
            Length = 2,
            Cycles = 6,
            AddressingMode =  AddressingMode.AddressingMode.IndirectX,
        },
        new()
        {
            Code = 0x31,
            Mnemonic = "AND",
            Length = 2,
            Cycles = 5,
            AddressingMode =  AddressingMode.AddressingMode.IndirectY,
        },
        
        /*EOR*/
        
        /*
         * EOR (Exclusive OR) performs a bitwise XOR between the accumulator and the operand.
         * The result is stored in the accumulator.
         * It updates the Zero flag if the result is zero and the Negative flag based on the high bit of the result,
         * while leaving the other flags unchanged.
         */
        new()
        {
            Code = 0x49,
            Mnemonic = "EOR",
            Length = 2,
            Cycles = 2,
            AddressingMode =  AddressingMode.AddressingMode.Immediate,
        },
        new()
        {
            Code = 0x45,
            Mnemonic = "EOR",
            Length = 2,
            Cycles = 3,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPage,
        },
        new()
        {
            Code = 0x55,
            Mnemonic = "EOR",
            Length = 2,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPageX,
        },
        new()
        {
            Code = 0x4d,
            Mnemonic = "EOR",
            Length = 3,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.Absolute,
        },
        new()
        {
            Code = 0x5d,
            Mnemonic = "EOR",
            Length = 3,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.AbsoluteX,
        },
        new()
        {
            Code = 0x59,
            Mnemonic = "EOR",
            Length = 3,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.AbsoluteY,
        },
        new()
        {
            Code = 0x41,
            Mnemonic = "EOR",
            Length = 2,
            Cycles = 6,
            AddressingMode =  AddressingMode.AddressingMode.IndirectX,
        },
        new()
        {
            Code = 0x51,
            Mnemonic = "EOR",
            Length = 2,
            Cycles = 5,
            AddressingMode =  AddressingMode.AddressingMode.IndirectY,
        },
        
        /*ORA*/
        /*
         *ORA performs a bitwise inclusive OR between the accumulator and the operand.
         * The result replaces the accumulator's value,
         * updating the zero flag (if the result is zero) and the negative flag (if bit 7 is set),
         * while leaving the other flags unchanged.
         */
        new()
        {
            Code = 0x09,
            Mnemonic = "ORA",
            Length = 2,
            Cycles = 2,
            AddressingMode =  AddressingMode.AddressingMode.Immediate,
        },
        new()
        {
            Code = 0x05,
            Mnemonic = "ORA",
            Length = 2,
            Cycles = 3,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPage,
        },
        new()
        {
            Code = 0x15,
            Mnemonic = "ORA",
            Length = 2,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPageX,
        },
        new()
        {
            Code = 0x0d,
            Mnemonic = "ORA",
            Length = 3,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.Absolute,
        },
        new()
        {
            Code = 0x1d,
            Mnemonic = "ORA",
            Length = 3,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.AbsoluteX,
        },
        new()
        {
            Code = 0x19,
            Mnemonic = "ORA",
            Length = 3,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.AbsoluteY,
        },
        new()
        {
            Code = 0x01,
            Mnemonic = "ORA",
            Length = 2,
            Cycles = 6,
            AddressingMode =  AddressingMode.AddressingMode.IndirectX,
        },
        new()
        {
            Code = 0x11,
            Mnemonic = "ORA",
            Length = 2,
            Cycles = 5,
            AddressingMode =  AddressingMode.AddressingMode.IndirectY,
        },
        
        /*Shifts*/
        /*ASL*/
        /*
         *ASL (Arithmetic Shift Left) shifts all bits one position to the left. In doing so:

                The leftmost bit is shifted into the carry flag.
                A 0 is inserted into the rightmost bit.
                The result updates the zero flag (if the result is 0) and the negative flag (if the new highest bit is 1).

            It can operate on the accumulator or a memory operand.
         * 
         */
        new()
        {
            Code = 0x0a,
            Mnemonic = "ASL",
            Length = 1,
            Cycles = 2,
            AddressingMode =  AddressingMode.AddressingMode.NoneAddressing
        },
        new()
        {
            Code = 0x06,
            Mnemonic = "ASL",
            Length = 2,
            Cycles = 5,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPage,
        },
        new()
        {
            Code = 0x16,
            Mnemonic = "ASL",
            Length = 2,
            Cycles = 6,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPageX,
        },
        new()
        {
            Code = 0x0e,
            Mnemonic = "ASL",
            Length = 3,
            Cycles = 6,
            AddressingMode =  AddressingMode.AddressingMode.Absolute,
        },
        new()
        {
            Code = 0x1e,
            Mnemonic = "ASL",
            Length = 3,
            Cycles = 7,
            AddressingMode =  AddressingMode.AddressingMode.AbsoluteX,
        },
        
        /*LSR*/
        /*
         *LSR (Logical Shift Right) shifts every bit one position to the right.
         * The least significant bit is moved into the carry flag,
         * and a 0 is inserted into the most significant bit.
         * This always clears the negative flag (since the new MSB is 0) and sets the zero flag if the result is zero.
         */
        new()
        {
            Code = 0x4a,
            Mnemonic = "LSR",
            Length = 1,
            Cycles = 2,
            AddressingMode =  AddressingMode.AddressingMode.NoneAddressing,
        },
        new()
        {
            Code = 0x46,
            Mnemonic = "LSR",
            Length = 2,
            Cycles = 5,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPage,
        },
        new()
        {
            Code = 0x56,
            Mnemonic = "LSR",
            Length = 2,
            Cycles = 6,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPageX,
        },
        new()
        {
            Code = 0x4e,
            Mnemonic = "LSR",
            Length = 3,
            Cycles = 6,
            AddressingMode =  AddressingMode.AddressingMode.Absolute,
        },
        new()
        {
            Code = 0x5e,
            Mnemonic = "LSR",
            Length = 3,
            Cycles = 7,
            AddressingMode =  AddressingMode.AddressingMode.AbsoluteX,
        },
        
        /*ROL*/
        /*
         * ROL (Rotate Left) shifts all bits one position to the left while inserting the previous state of the carry flag into bit 0.
         * The leftmost bit is moved into the carry flag.
         * Like other shifts, it updates the zero flag (if the result is zero) and the negative flag based on the new high bit.
         */
        new()
        {
            Code = 0x2a,
            Mnemonic = "ROL",
            Length = 1,
            Cycles = 2,
            AddressingMode =  AddressingMode.AddressingMode.NoneAddressing,
        },
        new()
        {
            Code = 0x26,
            Mnemonic = "ROL",
            Length = 2,
            Cycles = 5,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPage,
        },
        new()
        {
            Code = 0x36,
            Mnemonic = "ROL",
            Length = 2,
            Cycles = 6,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPageX,
        },
        new()
        {
            Code = 0x2e,
            Mnemonic = "ROL",
            Length = 3,
            Cycles = 6,
            AddressingMode =  AddressingMode.AddressingMode.Absolute,
        },
        new()
        {
            Code = 0x3e,
            Mnemonic = "ROL",
            Length = 3,
            Cycles = 7,
            AddressingMode =  AddressingMode.AddressingMode.AbsoluteX,
        },
        
        /*ROR*/
        /*
         *ROR (Rotate Right) shifts every bit one position to the right.
         * The least significant bit moves into the carry flag, and the previous carry flag is inserted into the most significant bit.
         * It updates the zero flag if the result is zero and the negative flag based on the new high bit.
         */
        new()
        {
            Code = 0x6a,
            Mnemonic = "ROR",
            Length = 1,
            Cycles = 2,
            AddressingMode =  AddressingMode.AddressingMode.NoneAddressing,
        },
        new()
        {
            Code = 0x66,
            Mnemonic = "ROR",
            Length = 2,
            Cycles = 5,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPage,
        },
        new()
        {
            Code = 0x76,
            Mnemonic = "ROR",
            Length = 2,
            Cycles = 6,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPageX,
        },
        new()
        {
            Code = 0x6e,
            Mnemonic = "ROR",
            Length = 3,
            Cycles = 6,
            AddressingMode =  AddressingMode.AddressingMode.Absolute,
        },
        new()
        {
            Code = 0x7e,
            Mnemonic = "ROR",
            Length = 3,
            Cycles = 7,
            AddressingMode =  AddressingMode.AddressingMode.AbsoluteX,
        },
        
        /*INC*/
        new()
        {
            Code = 0xe6,
            Mnemonic = "INC",
            Length = 2,
            Cycles = 5,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPage,
        },
        new()
        {
            Code = 0xf6,
            Mnemonic = "INC",
            Length = 2,
            Cycles = 6,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPageX,
        },
        new()
        {
            Code = 0xee,
            Mnemonic = "INC",
            Length = 3,
            Cycles = 6,
            AddressingMode =  AddressingMode.AddressingMode.Absolute,
        },
        new()
        {
            Code = 0xfe,
            Mnemonic = "INC",
            Length = 3,
            Cycles = 7,
            AddressingMode =  AddressingMode.AddressingMode.AbsoluteX,
        },
        
        /*INX*/
        new()
        {
            Code = 0xe8,
            Mnemonic = "INX",
            Length = 1,
            Cycles = 2,
            AddressingMode =  AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*INY*/
        new()
        {
            Code = 0xc8,
            Mnemonic = "INY",
            Length = 1,
            Cycles = 2,
            AddressingMode =  AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*DEC*/
        new()
        {
            Code = 0xc6,
            Mnemonic = "DEC",
            Length = 2,
            Cycles = 5,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPage,
        },
        new()
        {
            Code = 0xd6,
            Mnemonic = "DEC",
            Length = 2,
            Cycles = 6,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPageX,
        },
        new()
        {
            Code = 0xce,
            Mnemonic = "DEC",
            Length = 3,
            Cycles = 6,
            AddressingMode =  AddressingMode.AddressingMode.Absolute,
        },
        new()
        {
            Code = 0xde,
            Mnemonic = "DEC",
            Length = 3,
            Cycles = 7,
            AddressingMode =  AddressingMode.AddressingMode.AbsoluteX,
        },
        
        /*DEX*/
        new()
        {
            Code = 0xca,
            Mnemonic = "DEX",
            Length = 1,
            Cycles = 2,
            AddressingMode =  AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*DEY*/
        new()
        {
            Code = 0x88,
            Mnemonic = "DEY",
            Length = 1,
            Cycles = 2,
            AddressingMode =  AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*CMP*/
        new()
        {
            Code = 0xc9,
            Mnemonic = "CMP",
            Length = 2,
            Cycles = 2,
            AddressingMode =  AddressingMode.AddressingMode.Immediate,
        },
        new()
        {
            Code = 0xc5,
            Mnemonic = "CMP",
            Length = 2,
            Cycles = 3,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPage,
        },
        new()
        {
            Code = 0xd5,
            Mnemonic = "CMP",
            Length = 2,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPageX,
        },
        new()
        {
            Code = 0xcd,
            Mnemonic = "CMP",
            Length = 3,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.Absolute,
        },
        new()
        {
            Code = 0xdd,
            Mnemonic = "CMP",
            Length = 3,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.AbsoluteX,
        },
        new()
        {
            Code = 0xd9,
            Mnemonic = "CMP",
            Length = 3,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.AbsoluteY,
        },
        new()
        {
            Code = 0xc1,
            Mnemonic = "CMP",
            Length = 2,
            Cycles = 6,
            AddressingMode =  AddressingMode.AddressingMode.IndirectX,
        },
        new()
        {
            Code = 0xd1,
            Mnemonic = "CMP",
            Length = 2,
            Cycles = 5,
            AddressingMode =  AddressingMode.AddressingMode.IndirectY,
        },
        
        /*CPY*/
        new()
        {
            Code = 0xc0,
            Mnemonic = "CPY",
            Length = 2,
            Cycles = 2,
            AddressingMode =  AddressingMode.AddressingMode.Immediate,
        },
        new()
        {
            Code = 0xc4,
            Mnemonic = "CPY",
            Length = 2,
            Cycles = 3,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPage,
        },
        new()
        {
            Code = 0xcc,
            Mnemonic = "CPY",
            Length = 3,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.Absolute,
        },
        
        /*CPX*/
        new()
        {
            Code = 0xe0,
            Mnemonic = "CPX",
            Length = 2,
            Cycles = 2,
            AddressingMode =  AddressingMode.AddressingMode.Immediate,
        },
        new()
        {
            Code = 0xe4,
            Mnemonic = "CPX",
            Length = 2,
            Cycles = 3,
            AddressingMode =  AddressingMode.AddressingMode.ZeroPage,
        },
        new()
        {
            Code = 0xec,
            Mnemonic = "CPX",
            Length = 3,
            Cycles = 4,
            AddressingMode =  AddressingMode.AddressingMode.Absolute,
        },
        
        /*Branching*/
        /*JMP*/
        new()
        {
            Code = 0x4c,
            Mnemonic = "JMP",
            Length = 3,
            Cycles = 3,
            AddressingMode =  AddressingMode.AddressingMode.NoneAddressing,
        },
        new()
        {
            Code = 0x6c,
            Mnemonic = "JMP",
            Length = 3,
            Cycles = 5,
            AddressingMode =  AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*JSR*/
        new()
        {
            Code = 0x20,
            Mnemonic = "JSR",
            Length = 3,
            Cycles = 6,
            AddressingMode =  AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*RTS*/
        new()
        {
            Code = 0x60,
            Mnemonic = "RTS",
            Length = 1,
            Cycles = 6,
            AddressingMode =  AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*RTI*/
        new()
        {
            Code = 0x40,
            Mnemonic = "RTI",
            Length = 1,
            Cycles = 6,
            AddressingMode =  AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*BNE*/
        new()
        {
            Code = 0xd0,
            Mnemonic = "BNE",
            Length = 2,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*BVS*/
        new()
        {
            Code = 0x70,
            Mnemonic = "BVS",
            Length = 2,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*BVC*/
        new()
        {
            Code = 0x50,
            Mnemonic = "BVC",
            Length = 2,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*BMI*/
        new()
        {
            Code = 0x30,
            Mnemonic = "BMI",
            Length = 2,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*BEQ*/
        new()
        {
            Code = 0xf0,
            Mnemonic = "BEQ",
            Length = 2,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*BCS*/
        new()
        {
            Code = 0xb0,
            Mnemonic = "BCS",
            Length = 2,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*BCC*/
        new()
        {
            Code = 0x90,
            Mnemonic = "BCC",
            Length = 2,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*BPL*/
        new()
        {
            Code = 0x10,
            Mnemonic = "BPL",
            Length = 2,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*BIT*/
        new()
        {
            Code = 0x24,
            Mnemonic = "BIT",
            Length = 2,
            Cycles = 3,
            AddressingMode = AddressingMode.AddressingMode.ZeroPage,
        },
        new()
        {
            Code = 0x2c,
            Mnemonic = "BIT",
            Length = 3,
            Cycles = 4,
            AddressingMode = AddressingMode.AddressingMode.Absolute,
        },

        /* Stores, Loads */
                /*LDA*/
        new()
        {
            Code = 0xa9,
            Mnemonic = "LDA",
            Length = 2,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.Immediate,
        },
        new()
        {
            Code = 0xa5,
            Mnemonic = "LDA",
            Length = 2,
            Cycles = 3,
            AddressingMode = AddressingMode.AddressingMode.ZeroPage,
        },
        new()
        {
            Code = 0xb5,
            Mnemonic = "LDA",
            Length = 2,
            Cycles = 4,
            AddressingMode = AddressingMode.AddressingMode.ZeroPageX,
        },
        new()
        {
            Code = 0xad,
            Mnemonic = "LDA",
            Length = 3,
            Cycles = 4,
            AddressingMode = AddressingMode.AddressingMode.Absolute,
        },
        new()
        {
            Code = 0xbd,
            Mnemonic = "LDA",
            Length = 3,
            Cycles = 4, // +1 if page crossed
            AddressingMode = AddressingMode.AddressingMode.AbsoluteX,
        },
        new()
        {
            Code = 0xb9,
            Mnemonic = "LDA",
            Length = 3,
            Cycles = 4, // +1 if page crossed
            AddressingMode = AddressingMode.AddressingMode.AbsoluteY,
        },
        new()
        {
            Code = 0xa1,
            Mnemonic = "LDA",
            Length = 2,
            Cycles = 6,
            AddressingMode = AddressingMode.AddressingMode.IndirectX,
        },
        new()
        {
            Code = 0xb1,
            Mnemonic = "LDA",
            Length = 2,
            Cycles = 5, // +1 if page crossed
            AddressingMode = AddressingMode.AddressingMode.IndirectY,
        },

                /*LDX*/
        new()
        {
            Code = 0xa2,
            Mnemonic = "LDX",
            Length = 2,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.Immediate,
        },
        new()
        {
            Code = 0xa6,
            Mnemonic = "LDX",
            Length = 2,
            Cycles = 3,
            AddressingMode = AddressingMode.AddressingMode.ZeroPage,
        },
        new()
        {
            Code = 0xb6,
            Mnemonic = "LDX",
            Length = 2,
            Cycles = 4,
            AddressingMode = AddressingMode.AddressingMode.ZeroPageY,
        },
        new()
        {
            Code = 0xae,
            Mnemonic = "LDX",
            Length = 3,
            Cycles = 4,
            AddressingMode = AddressingMode.AddressingMode.Absolute,
        },
        new()
        {
            Code = 0xbe,
            Mnemonic = "LDX",
            Length = 3,
            Cycles = 4, // +1 if page crossed
            AddressingMode = AddressingMode.AddressingMode.AbsoluteY,
        },
        
        /*LDY*/
        new()
        {
            Code = 0xa0,
            Mnemonic = "LDY",
            Length = 2,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.Immediate,
        },
        new()
        {
            Code = 0xa4,
            Mnemonic = "LDY",
            Length = 2,
            Cycles = 3,
            AddressingMode = AddressingMode.AddressingMode.ZeroPage,
        },
        new()
        {
            Code = 0xb4,
            Mnemonic = "LDY",
            Length = 2,
            Cycles = 4,
            AddressingMode = AddressingMode.AddressingMode.ZeroPageX,
        },
        new()
        {
            Code = 0xac,
            Mnemonic = "LDY",
            Length = 3,
            Cycles = 4,
            AddressingMode = AddressingMode.AddressingMode.Absolute,
        },
        new()
        {
            Code = 0xbc,
            Mnemonic = "LDY",
            Length = 3,
            Cycles = 4, // +1 if page crossed
            AddressingMode = AddressingMode.AddressingMode.AbsoluteX,
        },

        /*STA*/
        new()
        {
            Code = 0x85,
            Mnemonic = "STA",
            Length = 2,
            Cycles = 3,
            AddressingMode = AddressingMode.AddressingMode.ZeroPage,
        },
        new()
        {
            Code = 0x95,
            Mnemonic = "STA",
            Length = 2,
            Cycles = 4,
            AddressingMode = AddressingMode.AddressingMode.ZeroPageX,
        },
        new()
        {
            Code = 0x8d,
            Mnemonic = "STA",
            Length = 3,
            Cycles = 4,
            AddressingMode = AddressingMode.AddressingMode.Absolute,
        },
        new()
        {
            Code = 0x9d,
            Mnemonic = "STA",
            Length = 3,
            Cycles = 5,
            AddressingMode = AddressingMode.AddressingMode.AbsoluteX,
        },
        new()
        {
            Code = 0x99,
            Mnemonic = "STA",
            Length = 3,
            Cycles = 5,
            AddressingMode = AddressingMode.AddressingMode.AbsoluteY,
        },
        new()
        {
            Code = 0x81,
            Mnemonic = "STA",
            Length = 2,
            Cycles = 6,
            AddressingMode = AddressingMode.AddressingMode.IndirectX,
        },
        new()
        {
            Code = 0x91,
            Mnemonic = "STA",
            Length = 2,
            Cycles = 6,
            AddressingMode = AddressingMode.AddressingMode.IndirectY,
        },

        /*STX*/
        new()
        {
            Code = 0x86,
            Mnemonic = "STX",
            Length = 2,
            Cycles = 3,
            AddressingMode = AddressingMode.AddressingMode.ZeroPage,
        },
        new()
        {
            Code = 0x96,
            Mnemonic = "STX",
            Length = 2,
            Cycles = 4,
            AddressingMode = AddressingMode.AddressingMode.ZeroPageY,
        },
        new()
        {
            Code = 0x8e,
            Mnemonic = "STX",
            Length = 3,
            Cycles = 4,
            AddressingMode = AddressingMode.AddressingMode.Absolute,
        },

        /*STY*/
        new()
        {
            Code = 0x84,
            Mnemonic = "STY",
            Length = 2,
            Cycles = 3,
            AddressingMode = AddressingMode.AddressingMode.ZeroPage,
        },
        new()
        {
            Code = 0x94,
            Mnemonic = "STY",
            Length = 2,
            Cycles = 4,
            AddressingMode = AddressingMode.AddressingMode.ZeroPageX,
        },
        new()
        {
            Code = 0x8c,
            Mnemonic = "STY",
            Length = 3,
            Cycles = 4,
            AddressingMode = AddressingMode.AddressingMode.Absolute,
        },
        
        /*CLD*/
        new()
        {
            Code = 0xD8,
            Mnemonic = "CLD",
            Length = 1,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*CLI*/
        new()
        {
            Code = 0x58,
            Mnemonic = "CLI",
            Length = 1,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*CLV*/
        new()
        {
            Code = 0xB8,
            Mnemonic = "CLV",
            Length = 1,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*CLC*/
        new()
        {
            Code = 0x18,
            Mnemonic = "CLC",
            Length = 1,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*SEC*/
        new()
        {
            Code = 0x38,
            Mnemonic = "SEC",
            Length = 1,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*SEI*/
        new()
        {
            Code = 0x78,
            Mnemonic = "SEI",
            Length = 1,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*SED*/
        new()
        {
            Code = 0xF8,
            Mnemonic = "SED",
            Length = 1,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },

        /*TAX*/
        new()
        {
            Code = 0xAA,
            Mnemonic = "TAX",
            Length = 1,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*TAY*/
        new()
        {
            Code = 0xA8,
            Mnemonic = "TAY",
            Length = 1,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*TSX*/
        new()
        {
            Code = 0xBA,
            Mnemonic = "TSX",
            Length = 1,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*TXA*/
        new()
        {
            Code = 0x8A,
            Mnemonic = "TXA",
            Length = 1,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*TXS*/
        new()
        {
            Code = 0x9A,
            Mnemonic = "TXS",
            Length = 1,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*TYA*/
        new()
        {
            Code = 0x98,
            Mnemonic = "TYA",
            Length = 1,
            Cycles = 2,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },

        /*PHA*/
        new()
        {
            Code = 0x48,
            Mnemonic = "PHA",
            Length = 1,
            Cycles = 3,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*PLA*/
        new()
        {
            Code = 0x68,
            Mnemonic = "PLA",
            Length = 1,
            Cycles = 4,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*PHP*/
        new()
        {
            Code = 0x08,
            Mnemonic = "PHP",
            Length = 1,
            Cycles = 3,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
        
        /*PLP*/
        new()
        {
            Code = 0x28,
            Mnemonic = "PLP",
            Length = 1,
            Cycles = 4,
            AddressingMode = AddressingMode.AddressingMode.NoneAddressing,
        },
    ];

    public static readonly Dictionary<byte, OpCode> OpCodeMap = BuildOpCodes();

    private static Dictionary<byte, OpCode> BuildOpCodes()
    {
        var map = new Dictionary<byte, OpCode>();
        foreach (var op in Codes)
        {
            map[op.Code] = op;
        }
        return map;
    }
}