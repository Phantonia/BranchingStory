using System.Collections.Immutable;

namespace Phantonia.BranchingStory
{
    public sealed record SwitchNode : StoryNode
    {
        public SwitchNode(string name, bool isGlobal, ImmutableDictionary<string, string> attributes, ImmutableDictionary<int, SwitchOptionNode> options) : base(attributes)
        {
            Name = name;
            IsGlobal = isGlobal;
            Attributes = attributes;
            Options = options;
        }

        public bool IsGlobal { get; init; }

        public string Name { get; init; }

        public ImmutableDictionary<int, SwitchOptionNode> Options { get; init; }
    }
}
