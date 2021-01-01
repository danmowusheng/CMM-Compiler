using System;
using System.Collections.Generic;
using System.IO;
namespace WordAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
             Analyzer a = new Analyzer(@"C:\Users\tp103\Desktop\aaa.cmm");
            /* a.ListToFile();
             a.WordSetToFile();*/
            List<Token> b = a.toTokenList();
            a.ListToFile();
            foreach (Token s in b) {
                Console.WriteLine(s.LineNum+" "+s.Type+" "+s.Value);
            
            }

            


        }
    }
}
