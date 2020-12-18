using System;
using System.Collections.Generic;
using System.IO;
namespace WordAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
             Analyzer a = new Analyzer(@"test.txt");
            /* a.ListToFile();
             a.WordSetToFile();*/
            List<Token> b = a.toTokenList();
            foreach (Token s in b) {
                Console.WriteLine(s.LineNum+" "+s.Type+" "+s.Value);
            
            }

            Token endToken = new Token();
            endToken.Type = TokenType.END;
            endToken.LineNum = b.FindLast((Token t) => t.LineNum >= 0).LineNum;
            b.Add(endToken);
            foreach (Token s in b)
            {
                Console.WriteLine(s.LineNum + " " + s.Type + " " + s.Value);

            }

            Parser parser = new Parser();
            //获取词法分析产生的token序列
            Parser.tokens = b;
            //读入分析表
            Parser.getAnalysisTable(@"parsing_table_v1.1.csv");
            //读入产生式
            Parser.getProduction(@"production.txt");



            bool fig = Parser.Parse();
            Console.WriteLine(fig);

        }
    }
}
