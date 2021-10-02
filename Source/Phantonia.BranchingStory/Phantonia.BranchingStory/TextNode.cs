using System.Collections.Immutable;

namespace Phantonia.BranchingStory
{
    public sealed record TextNode : StoryNode, INonBranchingNode
    {
        public TextNode(string text, StoryNode? nextNode = null, ImmutableDictionary<string, string>? attributes = null) : base(attributes)
        {
            Text = text;
            NextNode = nextNode;
        }

        public StoryNode? NextNode { get; init; }

        public string Text { get; init; }
    }
}
