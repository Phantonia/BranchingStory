using System.Collections.Immutable;
using System.Diagnostics;

namespace Phantonia.BranchingStory
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
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

        private string GetDebuggerDisplay() => $"{(IsGlobal ? "Global s" : "S")}witch {(Name is not null ? "named " + Name : "")}: {Options.Count} options";
    }
}
