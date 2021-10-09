using System;
using System.Collections.Immutable;

namespace Phantonia.BranchingStory
{
    public sealed class Story
    {
        public Story(StoryNode rootNode)
        {
            CurrentNode = rootNode;
            localBranchesTaken = ImmutableDictionary<string, int>.Empty;
        }

        private Story(StoryNode currentNode, ImmutableDictionary<string, int> localBranchesTaken)
        {
            CurrentNode = currentNode;
            this.localBranchesTaken = localBranchesTaken;
        }

        // internal for unit testing
        internal readonly ImmutableDictionary<string, int> localBranchesTaken;

        public StoryNode CurrentNode { get; }

        public bool CanProgressWithOption(int optionId)
        {
            return CurrentNode is SwitchNode switchNode
                && switchNode.Options.TryGetValue(optionId, out SwitchOptionNode? option)
                && option.NextNode is not null;
        }

        public bool CanProgressWithoutOption()
        {
            if (CurrentNode is INonBranchingNode { NextNode: not null })
            {
                return true;
            }

            if (CurrentNode is not PreviousNode previousNode)
            {
                return false;
            }

            if (!localBranchesTaken.TryGetValue(previousNode.TargetedSwitch, out int optionId))
            {
                return false;
            }

            if (previousNode.Branches.ContainsKey(optionId))
            {
                return true;
            }

            return previousNode.ElseNode is not null;
        }

        public Story Progress()
        {
            {
                if (CurrentNode is INonBranchingNode { NextNode: var nextNode })
                {
                    if (nextNode is null)
                    {
                        throw new InvalidOperationException("Reached the end.");
                    }

                    return new Story(nextNode, localBranchesTaken);
                }
            }

            if (CurrentNode is PreviousNode previousNode)
            {
                if (!localBranchesTaken.TryGetValue(previousNode.TargetedSwitch, out int optionId))
                {
                    throw new InvalidOperationException("Somehow a pre node targeted a switch that is not saved.");
                }

                if (!previousNode.Branches.TryGetValue(optionId, out StoryNode? nextNode))
                {
                    nextNode = previousNode.ElseNode;

                    if (nextNode is null)
                    {
                        throw new InvalidOperationException("Reached the end.");
                    }
                }

                return new Story(nextNode, localBranchesTaken);
            }

            throw new InvalidOperationException("Cannot progress without choosing an option in this case.");
        }

        public Story ProgressWithOption(int optionId)
        {
            if (CurrentNode is SwitchNode switchNode)
            {
                if (!switchNode.Options.TryGetValue(optionId, out SwitchOptionNode? option))
                {
                    throw new InvalidOperationException($"The id {optionId} does not exist in the current switch node.");
                }

                if (option.NextNode is null)
                {
                    throw new InvalidOperationException("Reached the end.");
                }

                if (switchNode.Name is not null)
                {
                    ImmutableDictionary<string, int> newBranchesTaken = localBranchesTaken.SetItem(switchNode.Name, optionId);
                    return new Story(option.NextNode, newBranchesTaken);
                }

                return new Story(option.NextNode, localBranchesTaken);
            }

            throw new InvalidOperationException("Cannot progress with an option in this case.");
        }
    }
}
