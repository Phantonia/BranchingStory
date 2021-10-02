namespace Phantonia.BranchingStory
{
    public interface INonBranchingNode : IHasAttributes
    {
        public abstract StoryNode? NextNode { get; init; }
    }
}
