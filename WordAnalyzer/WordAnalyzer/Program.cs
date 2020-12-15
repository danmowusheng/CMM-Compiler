using System;
using System.Collections.Generic;
using System.IO;

namespace WordAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
             Analyzer a = new Analyzer(@"C:\Users\tp103\Desktop\2.txt");
            /* a.ListToFile();
             a.WordSetToFile();*/
            List<Token> b = a.toTokenList();
            foreach (Token s in b) {
                Console.WriteLine(s.LineNum+" "+s.Type+" "+s.Value);
            
            }
        }
    }
}
