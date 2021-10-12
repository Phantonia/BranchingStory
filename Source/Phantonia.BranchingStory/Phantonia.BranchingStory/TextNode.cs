using System.Collections.Immutable;
using System.Diagnostics;

namespace Phantonia.BranchingStory
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public sealed record TextNode : StoryNode, INonBranchingNode
    {
        public TextNode(string text, StoryNode? nextNode = null, ImmutableDictionary<string, string>? attributes = null) : base(attributes)
        {
            Text = text;
            NextNode = nextNode;
        }

        public StoryNode? NextNode { get; init; }

        public string Text { get; init; }

        private string GetDebuggerDisplay() => $"Text node: {Text}";
    }
}
