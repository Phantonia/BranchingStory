using System.Collections.Immutable;

namespace Phantonia.BranchingStory.Markup
{
    internal sealed record BodyInterpretationResult(StoryNode? EntryPoint, ImmutableList<StoryNode> ExitPoints)
    {
        public ImmutableArray<Error> Errors { get; init; } = ImmutableArray<Error>.Empty;
    }
}
