using System.Collections.Immutable;

namespace Phantonia.BranchingStory
{
    public sealed record PreNode : StoryNode
    {
        public PreNode(string targetedSwitch, ImmutableDictionary<int, StoryNode> branches, StoryNode? elseNode = null, ImmutableDictionary<string, string>? attributes = null) : base(attributes)
        {
            TargetedSwitch = targetedSwitch;
            Branches = branches;
            ElseNode = elseNode;
        }

        public ImmutableDictionary<int, StoryNode> Branches { get; init; }

        public StoryNode? ElseNode { get; init; }

        public string TargetedSwitch { get; init; }
    }
}
