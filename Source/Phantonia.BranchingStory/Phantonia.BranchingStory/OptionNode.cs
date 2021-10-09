using System.Collections.Immutable;
using System.Diagnostics;

namespace Phantonia.BranchingStory
{
    public abstract record OptionNode : IHasAttributes, INonBranchingNode
    {
        private protected OptionNode(int id, StoryNode? nextNode, ImmutableDictionary<string, string>? attributes)
        {
            Id = id;
            NextNode = nextNode;
            Attributes = attributes ?? ImmutableDictionary<string, string>.Empty;
        }

        public ImmutableDictionary<string, string> Attributes { get; init; }

        public int Id { get; init; }

        public StoryNode? NextNode { get; init; }
    }
}