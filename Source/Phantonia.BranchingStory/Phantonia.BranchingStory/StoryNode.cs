using System.Collections.Immutable;

namespace Phantonia.BranchingStory
{
    public abstract record StoryNode : IHasAttributes
    {
        private protected StoryNode(ImmutableDictionary<string, string>? attributes)
        {
            Attributes = attributes ?? ImmutableDictionary<string, string>.Empty;
        }

        public ImmutableDictionary<string, string> Attributes { get; init; }
    }
}
