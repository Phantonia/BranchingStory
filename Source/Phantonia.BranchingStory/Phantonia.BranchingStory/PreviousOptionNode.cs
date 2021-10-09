using System.Collections.Immutable;
using System.Diagnostics;

namespace Phantonia.BranchingStory
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public sealed record PreviousOptionNode : OptionNode
    {
        public PreviousOptionNode(int id, StoryNode? nextNode = null, ImmutableDictionary<string, string>? attributes = null) : base(id, nextNode, attributes) { }

        private string GetDebuggerDisplay() => $"Previous option {Id}";
    }
}
