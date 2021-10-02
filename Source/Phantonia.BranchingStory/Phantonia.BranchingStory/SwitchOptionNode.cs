using System.Collections.Immutable;

namespace Phantonia.BranchingStory
{
    public sealed record SwitchOptionNode : IHasAttributes, INonBranchingNode
    {
        public SwitchOptionNode(int id, ImmutableDictionary<string, string> attributes)
        {
            Id = id;
            Attributes = attributes;
        }

        public ImmutableDictionary<string, string> Attributes { get; init; }

        public int Id { get; init; }

        public StoryNode? NextNode { get; init; }
    }
}
