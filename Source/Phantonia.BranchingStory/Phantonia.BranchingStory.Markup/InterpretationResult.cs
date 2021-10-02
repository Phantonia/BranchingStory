using System.Collections.Immutable;

namespace Phantonia.BranchingStory.Markup
{
    public sealed record InterpretationResult
    {
        public InterpretationResult() { }

        public ImmutableArray<Error> Errors { get; init; } = ImmutableArray<Error>.Empty;

        public Story? Story { get; init; }
    }
}
