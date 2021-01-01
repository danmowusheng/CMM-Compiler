using System;
using System.Collections.Generic;
using System.Text;

namespace CMM_Complier.Grammar
{
    public class JsonNode
    {
        public string name { get; set; }
        public List<JsonNode> children { get; set; }

        //更改结点格式
        public static JsonNode changeNodeFormat(Node node)
        {
            JsonNode jsonNode = new JsonNode();
            if (node.Type == TokenType.terminal)
            {
                jsonNode.name = node.Name;
            }
            else
            {
                jsonNode.name = node.Type.ToString();
            }

            if (node.Next == null)
            {
                jsonNode.children = null;
            }
            else
            {
                jsonNode.children = new List<JsonNode>();
                foreach (Node n in node.Next)
                {
                    jsonNode.children.Add(changeNodeFormat(n));
                }
            }

            return jsonNode;
        }

    }
}
