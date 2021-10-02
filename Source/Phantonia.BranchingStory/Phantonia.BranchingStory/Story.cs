using System;

namespace Phantonia.BranchingStory
{
    public sealed class Story
    {
        public Story(StoryNode rootNode)
        {
            CurrentNode = rootNode;
        }

        public StoryNode CurrentNode { get; }

        public bool CanProgressWithOption(int optionId)
        {
            return CurrentNode is not SwitchNode switchNode
                || !switchNode.Options.TryGetValue(optionId, out SwitchOptionNode? option)
                || option.NextNode is null;
        }

        public bool CanProgressWithoutOption()
        {
            return CurrentNode is INonBranchingNode { NextNode: not null };
        }

        public Story Progress()
        {
            if (CurrentNode is INonBranchingNode { NextNode: var nextNode })
            {
                if (nextNode is null)
                {
                    throw new InvalidOperationException("Reached the end.");
                }

                return new Story(nextNode);
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

                return new Story(option.NextNode);
            }

            throw new InvalidOperationException("Cannot progress with an option in this case.");
        }
    }
}
