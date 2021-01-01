﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CMM_Complier.Grammar
{
    
    public class Node
    {
        public TokenType Type { get; set; }
        public String Name { get; set; }
        public List<Node> Next { get; set; }
        public int Line { get; set; }

        public Node() { 
        
        }
        public Node(TokenType type,String name)
        {
            this.Type = type;
            this.Name = name;
        }
        public Node(TokenType type)
        {
            this.Type = type;
        }
    }
}
