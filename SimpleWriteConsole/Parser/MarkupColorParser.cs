using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SimpleWriteConsole.Parser
{
    public class MarkupNode
    {
        public string Tag { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<MarkupNode> Children { get; set; } = new List<MarkupNode>();
        public MarkupNode? Parent { get; set; }

        public bool IsTextNode => string.IsNullOrEmpty(Tag);
        public bool IsColorTag => !IsTextNode && MarkupColorParser.Colors.ContainsKey(Tag);
    }

    public static class MarkupColorParser
    {
        public static readonly Dictionary<string, ConsoleColor> Colors = new Dictionary<string, ConsoleColor>()
        {
            ["red"] = ConsoleColor.Red,
            ["green"] = ConsoleColor.Green,
            ["blue"] = ConsoleColor.Blue,
            ["yellow"] = ConsoleColor.Yellow,
            ["cyan"] = ConsoleColor.Cyan,
            ["white"] = ConsoleColor.White,
            ["gray"] = ConsoleColor.Gray,
            ["magenta"] = ConsoleColor.Magenta,
            ["black"] = ConsoleColor.Black,
            ["darkred"] = ConsoleColor.DarkRed,
            ["darkgreen"] = ConsoleColor.DarkGreen,
            ["darkblue"] = ConsoleColor.DarkBlue,
            ["darkyellow"] = ConsoleColor.DarkYellow,
            ["darkcyan"] = ConsoleColor.DarkCyan,
            ["darkgray"] = ConsoleColor.DarkGray,
            ["darkmagenta"] = ConsoleColor.DarkMagenta
        };
        private static readonly Regex TagRegex = new Regex(@"\[(\/?)([a-z]+)\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static MarkupNode Parse(string text)
        {
            var root = new MarkupNode();
            var stack = new Stack<MarkupNode>();
            stack.Push(root);

            int lastIndex = 0;

            foreach (Match match in TagRegex.Matches(text))
            {
                var textContent = SetValueEscape(text.Substring(lastIndex, match.Index - lastIndex));
                if (!string.IsNullOrEmpty(textContent))
                {
                    var textNode = new MarkupNode { Content = textContent, Parent = stack.Peek() };
                    stack.Peek().Children.Add(textNode);
                }

                lastIndex = match.Index + match.Length;
                var isClosing = match.Groups[1].Value == "/";
                var tag = match.Groups[2].Value.ToLower();

                if (isClosing)
                {
                    if (stack.Count > 1)
                        stack.Pop();
                }
                else if (Colors.ContainsKey(tag))
                {
                    var colorNode = new MarkupNode { Tag = tag, Parent = stack.Peek() };
                    stack.Peek().Children.Add(colorNode);
                    stack.Push(colorNode);
                }
            }

            var remaining = SetValueEscape(text.Substring(lastIndex));
            if (!string.IsNullOrEmpty(remaining))
            {
                var textNode = new MarkupNode { Content = remaining, Parent = stack.Peek() };
                stack.Peek().Children.Add(textNode);
            }

            return root;
        }

        public static string SetValueEscape(string text)
        {
            return text.Replace(@"\[\", "[").Replace(@"\]\", "]");
        }
    }

}
