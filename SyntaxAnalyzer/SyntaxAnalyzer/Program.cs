using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SyntaxAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            /*StreamReader sr = new StreamReader(@"C:\Users\tp103\Desktop\2.list");
            String s;
            while ((s = sr.ReadLine()) != null) {
                Console.WriteLine(s.Split(" ").Length);
            }*/
            Analyzer a = new Analyzer(@"C:\Users\tp103\Desktop\2.list");
            Node s=a.Analyze();
            get(s);
            
        }

        static void get(Node s) {
            if (s.Next == null)
            {
                if (s.Type == TokenType.terminal)
                {
                    Console.WriteLine(s.Name);
                }
            }
            else {
                foreach (Node n in s.Next) {
                    get(n);
                }
            
            }
            
        }
    }
}
