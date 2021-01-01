using cmm_vm.cons;
using cmm_vm.ins;
using System;

namespace cmm_vm
{
    class Program
    {
        static void Main(string[] args)
        {
            Instruction[] instructions = new Instruction[]
            {
                new Instruction(InstructionType.DC_FUN, "fac", 0),
                new Instruction(InstructionType.ARG_INT, "x", 1),
                new Instruction(InstructionType.END_ARG, "", 2),
                new Instruction(InstructionType.LD_VAR, "x", 3),
                new Instruction(InstructionType.LD_INT, "0", 4),
                new Instruction(InstructionType.OP_LES, "", 5),
                new Instruction(InstructionType.JBY_IF, "2", 6),
                new Instruction(InstructionType.JUMPBY, "3", 7),
                new Instruction(InstructionType.LD_INT, "-1", 8),
                new Instruction(InstructionType.RETURN, "", 9),
                new Instruction(InstructionType.LD_VAR, "x", 10),
                new Instruction(InstructionType.LD_INT, "0", 11),
                new Instruction(InstructionType.OP_EQL, "", 12),
                new Instruction(InstructionType.JBY_IF, "2", 13),
                new Instruction(InstructionType.JUMPBY, "3", 14),
                new Instruction(InstructionType.LD_INT, "1", 15),
                new Instruction(InstructionType.RETURN, "", 16),
                new Instruction(InstructionType.LD_VAR, "x", 17),
                new Instruction(InstructionType.LD_VAR, "x", 18),
                new Instruction(InstructionType.LD_INT, "1", 19),
                new Instruction(InstructionType.OP_SUB, "", 20),
                new Instruction(InstructionType.CALL_FUN, "fac", 21),
                new Instruction(InstructionType.OP_MUL, "", 22),
                new Instruction(InstructionType.RETURN, "", 23),
                new Instruction(InstructionType.END_FUN, "", 24),
                new Instruction(InstructionType.DC_FUN, "main", 25),
                new Instruction(InstructionType.END_ARG, "", 26),
                new Instruction(InstructionType.DC_VAR_INT, "x", 27),
                new Instruction(InstructionType.LD_INT, "0", 28),
                new Instruction(InstructionType.ST_VAR, "x", 29),
                new Instruction(InstructionType.DC_VAR_INT, "fx", 30),
                new Instruction(InstructionType.LD_INT, "1", 31),
                new Instruction(InstructionType.ST_VAR, "fx", 33),
                new Instruction(InstructionType.LD_VAR, "fx", 34),
                new Instruction(InstructionType.LD_INT, "0", 35),
                new Instruction(InstructionType.OP_GRT, "", 36),
                new Instruction(InstructionType.JBY_IF, "2", 37),
                new Instruction(InstructionType.JUMPBY, "13", 38),
                new Instruction(InstructionType.LD_VAR, "x", 39),
                new Instruction(InstructionType.CALL_FUN, "fac", 40),
                new Instruction(InstructionType.ST_VAR, "fx", 41),
                new Instruction(InstructionType.LD_VAR, "x", 42),
                new Instruction(InstructionType.WRITE, "", 43),
                new Instruction(InstructionType.LD_VAR, "fx", 44),
                new Instruction(InstructionType.WRITE, "", 45),
                new Instruction(InstructionType.LD_VAR, "x", 46),
                new Instruction(InstructionType.LD_INT, "1", 47),
                new Instruction(InstructionType.OP_ADD, "", 48),
                new Instruction(InstructionType.ST_VAR, "x", 49),
                new Instruction(InstructionType.JUMPBY, "-16", 50),
                new Instruction(InstructionType.LD_INT, "0", 51),
                new Instruction(InstructionType.RETURN, "", 52),
                new Instruction(InstructionType.END_FUN, "", 53),
                new Instruction(InstructionType.CALL_FUN, "main", 54),
            };

            Instruction[] instructions2 = new Instruction[]
            {
                new Instruction(InstructionType.LD_INT, "3", 1),
                new Instruction(InstructionType.DC_VAR_A_INT, "array_1", 1),
                new Instruction(InstructionType.LD_VAR, "array_1", 1),
                new Instruction(InstructionType.LD_INT, "0", 1),
                new Instruction(InstructionType.LD_ELE_INT, "", 1),
                new Instruction(InstructionType.WRITE, "", 1),
                new Instruction(InstructionType.LD_VAR, "array_1", 1),
                new Instruction(InstructionType.LD_INT, "0", 1),
                new Instruction(InstructionType.LD_INT, "12138", 1),
                new Instruction(InstructionType.ST_ADR, "", 1),
                new Instruction(InstructionType.LD_VAR, "array_1", 1),
                new Instruction(InstructionType.LD_INT, "0", 1),
                new Instruction(InstructionType.LD_ELE_INT, "", 1),
                new Instruction(InstructionType.WRITE, "", 1),
            };

            VirtualMachine vm = new VirtualMachine(instructions);

            vm.ExeToEnd();
        }
    }
}
