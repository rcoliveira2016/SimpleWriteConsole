using Xunit;
using SimpleConsole.Parser;
using System.Linq;

namespace SimpleConsole.Test
{
    public class MarkupColorParserTest
    {
        [Fact]
        public void Parse_EmptyString_ReturnsEmptyRootNode()
        {
            // Arrange
            var text = "";

            // Act
            var result = MarkupColorParser.Parse(text);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsTextNode);
            Assert.Empty(result.Children);
        }

        [Fact]
        public void Parse_TextWithoutTags_ReturnsRootWithTextNode()
        {
            // Arrange
            var text = "hello world";

            // Act
            var result = MarkupColorParser.Parse(text);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Children);
            var child = result.Children.First();
            Assert.True(child.IsTextNode);
            Assert.Equal("hello world", child.Content);
        }

        [Fact]
        public void Parse_SimpleColorTag_ReturnsCorrectTree()
        {
            // Arrange
            var text = "[red]hello[/red]";

            // Act
            var result = MarkupColorParser.Parse(text);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Children);
            var redNode = result.Children.First();
            Assert.False(redNode.IsTextNode);
            Assert.Equal("red", redNode.Tag);
            Assert.Single(redNode.Children);
            var textNode = redNode.Children.First();
            Assert.True(textNode.IsTextNode);
            Assert.Equal("hello", textNode.Content);
        }

        [Fact]
        public void Parse_NestedColorTags_ReturnsCorrectTree()
        {
            // Arrange
            var text = "[red]hello [blue]world[/blue][/red]";

            // Act
            var result = MarkupColorParser.Parse(text);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Children);
            var redNode = result.Children.First();
            Assert.Equal("red", redNode.Tag);
            Assert.Equal(2, redNode.Children.Count);

            var textNode1 = redNode.Children[0];
            Assert.True(textNode1.IsTextNode);
            Assert.Equal("hello ", textNode1.Content);

            var blueNode = redNode.Children[1];
            Assert.Equal("blue", blueNode.Tag);
            Assert.Single(blueNode.Children);

            var textNode2 = blueNode.Children.First();
            Assert.True(textNode2.IsTextNode);
            Assert.Equal("world", textNode2.Content);
        }

        [Fact]
        public void Parse_MultipleTagsAtSameLevel_ReturnsCorrectTree()
        {
            // Arrange
            var text = "[red]hello[/red][blue]world[/blue]";

            // Act
            var result = MarkupColorParser.Parse(text);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Children.Count);

            var redNode = result.Children[0];
            Assert.Equal("red", redNode.Tag);
            Assert.Single(redNode.Children);
            Assert.Equal("hello", redNode.Children.First().Content);

            var blueNode = result.Children[1];
            Assert.Equal("blue", blueNode.Tag);
            Assert.Single(blueNode.Children);
            Assert.Equal("world", blueNode.Children.First().Content);
        }

        [Fact]
        public void Parse_MultipleTagsAtMultLevels_ReturnsCorrectTree()
        {
            // Arrange
            var text = "[red]he[blue]llo[/blue][/red] [blue][red]world[/red] :)[/blue]";

            // Act
            var result = MarkupColorParser.Parse(text);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Children.Count);

            var redNode = result.Children[0];
            Assert.Equal("red", redNode.Tag);
            Assert.Equal(2, redNode.Children.Count);

            var redNodeFirthChild = redNode.Children.First();
            Assert.Equal("he", redNodeFirthChild.Content);
            Assert.True(redNodeFirthChild.IsTextNode);

            var redNodeFirthLast = redNode.Children.Last();
            Assert.Single(redNodeFirthLast.Children);
            Assert.Equal("llo", redNodeFirthLast.Children.First().Content);
            Assert.Equal("blue", redNodeFirthLast.Tag);

            var spaceNode = result.Children[1];
            Assert.True(spaceNode.IsTextNode);
            Assert.Empty(spaceNode.Children);
            Assert.Equal(" ", spaceNode.Content);

            var blueNode = result.Children[2];
            Assert.Equal("blue", blueNode.Tag);
            Assert.Equal(2, blueNode.Children.Count);

            var blueNodeFirthChild = blueNode.Children.First();
            Assert.Equal("red", blueNodeFirthChild.Tag);
            Assert.True(blueNodeFirthChild.IsColorTag);
            Assert.Equal("world", blueNodeFirthChild.Children.First().Content);

            var blueNodeFirthLast = blueNode.Children.Last();
            Assert.Empty(blueNodeFirthLast.Children);
            Assert.Equal(" :)", blueNodeFirthLast.Content);
            Assert.True(blueNodeFirthLast.IsTextNode);
        }

        [Fact]
        public void Parse_UnclosedTag_ParsesContentInside()
        {
            // Arrange
            var text = "[red]hello";

            // Act
            var result = MarkupColorParser.Parse(text);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Children);
            var redNode = result.Children.First();
            Assert.Equal("red", redNode.Tag);
            Assert.Single(redNode.Children);
            var textNode = redNode.Children.First();
            Assert.Equal("hello", textNode.Content);
        }

        [Fact]
        public void Parse_InvalidClosingTag_IgnoresTag()
        {
            // Arrange
            var text = "[red]hello[/blue]";

            // Act
            var result = MarkupColorParser.Parse(text);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Children);
            var redNode = result.Children.First();
            Assert.Equal("red", redNode.Tag);
            Assert.Single(redNode.Children);
            var textNode = redNode.Children.First();
            Assert.True(textNode.IsTextNode);
            Assert.Equal("hello", textNode.Content);
        }

        [Fact]
        public void Parse_InvalidColorTag_TreatsAsPlainText()
        {
            // Arrange
            var text = "[invalid]hello[/invalid]";

            // Act
            var result = MarkupColorParser.Parse(text);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Children);
            var textNode = result.Children.First();
            Assert.True(textNode.IsTextNode);
            Assert.Equal("hello", textNode.Content);
        }

        [Fact]
        public void Parse_TagWithUpperCase_IsCaseInsensitive()
        {
            // Arrange
            var text = "[RED]hello[/red]";

            // Act
            var result = MarkupColorParser.Parse(text);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Children);
            var redNode = result.Children.First();
            Assert.Equal("red", redNode.Tag);
            Assert.Single(redNode.Children);
            Assert.Equal("hello", redNode.Children.First().Content);
        }

        [Fact]
        public void Parse_TextWithLeadingAndTrailingContent_ReturnsCorrectTree()
        {
            // Arrange
            var text = "leading [red]hello[/red] trailing";

            // Act
            var result = MarkupColorParser.Parse(text);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Children.Count);

            var leadingText = result.Children[0];
            Assert.True(leadingText.IsTextNode);
            Assert.Equal("leading ", leadingText.Content);

            var redNode = result.Children[1];
            AssetTagAndTextNotTagsChilds(redNode, "red", "hello");

            var trailingText = result.Children[2];
            Assert.True(trailingText.IsTextNode);
            Assert.Equal(" trailing", trailingText.Content);
        }

        [Fact]
        public void Parse_EscapedBrackets_ReturnsTextWithBrackets()
        {
            // Arrange
            var text = "\\[red]hello\\[/red]";

            // Act
            var result = MarkupColorParser.Parse(text);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Children.Count);
            var textNode = result.Children.First();
            Assert.True(textNode.IsTextNode);
            Assert.Equal("\\", textNode.Content);

            AssetTagAndTextNotTagsChilds(result.Children[1], "red", "hello\\");

        }

        [Fact]
        public void Parse_EscapedAndNonEscapedBrackets_ReturnsCorrectTree()
        {
            // Arrange
            var text = "leading \\\\[\\\\red\\\\]\\\\ \\[\\red\\]\\ [blue]hello[/blue] trailing";

            // Act
            var result = MarkupColorParser.Parse(text);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Children.Count);

            var leadingText = result.Children[0];
            Assert.True(leadingText.IsTextNode);
            Assert.Equal("leading \\[\\red\\]\\ [red] ", leadingText.Content);

            var blueNode = result.Children[1];
            AssetTagAndTextNotTagsChilds(blueNode, "blue", "hello");

            var trailingText = result.Children[2];
            Assert.True(trailingText.IsTextNode);
            Assert.Equal(" trailing", trailingText.Content);
        }

        private void AssetTagAndTextNotTagsChilds(MarkupNode node, string tag, string text)
        {
            Assert.Equal(tag, node.Tag);
            Assert.Single(node.Children);
            var textNode = node.Children.First();
            Assert.True(textNode.IsTextNode);
            Assert.Equal(text, textNode.Content);
        }
    }
}
