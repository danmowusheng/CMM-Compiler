using System;
using System.Collections.Generic;
using System.IO;

namespace SyntaxAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            /*StreamReader sr = new StreamReader(@"C:\Users\tp103\Desktop\aaa.list");
            String s;
            while ((s = sr.ReadLine()) != null) {
                Console.WriteLine(s.Split(" ").Length);
            }*/
            Analyzer a = new Analyzer(@"C:\Users\tp103\Desktop\aaa.list");
            Node s=a.Analyze();
            //getPostExpr(s.Next[4].Next[1].Next[1].Next[2]);
            
           /* ProcessCaculate(s.Next[4].Next[1].Next[1]);
            foreach (String x in ins)
            {
                Console.WriteLine(x);
            }*/
            
        }

        public static List<String> ins=new List<String>();


        static void ProcessCaculate(Node s) {
            bool f = false;
            getPostExpr(s.Next[0]);
            string Target = res[0];
            if (res.Count != 1)
            {
                foreach (String str in res)
                {
                    if (str.Equals("+"))
                    {
                        ins.Add("OP_ADD");
                    }
                    else if (str.Equals("-"))
                    {
                        ins.Add("OP_SUB");
                    }
                    else if (str.Equals("*"))
                    {
                        ins.Add("OP_MUL");
                    }
                    else if (str.Equals("/"))
                    {
                        ins.Add("OP_DIV");
                    }
                    else if (str.Equals("OP_NEG"))
                    {
                        ins.Add(str);
                    }
                    else if (str.Equals("-"))
                    {
                        ins.Add("OP_SUB");
                    }
                    else if (str.Equals("locArray"))
                    {
                        f = true;
                    }
                    else
                    {
                        try
                        {
                            ins.Add("LD_INT " + int.Parse(str));
                        }
                        catch (Exception)
                        {
                            ins.Add("LD_VAR " + str);
                        }
                    }
                }
            }
            getPostExpr(s.Next[2]);
            foreach (String str in res) {
                if (str.Equals("+")) {
                    ins.Add("OP_ADD");
                } else if (str.Equals("-")) {
                    ins.Add("OP_SUB");
                }
                else if (str.Equals("*"))
                {
                    ins.Add("OP_MUL");
                }
                else if (str.Equals("/"))
                {
                    ins.Add("OP_DIV");
                }
                else if (str.Equals("OP_NEG"))
                {
                    ins.Add(str);
                }
                else if (str.Equals("-"))
                {
                    ins.Add("OP_SUB");
                } else if (str.Equals("locArray")) {
                    ins.Add("LD_ELE_INT");
                } else {
                    try {
                        ins.Add("LD_INT "+int.Parse(str));
                    }catch(Exception) {
                        ins.Add("LD_VAR "+str);
                    }
                }
            }
            if (f)
            {
                ins.Add("ST_ADR");
            }
            else {
                ins.Add("ST_VAR "+Target);
            }
        }

        static void getPostExpr(Node s) {
            res.Clear();
            tmp.Clear();
            flag = true;
            GeneratePostExpr(s);
            while (tmp.Count != 0) {
                res.Add(tmp.Pop());
            }
        }

        public static List<String> res = new List<string>();
        public static Stack<String> tmp = new Stack<string>();
        public static bool flag;
        static void GeneratePostExpr(Node s) {
            if (s.Next == null)
            {
                if (s.Type == TokenType.terminal)
                {
                    if (s.Name.Equals("("))
                    {
                        tmp.Push("(");
                    }
                    else if (s.Name.Equals(")"))
                    {
                        while (tmp.Count != 0 && !tmp.Peek().Equals("("))
                        {
                            res.Add(tmp.Pop());
                        }
                        tmp.Pop();
                    }
                    else if (s.Name.Equals("+"))
                    {
                        while (tmp.Count != 0)
                        {
                            if (tmp.Peek().Equals("(")|| tmp.Peek().Equals("["))
                            {
                                break;
                            }
                            res.Add(tmp.Pop());
                        }
                        tmp.Push("+");
                    }
                    else if (s.Name.Equals("-"))
                    {
                        while (tmp.Count != 0)
                        {
                            if (tmp.Peek().Equals("(")|| tmp.Peek().Equals("["))
                            {
                                break;
                            }
                            res.Add(tmp.Pop());
                        }
                        if (flag)
                        {
                            tmp.Push("-");
                        }
                        else {
                            tmp.Push("OP_NEG");
                            flag = true;
                        }
                        
                    }
                    else if (s.Name.Equals("*"))
                    {
                        while (tmp.Count != 0)
                        {
                            if (tmp.Peek().Equals("+") || tmp.Peek().Equals("-") || tmp.Peek().Equals("(") || tmp.Peek().Equals("["))
                            {
                                break;
                            }
                            else
                            {
                                res.Add(tmp.Pop());
                            }
                        }
                        tmp.Push("*");
                    }
                    else if (s.Name.Equals("/"))
                    {
                        while (tmp.Count != 0)
                        {
                            if (tmp.Peek().Equals("+") || tmp.Peek().Equals("-") || tmp.Peek().Equals("(") || tmp.Peek().Equals("["))
                            {
                                break;
                            }
                            else
                            {
                                res.Add(tmp.Pop());
                            }
                        }
                        tmp.Push("/");
                    }
                    else if (s.Name.Equals("["))
                    {
                        tmp.Push("[");
                    }
                    else if (s.Name.Equals("]")) {
                        while (tmp.Count != 0 && !tmp.Peek().Equals("["))
                        {
                            res.Add(tmp.Pop());
                        }
                        res.Add("locArray");
                        tmp.Pop();
                    }
                    else
                    {
                        res.Add(s.Name);
                    }
                }
             return;
            }
            else {
                if (s.Type == TokenType.unary) {
                    if (s.Next.Count == 2) {
                        flag = false;
                    }
                
                }
               
                foreach (Node q in s.Next) {
                    GeneratePostExpr(q);
                }

            }
        }
    }
}
