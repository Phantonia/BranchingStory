using System.Collections.Immutable;

namespace Phantonia.BranchingStory
{
    public interface IHasAttributes
    {
        public abstract ImmutableDictionary<string, string> Attributes { get; init; }
    }
}
