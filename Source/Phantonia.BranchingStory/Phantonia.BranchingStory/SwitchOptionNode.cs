using System.Collections.Immutable;
using System.Diagnostics;

namespace Phantonia.BranchingStory
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public sealed record SwitchOptionNode : IHasAttributes, INonBranchingNode
    {
        public SwitchOptionNode(int id, string text, StoryNode? nextNode = null, ImmutableDictionary<string, string>? attributes = null)
        {
            Id = id;
            Text = text;
            NextNode = nextNode;
            Attributes = attributes ?? ImmutableDictionary<string, string>.Empty;
        }

        public ImmutableDictionary<string, string> Attributes { get; init; }

        public int Id { get; init; }

        public StoryNode? NextNode { get; init; }

        public string Text { get; init; }

        private string GetDebuggerDisplay() => $"Switch option {Id}{(!string.IsNullOrWhiteSpace(Text) ? ":" + Text : "")}";
    }
}
