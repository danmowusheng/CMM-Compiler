using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using GrammerParser;
using WordAnalyzer;

namespace GrammerParser
{
    class Parser
    {
        //状态栈
        public static Stack<object> StateStack = new Stack<object>();
        //符号栈
        public static Stack<object> WordStack = new Stack<object>();
        //输入Token
        public static List<Token> tokens = new List<Token>();
        //一张分析表
        //根据当前输入String和当前所处状态int， 查询当前的Action动作string
        public static Dictionary<string, Dictionary<int, string>> analysisTable = new Dictionary<string, Dictionary<int, string>>();
        //产生式表
        public static List<Production> productions = new List<Production>();

        //读取文件中的分析表
        public static void getAnalysisTable(string filepath)
        {
            int count = 1;      //用于标定是第几行
            string line;
            List<string> wordlist = new List<string>();

            int state;
            List<int> states = new List<int>();
            StreamReader sr = new StreamReader(filepath);
            while ((line = sr.ReadLine()) != null)
            {
                if (count == 1)
                {
                    //读取终结字符和非终结字符
                    for (int i = 1; i < line.Split(',').Length; i++)
                    {
                        string word = line.Split(',')[i];
                        wordlist.Add(word);
                        analysisTable.Add(word, new Dictionary<int, string>());
                    }
                }
                else
                {
                    //获取当前行的状态
                    state = Convert.ToInt32(line.Split(',')[0]);
                    states.Add(state);
                    //填充分析表
                    for (int i = 1; i < line.Split(',').Length; i++)
                    {
                        analysisTable[wordlist[i - 1]].Add(state, line.Split(',')[i]);
                    }
                }
                count++;
            }
            //Console.WriteLine("读表结束");
            /*
            foreach (int s in states)
            {
                foreach(string word in wordlist)
                {
                    Console.Write(analysisTable[word][s]+" ");
                }                
                Console.WriteLine();
            }
            */
            Console.WriteLine("分析表读取成功");
        }

        //构造产生式
        public static void getProduction(string filepath)
        {
            string line;
            int count = 1;
            Production production;
            StreamReader sr = new StreamReader(filepath);
            while ((line = sr.ReadLine()) != null)
            {
                string left;      //左部非终结符号
                int rightNum;   //右部归约项数目


                left = line.Split('→')[0];

                //右部符号含终结符号与非终结符号，并且含有空串nullable
                rightNum = line.Split('→')[1].Length;
                production = new Production(left, rightNum);
                productions.Add(production);

                count++;
            }
            /*
            foreach (Production p in productions)
            {
                Console.WriteLine(p.left + " " + p.rightNum);
            }
            */
        }

        public static string Parse()
        {
            string output = "";
            try
            {
                //用来表示程序是否正常分析完成
                bool isEnd = false;
                //用于记录上一次动作
                string lastAction = null;

                //消除原来所存储内容
                //tokens.Clear();
                StateStack = new Stack<object>();
                WordStack = new Stack<object>();
                //状态栈压入初始状态0,符号栈压入结束符号
                StateStack.Push(0);
                WordStack.Push("$");
               

                //进行语法分析
                while (tokens.Count != 0)
                {
                    //value用来获取token在分析表中的符号信息
                    string value;
                    //读取token值
                    Token current = tokens[0];
                    //根据状态栈栈顶元素和读入token查找分析表
                    int topState = (int)StateStack.Peek();
                    //用于记录上一次动作
                    
                    //获取动作
                    value = (string)current.Value;

                    //如果token的类型为int，real或者identify和end的类型此时需要根据类型查找表
                    //下面的语句将token类型映射成表所需要的key
                    switch (current.Type)
                    {
                        case TokenType.IDENTIFIER:
                            value = "id";
                            break;
                        case TokenType.INTEGER:
                            value = "num";
                            break;
                        case TokenType.REAL:
                            value = "real";
                            break;
                        case TokenType.BASIC:
                            value = "basic";
                            break;
                        case TokenType.END:
                            value = "$";
                            break;
                    }

                    if (current.Value == null&&current.Type!=TokenType.END)
                    {
                        current.Value = Enum.GetName(typeof(TokenType), current.Type).ToLower();
                        value = (string)current.Value;
                    }


                    //获取动作
                    string action = analysisTable[value][topState];

                    if (action == " ")
                    {
                        if (lastAction != null)
                        {               
                            throw new ParserException("程序出现" + lastAction + "错误,  在第" + current.LineNum + "行");
                        }                        
                        throw new ParserException("程序出现语法错误!  错误在第"+ current.LineNum+"行");
                    }
                    else if (action == "acc")
                    {
                        //该行程序已经结束
                        isEnd = true;
                        tokens.Remove(current);
                    }
                    else
                    {
                        /*
                        if (endLineNum != 0 && current.LineNum > endLineNum)
                        {
                            throw new ParserException("在程序主体结束之后还有声明和定义，出现错误!");
                        }
                        */
                        char first = action[0];
                        //num可能是状态，也可能表示是第几条产生式
                        int num = Convert.ToInt32(action.Substring(1));

                        if (first == 's')
                        {
                            output += "移进" + current.Value+"\n";
                            //Console.WriteLine("移进" + current.Value);
                            //进行移进
                            WordStack.Push(current.Value);
                            //Console.WriteLine("移进后状态为：" + num);
                            output += "移进后状态为：" + num + "\n";
                            StateStack.Push(num);
                            tokens.Remove(current);
                            lastAction = "移进";
                        }
                        else if (first == 'r')
                        {

                            //进行归约
                            Production currentP = productions[num - 1];
                            //Console.WriteLine("归约,归约产生式为：" + currentP.left + " " + currentP.rightNum);
                            output += "归约,归约产生式为：" + currentP.left + " " + currentP.rightNum+ "\n";

                            //要弹出个数
                            int reductionNum = currentP.rightNum;
                            string nonterminal = currentP.left.ToString();
                            //弹栈
                            while (reductionNum != 0)
                            {
                                WordStack.Pop();
                                StateStack.Pop();
                                reductionNum--;
                            }
                            WordStack.Push(nonterminal);
                            topState = (int)StateStack.Peek();
                            //执行GOTO
                            int nextState = Convert.ToInt32(analysisTable[nonterminal][topState]);
                            StateStack.Push(nextState);
                            //Console.WriteLine("归约后GOTO：" + nextState);
                            output += "归约后GOTO：" + nextState + "\n";

                            lastAction = "归约";
                        }
                    }

                }
                if (isEnd == true)
                {
                    //Console.WriteLine("程序通过语法检查");
                    output += "程序通过语法检查"+ "\n";

                    return output;
                }
                else
                {
                    throw new ParserException("程序语法检查异常退出, 缺少结束符号");
                }

            }
            catch (ParserException e)
            {

                output += e.err_msg+"\n";
            }
            return output;
        }
    }
}
