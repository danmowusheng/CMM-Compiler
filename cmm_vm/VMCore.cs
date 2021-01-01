using cmm_vm.cons;
using cmm_vm.error;
using cmm_vm.heap;
using cmm_vm.var;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmm_vm
{
    class VMCore
    {
        private readonly ExceptionReporter reporter;

        private readonly Stack<IConstant> operandStack = new Stack<IConstant>();

        private readonly VariableTableStack variableTableStack;

        private readonly Heap heap;

        public VMCore(ExceptionReporter reporter)
        {
            this.reporter = reporter;
            this.variableTableStack = new VariableTableStack(reporter);
            this.heap = new Heap(reporter);
        }

        #region 显示环境
        public Dictionary<String, ValueType> GetVariableTable()
        {
            Dictionary<String, ValueType> vars = new Dictionary<string, ValueType>();
            //从栈底开始返回,array[0]是栈顶所以需要reverse
            foreach(VariableTable table in variableTableStack.GetTableStack().ToArray().Reverse())
            {
                foreach(String varName in table.GetVars().Keys)
                {
                    IConstant newVal;

                    if (vars.TryGetValue(varName, out ValueType value))
                    {
                        table.GetValue(varName, out newVal);
                        if(newVal.Type == NumericType.INT)
                        {
                            vars[varName] = (int)newVal.Value;
                        }
                        else
                        {
                            vars[varName] = (double)newVal.Value;
                        }

                        continue;
                    }
                    table.GetValue(varName, out newVal);
                    if (newVal.Type == NumericType.INT)
                    {
                        vars.Add(varName, (int)newVal.Value);
                    }
                    else
                    {
                        vars.Add(varName, (double)newVal.Value);
                    }
                }
            }

            return vars;
        }

        public ValueType GetVariable(String name)
        {
            IConstant constant = variableTableStack.GetValue(name);
            if(constant.Type == NumericType.INT)
            {
                return (int)constant.Value;
            }
            else
            {
                return (double)constant.Value;
            }
        }

        public ValueType GetVariableNoErr(String name)
        {
            IConstant constant = variableTableStack.GetValueNoErr(name);
            if(constant == null)
            {
                return null;
            }
            if (constant.Type == NumericType.INT)
            {
                return (int)constant.Value;
            }
            else
            {
                return (double)constant.Value;
            }
        }

        public List<ValueType> ShowStack()
        {
            List<ValueType> valueList = new List<ValueType>();
            foreach(IConstant constant in operandStack.ToList())
            {
                if(constant.Type == NumericType.INT)
                {
                    valueList.Add((int)constant.Value);
                }
                else
                {
                    valueList.Add((int)constant.Value);
                }
            }
            return valueList;
        }

        //获取不到对应数组时返回空值
        public Array GetArray(String varName)
        {
            ValueType address = GetVariableNoErr(varName);
            if(address != null)
            {
                if (address.GetType().Equals(typeof(int)))
                {
                    return heap.GetArray((int)address);
                }
            }
            return null;
        }

        #endregion

        //空
        public void Pass()
        {
            //do Nothing
        }

        //结束任务
        public void EndAll()
        {
            operandStack.Clear();
            variableTableStack.Flush();
            heap.Flush();
        }

        #region 栈操作
        //入栈
        public void PushOperand(IConstant operand)
        {
            operandStack.Push(operand);
        }

        //出栈
        public IConstant PopOperand()
        {
            //只要语义分析正确,这里不会出现空栈的情况,出现了就抛异常
            return operandStack.Pop();
        }

        //整数入栈
        public void PushIntConst(int val, IntUsage usage)
        {
            operandStack.Push(new IntConstant(val, usage));
        }

        //实数入栈
        public void PushRealConst(double val)
        {
            operandStack.Push(new RealConstant(val));
        }

        //变量入栈
        public void PushVariable(String varName)
        {
            operandStack.Push(variableTableStack.GetValue(varName));
        }

        //整型数组元素入栈
        public void PushIntByIndexAddress()
        {
            IntConstant index = PopIntConst();
            IntConstant address = PopIntConst();
            operandStack.Push(heap.GetIntValue((int)address.Value, (int)index.Value));
        }

        //实型数组元素入栈
        public void PushRealByIndexAddress()
        {
            IntConstant index = PopIntConst();
            IntConstant address = PopIntConst();
            operandStack.Push(heap.GetRealValue((int)address.Value, (int)index.Value));
        }

        //整数出栈
        public IntConstant PopIntConst()
        {
            IConstant constant = operandStack.Pop();
            if(constant.Type != NumericType.INT)
            {
                throw reporter.Throw("值类型错误", VMExceptionType.WRONG_TYPE);
            }
            return (IntConstant)constant;
        }

        //实数出栈
        public RealConstant PopRealConst()
        {
            IConstant constant = operandStack.Pop();
            if (constant.Type != NumericType.REAL)
            {
                throw reporter.Throw("值类型错误", VMExceptionType.WRONG_TYPE);
            }
            return (RealConstant)constant;
        }

        //复制栈顶值
        public void Dup()
        {
            operandStack.Push(operandStack.Peek().GetClone());
        }

        #endregion

        #region 堆操作
        //申请整数数组
        public void NewIntArray()
        {
            operandStack.Push(heap.AllocIntArray((int)PopIntConst().IntValue));
        }

        //申请实数数组
        public void NewRealArray()
        {
            operandStack.Push(heap.AllocRealArray((int)PopIntConst().IntValue));
        }

        //存储值到指定数组的指定位置
        public void StoreValueIntoIndexOfArray()
        {
            IConstant value = operandStack.Pop();

            int index = (int)PopIntConst().Value;

            IConstant address = PopIntConst();

            if(value.Type == NumericType.INT)
            {
                heap.SetIntValue((int)address.Value, index, (int)value.Value);
            }
            else
            {
                heap.SetRealValue((int)address.Value, index, (double)value.Value);
            }
        }

        #endregion

        #region 变量表操作
        //存储变量到变量表
        public void StoreVariable(String varName)
        {
            IConstant val = operandStack.Pop();
            if(val.Type == NumericType.INT)
            {
                variableTableStack.SetIntValue(varName, (int)val.Value);
            }
            else
            {
                variableTableStack.SetRealValue(varName, (double)val.Value);
            }
        }

        //新建变量表
        public void NewScope()
        {
            variableTableStack.PushNew();
        }

        //删除变量表
        public void DeleteScope()
        {
            VariableTable variables = variableTableStack.GetTop();
            List<IConstant> constants = variables.GetAll();
            foreach(IConstant constant in constants)
            {
                if(constant.Type != NumericType.INT)
                {
                    continue;
                }
                IntConstant intConstant = (IntConstant)constant;
                if(intConstant.Usage == IntUsage.ARRAYADDR)
                {
                    heap.Free((int)intConstant.Value);
                }
            }
            variableTableStack.DropTop();
            
        }

        //声明整数变量
        public void DeclareIntVar(String varName)
        {
            variableTableStack.DeclareIntVar(varName);
        }

        //声明实数变量
        public void DeclareRealVar(String varName)
        {
            variableTableStack.DeclareRealVar(varName);
        }

        //弃用
        ////声明堆地址
        //public void declareArrayVar(String varName)
        //{
        //    variableTableStack.DeclareIntVar(varName, IntUsage.ARRAYADDR);
        //}

        //声明整型数组
        public void DeclareIntArray(String varName)
        {
            variableTableStack.DeclareIntVar(varName, IntUsage.ARRAYADDR);
            IConstant size = PopOperand();
            if(size.Type == NumericType.REAL)
            {
                //todo:数组的大小不为整数时报错
                //不确定语法分析时做没做
                return;
            }
            variableTableStack.SetIntValue(varName, (int)heap.AllocIntArray((int)size.Value).Value);
        }
        
        //声明实型数组
        public void DeclareRealArray(String varName)
        {
            variableTableStack.DeclareIntVar(varName, IntUsage.ARRAYADDR);
            IConstant size = PopOperand();
            if (size.Type == NumericType.REAL)
            {
                //todo:数组的大小不为整数时报错
                //不确定语法分析时做没做
                return;
            }
            variableTableStack.SetIntValue(varName, (int)heap.AllocRealArray((int)size.Value).Value);
        }

        //声明函数
        public void DeclareFunction(String funName, int startIndex)
        {
            variableTableStack.DeclareFun(funName, startIndex);
        }

        //获取函数地址
        public int GetFunction(String funName)
        {
            IConstant constant = variableTableStack.GetValue(funName);
            if (constant.Type == NumericType.INT && ((IntConstant)constant).Usage == IntUsage.INSADDR)
            {
                return (int)constant.Value;
            }
            throw new Exception("cmm_vm::VMCore::getFunction: 变量名不指向函数");
        }



        #endregion

    }
}
