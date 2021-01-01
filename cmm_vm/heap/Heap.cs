using cmm_vm.cons;
using cmm_vm.error;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace cmm_vm.heap
{
    class Heap
    {
        private Dictionary<Int32, Array> heapMap = new Dictionary<int, Array>();

        private ExceptionReporter reporter;

        //数组最大长度
        private static readonly int MAXSIZE = Int32.MaxValue;

        public Heap(ExceptionReporter reporter)
        {
            this.reporter = reporter;
        }

        //获取随机堆地址,暂时不考虑堆满的情况
        private int GetRandomAddress()
        {
            int n;
            Random random = new Random();
            while (heapMap.ContainsKey(n = random.Next(Int32.MinValue, Int32.MaxValue))) ;
            return n;
        }

        public IntConstant AllocIntArray(int size)
        {
            if(size <= 0 || size > MAXSIZE)
            {
                throw reporter.Throw("数组下标越界", VMExceptionType.WRONG_INDEX);
            }
            Int32 address = GetRandomAddress();
            int[] intArray;
            intArray = new int[1]{ 0};
            Array.Resize(ref intArray, size);
            Array.Fill(intArray, 0);
            heapMap.Add(address, intArray);
            return new IntConstant(address, IntUsage.ARRAYADDR);
        }

        public IntConstant AllocRealArray(int size)
        {
            if (size <= 0 || size > MAXSIZE)
            {
                throw reporter.Throw("数组下标越界", VMExceptionType.WRONG_INDEX);
            }
            Int32 address = GetRandomAddress();
            double[] realArray;
            realArray = new double[1] { 0.0 };
            Array.Resize(ref realArray, size);
            Array.Fill(realArray, 0.0);
            heapMap.Add(address, realArray);
            return new IntConstant(address, IntUsage.ARRAYADDR);
        }

        public IntConstant GetIntValue(int address, int index)
        {
            Array array = null;
            heapMap.TryGetValue(address, out array);
            if(array == null)
            {
                //出现这种情况是编译器的问题,不是用户源代码的问题
                throw new Exception("对应地址不存在数组");
            }
            if(index < 0 || index > array.Length - 1)
            {
                throw reporter.Throw("数组下标越界", VMExceptionType.WRONG_INDEX);
            }
            object val = array.GetValue(index);
            if (!val.GetType().Equals(typeof(int)))
            {
                throw reporter.Throw("访问的数组不是一个整型数组", VMExceptionType.WRONG_TYPE);
            }
            return (IntConstant)new IntConstant((int)val);
        } 

        public RealConstant GetRealValue(int address, int index)
        {
            Array array = null;
            heapMap.TryGetValue(address, out array);
            if (array == null)
            {
                //出现这种情况是编译器的问题,不是用户源代码的问题
                throw new Exception("对应地址不存在数组");
            }
            if (index < 0 || index > array.Length - 1)
            {
                throw reporter.Throw("数组下标越界", VMExceptionType.WRONG_INDEX);
            }
            object val = array.GetValue(index);
            if (!val.GetType().Equals(typeof(double)))
            {
                throw reporter.Throw("访问的数组不是一个实型数组", VMExceptionType.WRONG_TYPE);
            }
            return (RealConstant)new RealConstant((double)val);
        }

        public Array GetArray(int address)
        {
            heapMap.TryGetValue(address, out Array array);
            return array;
        }

        public Boolean SetIntValue(int address, int index, int val)
        {
            Array array = null;
            if (heapMap.TryGetValue(address, out array))
            {
                if (index < 0 || index > array.Length - 1)
                {
                    throw reporter.Throw("数组下标越界", VMExceptionType.WRONG_INDEX);
                }
                if (array.GetValue(index).GetType().Equals(typeof(double)))
                {
                    throw reporter.Throw("不能将整型元素置入实型数组", VMExceptionType.WRONG_TYPE);
                }
                array.SetValue(val, index);
                return true;
            }
            throw reporter.Throw("不能访问未声明的数组", VMExceptionType.UNDEFINED_VAR);
        }

        public Boolean SetRealValue(int address, int index, double val)
        {
            Array array = null;
            if (heapMap.TryGetValue(address, out array))
            {
                if (index < 0 || index > array.Length - 1)
                {
                    throw reporter.Throw("数组下标越界", VMExceptionType.WRONG_INDEX);
                }
                if (array.GetValue(index).GetType().Equals(typeof(int)))
                {
                    throw reporter.Throw("不能将实型元素置入整型数组", VMExceptionType.WRONG_TYPE);
                }
                array.SetValue(val, index);
                return true;
            }
            throw reporter.Throw("不能使用未声明的数组", VMExceptionType.UNDEFINED_VAR);
        }

        public Boolean Free(int address)
        {
            if (!heapMap.ContainsKey(address))
            {
                throw reporter.Throw("不能删除未声明的数组", VMExceptionType.UNDEFINED_VAR);
            }
            heapMap.Remove(address);
            return true;
        }

        public void Flush()
        {
            heapMap.Clear();
        }
    }

}
