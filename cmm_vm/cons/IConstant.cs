using System;

namespace cmm_vm.cons
{
    /// <summary>
    /// 常数类型接口
    /// </summary>
    interface IConstant
    {
        NumericType Type { get; }

        //使用确定类型的方式访问值（错误访问会产生未处理的异常）
        int? IntValue { get; set; }
        double? RealValue { get; set; }

        //使用不确定类型的方式访问值
        ValueType Value { get; set; }

        //复制
        IConstant GetClone();
    }

    enum NumericType
    {
        INT,    //int32
        REAL,   //double
    }
}
