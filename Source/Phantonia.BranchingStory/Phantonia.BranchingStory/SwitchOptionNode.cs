using System.Collections.Immutable;
using System.Diagnostics;

namespace Phantonia.BranchingStory
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public sealed record SwitchOptionNode : OptionNode, IHasAttributes, INonBranchingNode
    {
        public SwitchOptionNode(int id, string text, StoryNode? nextNode = null, ImmutableDictionary<string, string>? attributes = null) : base(id, nextNode, attributes)
        {
            Text = text;
        }

        public string Text { get; init; }

        private string GetDebuggerDisplay() => $"Switch option {Id}{(!string.IsNullOrWhiteSpace(Text) ? ":" + Text : "")}";
    }
}
