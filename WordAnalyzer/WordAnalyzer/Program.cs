using System;
using System.Collections.Generic;
using System.IO;

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
             a.ListToFile();
             a.WordSetToFile();
        }
    }
}
