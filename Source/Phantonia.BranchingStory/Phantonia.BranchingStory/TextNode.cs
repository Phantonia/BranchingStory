using System.Collections.Immutable;

namespace Phantonia.BranchingStory
{
    public sealed record TextNode : StoryNode, INonBranchingNode
    {
        public TextNode(string text, ImmutableDictionary<string, string>? attributes = null) : base(attributes)
        {
            Text = text;
        }

        public StoryNode? NextNode { get; init; }

        public string Text { get; init; }
    }
}
