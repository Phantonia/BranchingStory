using System.Collections.Immutable;

namespace Phantonia.BranchingStory
{
    public sealed record SwitchNode : StoryNode
    {
        public SwitchNode(ImmutableDictionary<int, SwitchOptionNode> options, string? name = null, bool isGlobal = false, ImmutableDictionary<string, string>? attributes = null) : base(attributes)
        {
            Name = name;
            IsGlobal = isGlobal;
            Options = options;
        }

        public bool IsGlobal { get; init; }

        public string? Name { get; init; }

        public ImmutableDictionary<int, SwitchOptionNode> Options { get; init; }
    }
}
