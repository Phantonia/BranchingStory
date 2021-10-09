using System.Collections.Immutable;
using System.Diagnostics;

namespace Phantonia.BranchingStory
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public sealed record PreviousNode : StoryNode
    {
        public PreviousNode(string targetedSwitch, ImmutableDictionary<int, PreviousOptionNode> branches, StoryNode? elseNode = null, ImmutableDictionary<string, string>? attributes = null) : base(attributes)
        {
            TargetedSwitch = targetedSwitch;
            Branches = branches;
            ElseNode = elseNode;
        }

        public ImmutableDictionary<int, PreviousOptionNode> Branches { get; init; }

        public StoryNode? ElseNode { get; init; }

        public string TargetedSwitch { get; init; }

        private string GetDebuggerDisplay() => $"Pre: {Branches.Count} branches with{(ElseNode is not null ? "out" : "")} else node";
    }
}
