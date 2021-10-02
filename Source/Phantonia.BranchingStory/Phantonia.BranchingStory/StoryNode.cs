using System.Collections.Immutable;

namespace Phantonia.BranchingStory
{
    public abstract record StoryNode : IHasAttributes
    {
        private protected StoryNode(ImmutableDictionary<string, string> attributes)
        {
            Attributes = attributes;
        }

        public ImmutableDictionary<string, string> Attributes { get; init; }

        public StoryNode? ParentNode { get; init; }
    }
}
