using System;
using System.Collections.Generic;
using System.Text;

namespace cmm_vm.error
{
    class ExceptionReporter
    {
        readonly VirtualMachine machine;

        public ExceptionReporter(VirtualMachine vm)
        {
            machine = vm;
        }

        public VMException Throw(String msg, VMExceptionType type)
        {
            return new VMException(machine.GetCurrentInsLine(), msg, type);
        }
    }
}
