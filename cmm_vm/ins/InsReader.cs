using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace cmm_vm.ins
{
    class InsReader
    {
        public static Instruction[] ReadFromFile(String fileName)
        {
            List<Instruction> instructions = new List<Instruction>();
            StreamReader sr = new StreamReader(fileName, Encoding.Default);
            String line;
            while((line = sr.ReadLine()) != null)
            {
                instructions.Add(ParseIns(line));
            }
            return instructions.ToArray();
        }

        public static Instruction ParseIns(String s)
        {
            String[] strings = s.Replace('\t', ' ').Split(" ");

            int lineNum = Convert.ToInt32(strings[0]);
            String type = strings[1];
            String arg = strings[2];

            return new Instruction((InstructionType)Enum.Parse(typeof(InstructionType), type), arg, lineNum);
        }
    }
}
