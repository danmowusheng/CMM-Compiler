using System;
using System.Collections.Generic;
using System.Text;

namespace GrammerParser.Word
{
    enum TokenType { IF, ELSE, WHILE, READ, WRITE, BASIC, SYMBOL, INTEGER, REAL, IDENTIFIER, FLOAT, END}
    class Token
    {
        private TokenType type;
        private object value;
        private int lineNum;

        public object Value { get => value; set => this.value = value; }
        public int LineNum { get => lineNum; set => lineNum = value; }
        internal TokenType Type { get => type; set => type = value; }
    }
}