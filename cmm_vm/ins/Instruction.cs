using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace cmm_vm.ins
{
    class Instruction
    {
        public InstructionType Type { get; set; } = InstructionType.NOP;
        public String Argument { get; set; }
        public int LineNum { get; set; }
        public Boolean Breaked { get; set; } = false;

        public Instruction() { }

        public Instruction(InstructionType type) : this() => Type = type;

        public Instruction(InstructionType type, String arg) : this(type) => Argument = arg;

        public Instruction(InstructionType type, String arg, int lineNum) : this(type, arg) => LineNum = lineNum;
    }

    enum InstructionType
    {
        NOP,        //空指令
        LD_INT,     //入栈整型
        LD_REAL,    //入栈实型
        LD_VAR,     //入栈变量
        LD_ELE_INT, //入栈整型数组元素
        LD_ELE_REAL,//入栈实型数组元素
        DUPTOP,     //复制栈顶元素
        ST_VAR,     //储存到变量
        ST_ADR,     //储存到数组
        DC_VAR_INT, //声明整型变量
        DC_VAR_REAL,//声明实型变量
        //DC_VAR_ADR, //声明堆地址,弃用
        DC_VAR_A_INT,   //声明并初始化整型数组为{0, ...}
        DC_VAR_A_REAL,  //声明并初始化实型数组为{0.0, ...}
        PUSH_VT,    //新变量表
        POP_VT,     //弹出变量表
        DC_FUN,     //声明函数
        END_FUN,    //结束函数
        ARG_INT,    //声明整型参数
        ARG_REAL,   //声明实型参数
        ARG_ADR,    //声明堆地址参数
        END_ARG,    //结束参数定义
        CALL_FUN,   //调用函数
        RETURN,     //结束调用
        JUMPBY,     //跳转到相对位置
        JBY_IF,     //如果为真则跳转到相对位置
        OP_ADD,     //  +
        OP_SUB,     //  -
        OP_MUL,     //  *
        OP_DIV,     //  /
        OP_EQL,     //  ==
        OP_N_E,     //  <>
        OP_GRT,     //  >
        OP_G_E,     //  >=
        OP_LES,     //  <
        OP_L_E,     //  <=
        OP_NOT,     //  !
        OP_AND,     //  &&
        OP_OR,      //  ||
        OP_NEG,     //  -
        RD_INT,     //输入整型
        RD_REAL,    //输入实型
        WRITE,      //输出
    }
}
