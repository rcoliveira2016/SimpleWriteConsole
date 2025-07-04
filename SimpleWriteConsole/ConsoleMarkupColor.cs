using SimpleWriteConsole.Parser;
using System;

namespace SimpleWriteConsole
{
    public static class ConsoleMarkupColor
    {
        public static void Write(string text)
        {
            var tree = MarkupColorParser.Parse(text);
            RenderTree(tree, writeLine: false);
        }

        public static void WriteLine(string text)
        {
            var tree = MarkupColorParser.Parse(text);
            RenderTree(tree, writeLine: true);
        }

        private static void RenderTree(MarkupNode node, bool writeLine = false)
        {
            var defaultColor = Console.ForegroundColor;
            RenderNode(node, defaultColor);

            if (writeLine)
                Console.WriteLine();

            Console.ForegroundColor = defaultColor;
        }

        private static void RenderNode(MarkupNode node, ConsoleColor defaultColor)
        {
            if (node.IsTextNode)
            {
                Console.Write(node.Content);
            }

            var previousColor = Console.ForegroundColor;

            if (node.IsColorTag)
            {
                Console.ForegroundColor = MarkupColorParser.Colors[node.Tag];
            }

            foreach (var child in node.Children)
            {
                RenderNode(child, defaultColor);
            }

            if (node.IsColorTag)
            {
                Console.ForegroundColor = previousColor;
            }
        }
    }
}
