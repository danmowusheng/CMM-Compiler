using cmm_vm.cons;
using cmm_vm.error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmm_vm.var
{
    class VariableTableStack
    {
        private readonly Stack<VariableTable> tableStack = new Stack<VariableTable>();

        private readonly VariableTable globalTable;

        private ExceptionReporter reporter;

        public VariableTableStack(ExceptionReporter reporter)
        {
            this.reporter = reporter;
            globalTable = new VariableTable(reporter);
            tableStack.Push(globalTable);
        }

        public VariableTable GetTop()
        {
            return tableStack.Peek();
        }

        public Stack<VariableTable> GetTableStack()
        {
            return tableStack;
        }

        public void PushNew()
        {
            tableStack.Push(new VariableTable(reporter));
        }

        public void DropTop()
        {
            tableStack.Pop();
        }

        public void Flush()
        {
            globalTable.Flush();
            tableStack.Clear();
            tableStack.Push(globalTable);
        }

        #region 对具体变量表的操作
        public IConstant GetValueShallow(String varName)
        {
            if(tableStack.Peek().GetValue(varName, out IConstant value))
            {
                return value;
            }
            //自己做了错误处理之后,不需要抛出异常
            //throw new Exception("cmm_vm.var::VariableTableStack::GetValueShallow: 尝试获取未定义的变量");
            return null;
        }

        //依据嵌套逐层查找变量
        public IConstant GetValue(String varName)
        {
            foreach (VariableTable table in tableStack.ToArray())
            {
                //找到最近层定义的变量后立即退出
                if (table.GetValue(varName, out IConstant value))
                {
                    return value;
                }
            }
            throw reporter.Throw("不能使用未定义的变量", VMExceptionType.UNDEFINED_VAR);
        }

        public IConstant GetValueNoErr(String varName)
        {
            foreach (VariableTable table in tableStack.ToArray())
            {
                //找到最近层定义的变量后立即退出
                //这里使用不会产生错误信息的方法
                if (table.GetValue(varName, out IConstant value))
                {
                    return value;
                }
            }
            return null;
        }

        public void DeclareIntVar(String varName)
        {
            if (tableStack.Peek().DeclareIntVar(varName))
                return;
            //throw new Exception("cmm_vm.var::VariableTableStack::DeclareIntVar: 重复定义整型变量:"+varName);
        }

        public void DeclareIntVar(String varName, IntUsage usage)
        {
            if (tableStack.Peek().DeclareIntVar(varName, usage))
                return;
            //throw new Exception("cmm_vm.var::VariableTableStack::DeclareIntVar: 重复定义整型变量:" + varName);
        }

        public void DeclareRealVar(String varName)
        {
            if (tableStack.Peek().DeclareRealVar(varName))
                return;
            //throw new Exception("cmm_vm.var::VariableTableStack::DeclareRealVar: 重复定义实型变量:" + varName);
        }

        public void SetIntValue(String varName, int value)
        {
            if (tableStack.Peek().SetIntValue(varName, value))
                return;
            //throw new Exception("cmm_vm.var::VariableTableStack::SetIntValue: 使用未定义的整型变量:" + varName);
        }

        public void SetRealValue(String varName, double value)
        {
            if (tableStack.Peek().SetRealValue(varName, value))
                return;
            //throw new Exception("cmm_vm.var::VariableTableStack::SetRealValue: 使用未定义的实型变量:" + varName);
        }

        public void DeleteVar(String varName)
        {
            if (tableStack.Peek().DeleteVar(varName))
                return;
            //这个在DeleteVar方法里面没做错误处理,抛出一个异常
            throw new Exception("cmm_vm.var::VariableTableStack::DeleteVar: 尝试删除未定义的实型变量:" + varName);
        }

        public void DeclareFun(String name, int index)
        {
            if (tableStack.Peek().DeclareFunction(name, index))
                return;
            //throw new Exception("cmm_vm.var::VariableTableStack::DeclareFun: 重复定义函数:" + name);
        }

        #endregion
    }
}
