using System;

namespace cmm_vm.cons
{
    /// <summary>
    /// 整型常数
    /// </summary>
    class IntConstant : IConstant
    {
        private int value;

        public NumericType Type => NumericType.INT;

        public IntUsage Usage
        {
            get;
            set;
        } = IntUsage.DEFAULT;

        public int? IntValue 
        { 
            get => value;
            set
            {
                if (value == null)
                {
                    throw new Exception("cmm_vm.cons::IntConstant::IntValue: 不能将空值分配给IntConstant");
                }
            }
        }
        public double? RealValue { get => null; set => throw new Exception("cmm_vm.cons::IntConstant::RealValue: IntConstant类型不能安排浮点数值"); }
        public ValueType Value 
        { 
            get => value;
            set
            {
                if (!value.GetType().ToString().Equals("System.Int32"))
                {
                    throw new Exception("cmm_vm.cons::IntConstant::Value: IntConstant类型只能安排Int32类型的值");
                }
                this.value = (int)value;
            }
        }

        public IntConstant()
        {
            value = 0;
        }

        public IntConstant(int val)
        {
            value = val;
        }

        public IntConstant(int val, IntUsage usage)
        {
            value = val;
            Usage = usage;
        }

        public IConstant GetClone()
        {
            return new IntConstant((int)IntValue);
        }

        public static bool operator ==(IntConstant a, IntConstant b)
        {
            return a.Value.Equals(b.Value);
        }

        public static bool operator !=(IntConstant a, IntConstant b)
        {
            return a.Value.Equals(b.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    enum IntUsage
    {
        DEFAULT,
        INSADDR,
        ARRAYADDR,
        STACK_HEAD,
    }
}
