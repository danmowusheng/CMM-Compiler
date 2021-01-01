using cmm_vm.cons;
using cmm_vm.error;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmm_vm.var
{
    class VariableTable
    {
        private readonly Dictionary<String, IConstant> varDict = new Dictionary<String, IConstant>();

        private ExceptionReporter reporter;

        public VariableTable(ExceptionReporter reporter)
        {
            this.reporter = reporter;
        }

        public Dictionary<String, ValueType> GetVars()
        {
            Dictionary<String, ValueType> vars = new Dictionary<string, ValueType>();
            foreach(String varName in varDict.Keys)
            {
                varDict.TryGetValue(varName, out IConstant constant);
                if(constant.Type == NumericType.INT)
                {
                    vars.Add(varName, (int)(constant.Value));
                }
                else
                {
                    vars.Add(varName, (double)(constant.Value));
                }
            }
            return vars;
        }

        public bool GetValue(String varName, out IConstant value)
        {
            if (varDict.ContainsKey(varName))
            {
                value = varDict[varName];
                return true;
            }
            value = null;
            return false;
        }

        public bool IsDeclared(String varName)
        {
            return varDict.ContainsKey(varName);
        }

        public bool DeclareIntVar(String varName)
        {
            if (varDict.ContainsKey(varName))
            {
                throw reporter.Throw("重复定义变量或函数名", VMExceptionType.REPEAT_DEFINE);
            }
            varDict.Add(varName, new IntConstant(0));
            return true;
        }

        public bool DeclareIntVar(String varName, IntUsage usage)
        {
            if (varDict.ContainsKey(varName))
            {
                throw reporter.Throw("重复定义变量或函数名", VMExceptionType.REPEAT_DEFINE);
            }
            varDict.Add(varName, new IntConstant(0, usage));
            return true;
        }

        public bool DeclareRealVar(String varName)
        {
            if (varDict.ContainsKey(varName))
            {
                throw reporter.Throw("重复定义变量或函数名", VMExceptionType.REPEAT_DEFINE);
            }
            varDict.Add(varName, new RealConstant(0));
            return true;
        }

        public bool DeclareFunction(String name, int index)
        {
            if (varDict.ContainsKey(name))
            {
                throw reporter.Throw("重复定义变量或函数名", VMExceptionType.REPEAT_DEFINE);
            }
            varDict.Add(name, new IntConstant(index, IntUsage.INSADDR));
            return true;
        }

        public bool SetIntValue(String varName, int intValue)
        {
            if (!varDict.ContainsKey(varName))
            {
                throw reporter.Throw("不能使用未定义的变量", VMExceptionType.UNDEFINED_VAR);
            }
            IConstant var = varDict[varName];
            if (var.Type != NumericType.INT)
            {
                return false;
            }
            var.Value = intValue;
            return true;
        }

        public bool SetRealValue(String varName, double realValue)
        {
            if (!varDict.ContainsKey(varName))
            {
                throw reporter.Throw("不能使用未定义的变量", VMExceptionType.UNDEFINED_VAR);
            }
            IConstant var = varDict[varName];
            if(var.Type != NumericType.REAL)
            {
                return false;
            }
            var.Value = realValue;
            return true;
        }

        //应该用不到
        public bool DeleteVar(String varName)
        {
            if (varDict.ContainsKey(varName))
            {
                varDict.Remove(varName);
                return true;
            }
            return false;
        }

        public List<IConstant> GetAll()
        {
            return varDict.Values.ToList();
        }

        public void Flush()
        {
            varDict.Clear();
        }
    }
}
