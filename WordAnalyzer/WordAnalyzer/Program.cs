using System;
using System.Collections.Generic;

namespace WordAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            Analyzer a = new Analyzer("1.txt");
           foreach (String b in a.list) {
                Console.WriteLine(b);
                
            }
        }
    }
}
