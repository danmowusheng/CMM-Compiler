using System;

namespace cmm_vm.cons
{
    /// <summary>
    /// 实型常数
    /// </summary>
    class RealConstant : IConstant
    {
        private double value;

        public NumericType Type => NumericType.REAL;

        public int? IntValue { get => null; set => throw new Exception("cmm_vm.cons::RealConstant::IntValue: RealConstant类型不能安排整数数值"); }
        
        public double? RealValue
        {
            get => value;
            set
            {
                if (value == null)
                {
                    throw new Exception("cmm_vm.cons::RealConstant::RealValue: 不能将空值分配给RealConstant");
                }
            }
        }

        public ValueType Value
        {
            get => value;
            set
            {
                if (value.GetType().ToString().Equals("System.Double"))
                {
                    this.value = (double)value;
                }
                else if (value.GetType().ToString().Equals("System.Int32"))
                {
                    this.value = Double.Parse(value.ToString());
                }
                else
                {
                    throw new Exception("cmm_vm.cons::RealConstant::Value: RealConstant类型只能安排Double类型的值");
                }
            }
        }

        public RealConstant()
        {
            value = 0;
        }

        public RealConstant(int val)
        {
            value = (double)val;
        }

        public RealConstant(double val)
        {
            value = val;
        }

        public IConstant GetClone()
        {
            return new RealConstant((double)RealValue);
        }

        public static bool operator ==(RealConstant a, RealConstant b)
        {
            return a.Value.Equals(b.Value);
        }

        public static bool operator !=(RealConstant a, RealConstant b)
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
}
