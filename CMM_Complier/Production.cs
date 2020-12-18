using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GrammerParser
{
    class Production
    {
        public string left;
        public int rightNum;

        public Production(string left, int rightNum)
        {
            this.left = left;
            this.rightNum = rightNum;
        }
    
    }
}
