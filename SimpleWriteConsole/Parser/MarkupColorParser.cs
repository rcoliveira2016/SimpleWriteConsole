using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

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
        public static readonly ReadOnlyDictionary<string, ConsoleColor> Colors =
            new ReadOnlyDictionary<string, ConsoleColor>(new Dictionary<string, ConsoleColor>
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
            });

        private static readonly Regex TagRegex = new Regex(@"\[(\/?)([a-z]+)\]",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static MarkupNode Parse(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new MarkupNode();

            var root = new MarkupNode();
            var stack = new Stack<MarkupNode>();
            stack.Push(root);

            int lastIndex = 0;
            var matches = TagRegex.Matches(text);

            if (matches.Count > 0)
            {
                root.Children.Capacity = Math.Max(4, matches.Count);
            }

            foreach (Match match in matches)
            {
                if (match.Index > lastIndex)
                {
                    var textContent = UnescapeText(text.Substring(lastIndex, match.Index - lastIndex));
                    if (!string.IsNullOrEmpty(textContent))
                    {
                        var textNode = new MarkupNode
                        {
                            Content = textContent,
                            Parent = stack.Peek()
                        };
                        stack.Peek().Children.Add(textNode);
                    }
                }

                lastIndex = match.Index + match.Length;
                var isClosing = match.Groups[1].Value == "/";
                var tag = match.Groups[2].Value.ToLowerInvariant();

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

            if (lastIndex < text.Length)
            {
                var remaining = UnescapeText(text.Substring(lastIndex));
                if (!string.IsNullOrEmpty(remaining))
                {
                    var textNode = new MarkupNode
                    {
                        Content = remaining,
                        Parent = stack.Peek()
                    };
                    stack.Peek().Children.Add(textNode);
                }
            }

            return root;
        }

        private static string UnescapeText(string text)
        {
            if (text.IndexOf(@"\[\", StringComparison.InvariantCulture) == -1 &&
                text.IndexOf(@"\]\", StringComparison.InvariantCulture) == -1)
                return text;

            var sb = new System.Text.StringBuilder(text.Length);
            for (int i = 0; i < text.Length; i++)
            {
                // Verifica se é o padrão \[\ ou \]\
                if (text[i] == '\\' && i + 2 < text.Length && text[i + 2] == '\\')
                {
                    var middle = text[i + 1];
                    if (middle == '[' || middle == ']')
                    {
                        sb.Append(middle);
                        i += 2; 
                        continue;
                    }
                }
                sb.Append(text[i]);
            }
            return sb.ToString();
        }
    }
}