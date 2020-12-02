using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WordAnalyzer
{
    class Analyzer
    {
        
        public HashSet<String> wordSet;
        public List<String> list;
        List<int> sideList;
        List<int> num;
        List<int> letters;
        List<int> followNum;
        StreamReader sr;
        void initializeList(String URL) {
            followNum = new List<int>();
            followNum.Add(41);
            followNum.Add(32);
            followNum.Add(93);
            followNum.Add(61);
            followNum.Add(47);
            followNum.Add(43);
            followNum.Add(45);
            followNum.Add(42);
            followNum.Add(59);
            followNum.Add(60);
            followNum.Add(62);
            followNum.Add(9);
            followNum.Add(10);
            followNum.Add(13);
            followNum.Add(-1);
            sideList = new List<int>();
            sideList.Add(40); // (
            sideList.Add(41); //  )
            sideList.Add(32);  // 空格
            sideList.Add(123);  //  {
            sideList.Add(125);   //  }
            sideList.Add(91);    // [
            sideList.Add(93);    // ]
            sideList.Add(61);     //  =
            sideList.Add(47);     // / 
            sideList.Add(43);    //  +
            sideList.Add(45);    //  -
            sideList.Add(42);    // *
            sideList.Add(59);    //  ;
            sideList.Add(60);     // <
            sideList.Add(62);     // >
            sideList.Add(9);     //制表
            sideList.Add(10);    //换行
            sideList.Add(13);    //回车
            sideList.Add(-1);    //结束
            num = new List<int>();
            for (int i = 48; i <= 57; i++)
            {
                num.Add(i);
            }
            letters = new List<int>();
            for (int i = 97; i <= 122; i++)
            {
                letters.Add(i);
            }
            for (int i = 65; i <= 90; i++)
            {
                letters.Add(i);
            }
            wordSet = new HashSet<string>();
            list = new List<string>();
            try
            {
                sr = new StreamReader(URL);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Initialization failed\n");
                Console.WriteLine(e);
            }
        }


        public Analyzer(String URL)
        {
            initializeList(URL);  
            int current = sr.Read(), next;
            List<char> buffer = new List<char>();
            while ((next = sr.Read()) != -1 || current!=-1) {
                if (sideList.Contains(current))
                {
                    if (current == 47)    //  /和注释
                    {
                        if (next == 42)
                        {  //处理注解
                            wordSet.Add("/*");
                            current = next;
                            while ((next = sr.Read()) != -1)
                            {
                                if ((next == 47) && (current == 42))
                                {
                                    wordSet.Add("*/");
                                    break;
                                }
                                current = next;
                            }
                            if (next == -1)
                            {
                                throw new Exception("do not find */ to match /*");
                            }
                            current = sr.Read();
                            continue;
                        }
                        else
                        {
                            list.Add("/");
                            wordSet.Add("/");
                            current = next;
                            continue;
                        }
                    }
                    else if (current == 61)  //  =和==
                    {
                        if (next == 61)
                        {
                            list.Add("==");
                            wordSet.Add("==");
                            current = sr.Read();
                            continue;
                        }
                        else
                        {
                            list.Add("=");
                            wordSet.Add("=");
                            current = next;
                            continue;
                        }
                    }
                    else if (current == 60)    // <和<>
                    {
                        if (next == 62)
                        {
                            list.Add("<>");
                            wordSet.Add("<>");
                            current = sr.Read();
                            continue;
                        }
                        else
                        {
                            list.Add("<");
                            wordSet.Add("<");
                            current = next;
                            continue;
                        }
                    }
                    else if (current == 9 || current == 10 || current == 13 || current==32) {
                        current = next;
                    }
                    else
                    {
                        list.Add(((char)current).ToString());
                        wordSet.Add(((char)current).ToString());
                        current = next;
                    }
                }
                else if (num.Contains(current))
                {
                    buffer.Add((char)current);
                    current = next;
                    if (!num.Contains(current))
                    {
                        list.Add(new String(buffer.ToArray()));
                        wordSet.Add(new String(buffer.ToArray()));
                        buffer.Clear();
                    }
                    else
                    {
                        while ((next = sr.Read()) != -1)
                        {
                            if (num.Contains(next))
                            {
                                buffer.Add((char)current);
                                current = next;
                                continue;
                            }
                            else if (followNum.Contains(next))
                            {
                                buffer.Add((char)current);
                                list.Add(new String(buffer.ToArray()));
                                wordSet.Add(new String(buffer.ToArray()));
                                buffer.Clear();
                                current = next;
                                break;
                            }
                            else
                            {
                                buffer.Add((char)current);
                                current = next;
                                while ((next = sr.Read()) != -1)
                                {
                                    if (current == 9 || current == 10 || current == 32 || current == 13)
                                    {
                                        current = next;
                                        break;
                                    }
                                    else
                                    {
                                        buffer.Add((char)current);
                                        current = next;
                                    }
                                }
                                throw new Exception("illegal symbol " + buffer.ToArray());
                                buffer.Clear();
                            }

                        }
                    }

                }
                else if (letters.Contains(current))
                {
                    buffer.Add((char)current);
                    current = next;
                    if (sideList.Contains(current))
                    {
                        list.Add(new String(buffer.ToArray()));
                        wordSet.Add(new String(buffer.ToArray()));
                        buffer.Clear();
                    }
                    else if (letters.Contains(next) || num.Contains(next) || next == 95)
                    {
                        while ((next = sr.Read()) != -1)
                        {
                            if (sideList.Contains(next))
                            {
                                buffer.Add((char)current);
                                list.Add(new String(buffer.ToArray()));
                                wordSet.Add(new String(buffer.ToArray()));
                                if (current == 95)
                                {
                                    throw new Exception("illegal symbol " + buffer.ToArray());
                                }
                                buffer.Clear();
                                current = next;
                                break;
                            }
                            else if (letters.Contains(next) || num.Contains(next) || next == 95)
                            {
                                buffer.Add((char)current);
                                current = next;
                            }
                            else
                            {
                                buffer.Add((char)current);
                                current = next;
                                while ((next = sr.Read()) != -1)
                                {
                                    if (current == 9 || current == 10 || current == 32 || current == 13)
                                    {
                                        current = next;
                                        break;
                                    }
                                    else
                                    {
                                        buffer.Add((char)current);
                                        current = next;
                                    }
                                }
                                throw new Exception("illegal symbol " + buffer.ToArray());
                                buffer.Clear();
                            }
                        }
                    }
                    else
                    {
                        buffer.Add((char)current);
                        current = next;
                        while ((next = sr.Read()) != -1)
                        {
                            if (current == 9 || current == 10 || current == 32 || current == 13)
                            {
                                current = next;
                                break;
                            }
                            else
                            {
                                buffer.Add((char)current);
                                current = next;
                            }
                        }
                        throw new Exception("illegal symbol " + buffer.ToArray());
                        buffer.Clear();
                    }
                }
                else {
                    buffer.Add((char)current);
                    current = next;
                    while ((next = sr.Read()) != -1)
                    {
                        if (current == 9 || current == 10 || current == 32 || current == 13)
                        {
                            current = next;
                            break;
                        }
                        else
                        {
                            buffer.Add((char)current);
                            current = next;
                        }
                    }
                    throw new Exception("illegal symbol " + buffer.ToArray());
                    buffer.Clear();

                }
              

            }
            sr.Close();
           
        }
    }
}