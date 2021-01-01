using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace cmm_vm.error
{
    class VMException: Exception
    {
        public int LineNum { get; }

        public VMExceptionType Type { get; }

        public VMException(int lineNum, String msg, VMExceptionType type):base(msg)
        {
            LineNum = lineNum;
            Type = type;
        }

        override public String ToString()
        {
            return Type.ToString()+":\t"+Message+"\t@"+LineNum.ToString();
        }
    }

    enum VMExceptionType
    {
        WRONG_INDEX,
        WRONG_TYPE,
        UNDEFINED_VAR,
        REPEAT_DEFINE,
    }
}
