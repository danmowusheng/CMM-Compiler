using cmm_vm.cons;
using cmm_vm.error;
using cmm_vm.ins;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace cmm_vm
{
    /// <summary>
    /// 虚拟机
    /// </summary>
    class VirtualMachine
    {
        //待执行指令
        private Instruction[] instructions;

        //环境
        private readonly VMCore core;

        //VM类型异常报告
        private readonly ExceptionReporter reporter;

        //当前指令索引
        private int currentInsIndex = 0;

        //当前指令对应源代码的行号
        public int GetCurrentInsLine()
        {
            return instructions[currentInsIndex].LineNum;
        }

        //需要指定输入输出方法
        public ReadDelegate ReadMethod { get; set; } = new ReadDelegate(Console.ReadLine);
        public WriteDelegate WriteMethod { get; set; } = new WriteDelegate(Console.WriteLine);

        /// <summary>
        /// 需要手动指定待执行的指令和输入输出方法
        /// </summary>
        public VirtualMachine()
        {
            this.reporter = new ExceptionReporter(this);
            this.core = new VMCore(this.reporter);
        }

        /// <summary>
        /// 需要手动指定输入输出方法
        /// </summary>
        /// <param name="inss"> 待执行指令序列 </param>
        public VirtualMachine(Instruction[] inss):this()
        {
            instructions = inss;
        }

        /// <summary>
        /// 设置待执行指令序列
        /// </summary>
        /// <param name="inss"> 待执行指令序列 </param>
        public void SetInstructions(Instruction[] inss)
        {
            instructions = inss;
        }

        #region 执行及调试命令
        /// <summary>
        /// 单步执行指令
        /// </summary>
        /// <returns> 是否成功执行,当指令全部执行完时再执行会返回false </returns>
        public Boolean ExecuteOne()
        {
            if(currentInsIndex >= instructions.Length)
            {
                core.EndAll();
                return false;
            }

            Instruction ins = instructions[currentInsIndex];
            #region 测试使用
            //Console.WriteLine("\n=================#一行中间代码被执行了#================");
            //Console.WriteLine("源代码行号:\t" + ins.LineNum + "\t中间代码实际位置:\t" + currentInsIndex);
            //List<ValueType> stackValues = ShowStack().ToList();
            //foreach (ValueType value in stackValues)
            //{
            //    Console.WriteLine("栈中的值:\t" + value);
            //}
            //Dictionary<String, ValueType> d = core.GetVariableTable();
            //foreach (String s in d.Keys)
            //{
            //    d.TryGetValue(s, out ValueType val);
            //    Console.WriteLine("变量:" + s + "\t值:" + val);
            //}
            #endregion
            ProcessIns(ins.Type, ins.Argument);
            return true;
        }

        /// <summary>
        /// 执行到下一个断点
        /// </summary>
        /// <returns> 是否停止,在指令全部执行结束后如果还没停止则返回false </returns>
        public Boolean ExeToPoint()
        {
            while (ExecuteOne())
            {
                if(instructions[currentInsIndex].Breaked == true)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 执行全部指令
        /// </summary>
        public void ExeToEnd()
        {
            while (ExecuteOne()) ;
        }

        /// <summary>
        /// 设置断点
        /// </summary>
        /// <param name="lineNum"> 指令行号(注意不是源代码行号,是指令行号,从0开始索引) </param>
        public void SetBreakPoint(int lineNum)
        {
            instructions[lineNum].Breaked = true;
        }

        /// <summary>
        /// 取消设置断点
        /// </summary>
        /// <param name="lineNum"> 指令行号(注意不是源代码行号,是指令行号,从0开始索引) </param>
        public void UnSetBreakPoint(int lineNum)
        {
            instructions[lineNum].Breaked = false;
        }

        /// <summary>
        /// 显示栈中值
        /// </summary>
        /// <returns> 栈中的值列表,最左是栈顶 </returns>
        public List<ValueType> ShowStack()
        {
            return core.ShowStack();
        }

        /// <summary>
        /// 获取变量表(不同scope中同名的变量只会显示最近scope的变量值)
        /// </summary>
        /// <returns> 变量表字典 </returns>
        public Dictionary<String, ValueType> GetVariableTable()
        {
            return core.GetVariableTable();
        }

        /// <summary>
        /// 逐层搜索scope直到获取某个变量的值
        /// </summary>
        /// <param name="varName"> 变量名 </param>
        /// <returns> 最近scope变量值 </returns>
        public ValueType GetVariable(String varName)
        {
            return core.GetVariable(varName);
        }

        /// <summary>
        /// 查看某个数组
        /// </summary>
        /// <param name="arrayName"> 数组名 </param>
        /// <returns> 数组 </returns>
        public Array GetArray(String arrayName)
        {
            return core.GetArray(arrayName);
        }

        #endregion

        #region 处理指令
        //处理指令
        private void ProcessIns(InstructionType insType, String arg)
        {
            switch (insType)
            {
                case InstructionType.NOP:
                    core.Pass();
                    break;
                case InstructionType.LD_INT:
                    core.PushIntConst(GetIntFromArg(arg), IntUsage.DEFAULT);
                    break;
                case InstructionType.LD_REAL:
                    core.PushRealConst(GetDoubleFromArg(arg));
                    break;
                case InstructionType.LD_VAR:
                    core.PushVariable(arg);
                    break;
                case InstructionType.LD_ELE_INT:
                    core.PushIntByIndexAddress();
                    break;
                case InstructionType.LD_ELE_REAL:
                    core.PushRealByIndexAddress();
                    break;
                case InstructionType.DUPTOP:
                    core.Dup();
                    break;
                case InstructionType.ST_VAR:
                    core.StoreVariable(arg);
                    break;
                case InstructionType.ST_ADR:
                    core.StoreValueIntoIndexOfArray();
                    break;
                case InstructionType.DC_VAR_INT:
                    core.DeclareIntVar(arg);
                    break;
                case InstructionType.DC_VAR_REAL:
                    core.DeclareRealVar(arg);
                    break;
                //case InstructionType.DC_VAR_ADR:
                //    core.declareArrayVar(arg);
                //    break;
                case InstructionType.DC_VAR_A_INT:
                    core.DeclareIntArray(arg);
                    break;
                case InstructionType.PUSH_VT:
                    core.NewScope();
                    break;
                case InstructionType.POP_VT:
                    core.DeleteScope();
                    break;
                case InstructionType.DC_FUN:
                    core.DeclareFunction(arg, currentInsIndex + 1);
                    currentInsIndex = FindIndexAfterEND_FC();
                    if(currentInsIndex == -1)
                    {
                        throw new Exception("cmm.vm::VirtualMachine::ProcessIns: 在定义函数指令后没有函数结束指令");
                    }
                    return;
                case InstructionType.END_FUN:
                    throw new Exception("cmm.vm::VirtualMachine::ProcessIns: 执行了不应该被执行的指令");
                case InstructionType.ARG_INT:
                    IConstant retAddr = core.PopOperand();
                    core.DeclareIntVar(arg);
                    core.StoreVariable(arg);
                    core.PushOperand(retAddr);
                    break;
                case InstructionType.ARG_REAL:
                    retAddr = core.PopOperand();
                    core.DeclareRealVar(arg);
                    core.StoreVariable(arg);
                    core.PushOperand(retAddr);
                    break;
                case InstructionType.ARG_ADR:
                    retAddr = core.PopOperand();
                    //数组地址是int类型
                    core.DeclareIntVar(arg);
                    core.StoreVariable(arg);
                    core.PushOperand(retAddr);
                    break;
                case InstructionType.END_ARG:
                    IntConstant retAddress = core.PopIntConst();
                    IntConstant stackFrame = new IntConstant((int)retAddress.Value, IntUsage.STACK_HEAD);
                    core.PushOperand(stackFrame);
                    break;
                case InstructionType.CALL_FUN:
                    //存下一个地址为返回地址,供存入栈帧头使用
                    core.PushOperand(new IntConstant(currentInsIndex + 1, IntUsage.INSADDR));
                    core.NewScope();
                    //设置当前地址为函数地址
                    currentInsIndex = core.GetFunction(arg);
                    return;
                case InstructionType.RETURN:
                    IConstant retval = core.PopOperand();
                    IConstant stacdHeadCand = core.PopOperand();
                    while(stacdHeadCand.Type != NumericType.INT || ((IntConstant)stacdHeadCand).Usage != IntUsage.STACK_HEAD)
                    {
                        stacdHeadCand = core.PopOperand();
                    }
                    core.PushOperand(retval);
                    core.DeleteScope();
                    currentInsIndex = (int)stacdHeadCand.Value;
                    return;
                case InstructionType.JUMPBY:
                    currentInsIndex += GetIntFromArg(arg);
                    return;
                case InstructionType.JBY_IF:
                    int predicate = (int)core.PopIntConst().Value;
                    if(predicate == 0)
                    {
                        break;
                    }
                    else
                    {
                        currentInsIndex += GetIntFromArg(arg);
                        return;
                    }
                case InstructionType.OP_ADD:
                    CalAndPush(Add);
                    break;
                case InstructionType.OP_SUB:
                    CalAndPush(Sub);
                    break;
                case InstructionType.OP_MUL:
                    CalAndPush(Mul);
                    break;
                case InstructionType.OP_DIV:
                    CalAndPush(Div);
                    break;
                case InstructionType.OP_EQL:
                    CalAndPush(Equals);
                    break;
                case InstructionType.OP_N_E:
                    CalAndPush(NotEquals);
                    break;
                case InstructionType.OP_GRT:
                    CalAndPush(GreaterThan);
                    break;
                case InstructionType.OP_G_E:
                    CalAndPush(GreaterEquals);
                    break;
                case InstructionType.OP_LES:
                    CalAndPush(LessThan);
                    break;
                case InstructionType.OP_L_E:
                    CalAndPush(LessEquals);
                    break;
                case InstructionType.OP_NOT:
                    CalAndPush(Not);
                    break;
                case InstructionType.OP_AND:
                    CalAndPush(And);
                    break;
                case InstructionType.OP_OR:
                    CalAndPush(Or);
                    break;
                case InstructionType.OP_NEG:
                    CalAndPush(Neg);
                    break;
                case InstructionType.RD_INT:
                    ReadInt();
                    break;
                case InstructionType.RD_REAL:
                    ReadReal();
                    break;
                case InstructionType.WRITE:
                    Write();
                    break;
            }
            currentInsIndex++;
        }

        #endregion

        #region 虚拟机计算函数
        private int FindIndexAfterEND_FC()
        {
            while(currentInsIndex < instructions.Length)
            {
                if(instructions[currentInsIndex].Type == InstructionType.END_FUN)
                {
                    return currentInsIndex + 1;
                }
                currentInsIndex++;
            }
            return -1;
        }

        private void CalAndPush(Fun1 fun)
        {
            IConstant arg = core.PopOperand();
            core.PushOperand(fun.Invoke(arg));
        }

        private void CalAndPush(Fun2 fun)
        {
            IConstant arg2 = core.PopOperand();
            IConstant arg1 = core.PopOperand();
            core.PushOperand(fun.Invoke(arg1, arg2));
        }

        private delegate IConstant Fun1(IConstant arg1);

        private delegate IConstant Fun2(IConstant arg1, IConstant arg2);

        #region 算术和逻辑操作
        //加法
        private static IConstant Add(IConstant arg1, IConstant arg2)
        {
            //只要一个参数为实数，则按照两个函数都为实数计算
            if(arg1.Type == NumericType.REAL || arg2.Type == NumericType.REAL)
            {
                return new RealConstant(Convert.ToDouble(arg1.Value) + Convert.ToDouble(arg2.Value));
            }
            //否则两个都是整数
            return new IntConstant((int)arg1.Value + (int)arg2.Value);
        }

        //减法
        private static IConstant Sub(IConstant arg1, IConstant arg2)
        {
            //只要一个参数为实数，则按照两个函数都为实数计算
            if (arg1.Type == NumericType.REAL || arg2.Type == NumericType.REAL)
            {
                return new RealConstant(Convert.ToDouble(arg1.Value) - Convert.ToDouble(arg2.Value));
            }
            //否则两个都是整数
            return new IntConstant((int)arg1.Value - (int)arg2.Value);
        }

        //乘法
        private static IConstant Mul(IConstant arg1, IConstant arg2)
        {
            //只要一个参数为实数，则按照两个函数都为实数计算
            if (arg1.Type == NumericType.REAL || arg2.Type == NumericType.REAL)
            {
                return new RealConstant(Convert.ToDouble(arg1.Value) * Convert.ToDouble(arg2.Value));
            }
            //否则两个都是整数
            return new IntConstant((int)arg1.Value * (int)arg2.Value);
        }

        //除法
        private static IConstant Div(IConstant arg1, IConstant arg2)
        {
            //只要一个参数为实数，则按照两个函数都为实数计算
            if (arg1.Type == NumericType.REAL || arg2.Type == NumericType.REAL)
            {
                return new RealConstant(Convert.ToDouble(arg1.Value) / Convert.ToDouble(arg2.Value));
            }
            //否则两个都是整数
            return new IntConstant((int)arg1.Value / (int)arg2.Value);
        }

        //取反
        private static IConstant Neg(IConstant arg)
        {
            //实数
            if (arg.Type == NumericType.REAL)
            {
                return new RealConstant(-Convert.ToDouble(arg.Value));
            }
            //整数
            return new IntConstant(-(int)arg.Value);
        }

        //相等
        private static IConstant Equals(IConstant arg1, IConstant arg2)
        {
            if (Convert.ToDouble(arg1.Value) == Convert.ToDouble(arg2.Value))
            {
                return new IntConstant(1);
            }
            return new IntConstant(0);
        }

        //不相等
        private static IConstant NotEquals(IConstant arg1, IConstant arg2)
        {
            if (Convert.ToDouble(arg1.Value) != Convert.ToDouble(arg2.Value))
            {
                return new IntConstant(1);
            }
            return new IntConstant(0);
        }

        //大于
        private static IConstant GreaterThan(IConstant arg1, IConstant arg2)
        {
            if (Convert.ToDouble(arg1.Value) > Convert.ToDouble(arg2.Value))
            {
                return new IntConstant(1);
            }
            return new IntConstant(0);
        }

        //小于
        private static IConstant LessThan(IConstant arg1, IConstant arg2)
        {
            if (Convert.ToDouble(arg1.Value) < Convert.ToDouble(arg2.Value))
            {
                return new IntConstant(1);
            }
            return new IntConstant(0);
        }

        //大于等于
        private static IConstant GreaterEquals(IConstant arg1, IConstant arg2)
        {
            if (Convert.ToDouble(arg1.Value) >= Convert.ToDouble(arg2.Value))
            {
                return new IntConstant(1);
            }
            return new IntConstant(0);
        }

        //小于等于
        private static IConstant LessEquals(IConstant arg1, IConstant arg2)
        {
            if (Convert.ToDouble(arg1.Value) <= Convert.ToDouble(arg2.Value))
            {
                return new IntConstant(1);
            }
            return new IntConstant(0);
        }

        //非
        private static IConstant Not(IConstant arg)
        {
            if (Convert.ToDouble(arg.Value) == 0)
            {
                return new IntConstant(1);
            }
            return new IntConstant(0);
        }

        //与
        private static IConstant And(IConstant arg1, IConstant arg2)
        {
            if (Convert.ToDouble(arg1.Value) != 0 && Convert.ToDouble(arg2.Value) != 0)
            {
                return new IntConstant(1);
            }
            return new IntConstant(0);
        }

        //或
        private static IConstant Or(IConstant arg1, IConstant arg2)
        {
            if (Convert.ToDouble(arg1.Value) != 0 || Convert.ToDouble(arg2.Value) != 0)
            {
                return new IntConstant(1);
            }
            return new IntConstant(0);
        }

        #endregion

        #endregion

        #region IO

        public delegate String ReadDelegate();
        public delegate void WriteDelegate(String str);

        //输入
        void ReadInt()
        {
            int val = Convert.ToInt32(ReadMethod);
            core.PushIntConst(val, IntUsage.DEFAULT);
        }

        void ReadReal()
        {
            double val = Convert.ToDouble(ReadMethod);
            core.PushRealConst(val);
        }

        //输出
        void Write()
        {
            String s = core.PopOperand().ToString();
            WriteMethod(s);
        }

        #endregion

        private int GetIntFromArg(String arg)
        {
            int retVal = 0;
            try
            {
               retVal = Convert.ToInt32(arg);
            }
            catch
            {
                throw reporter.Throw("参数类型错误", VMExceptionType.WRONG_TYPE);
            }
            return retVal;
        }

        private double GetDoubleFromArg(String arg)
        {
            double retVal = 0;
            try
            {
                retVal = Convert.ToDouble(arg);
            }
            catch
            {
                throw reporter.Throw("参数类型错误", VMExceptionType.WRONG_TYPE);
            }
            return retVal;
        }
    }
}
