using System.Collections.Immutable;

namespace Phantonia.BranchingStory
{
    public sealed record ActionNode : StoryNode, INonBranchingNode
    {
        public ActionNode(string actionName, ImmutableDictionary<string, string>? attributes = null) : base(attributes)
        {
            ActionName = actionName;
        }

        public string ActionName { get; init; }

        public StoryNode? NextNode { get; init; }
    }
}
